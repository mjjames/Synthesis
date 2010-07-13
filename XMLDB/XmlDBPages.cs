using System;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;
using System.Web.Configuration;
using System.Collections.Generic;
using mjjames.AdminSystem.Repositories;

namespace mjjames.AdminSystem
{
	public class XmlDBpages : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			var ourPage = new page();

			if (PKey > 0)
			{
				ourPage = (from p in AdminDC.pages
						   where p.page_key == PKey
						   select p).SingleOrDefault();
			}

			if (FKey > 0)
			{
				ourPage.page_fkey = FKey;
			}
			else
			{
				FKey = ourPage.page_fkey == null ? 0 : ourPage.page_fkey.Value;
			}

			return ourPage;
		}

		public override void ArchiveData(int iKey)
		{
			page ourPage = (from p in AdminDC.pages
							where p.page_key == iKey
							select p).SingleOrDefault();
							
			var archiveDC = new DataContexts.Archive.archiveDataContext();
			var archivePage = new DataEntities.Archive.page
														{
															page_key = ourPage.page_key,
															page_fkey = ourPage.page_fkey,
															accesskey = ourPage.accesskey,
															active = ourPage.active,
															body = ourPage.body,
															linkurl = ourPage.linkurl,
															metadescription = ourPage.metadescription,
															metakeywords = ourPage.metakeywords,
															navtitle = ourPage.navtitle,
															page_url = ourPage.page_url,
															pageid = ourPage.pageid,
															password = ourPage.password,
															passwordprotect = ourPage.passwordprotect,
															showinfeaturednav = ourPage.showinfeaturednav,
															showinfooter = ourPage.showinfooter,
															showinnav = ourPage.showinnav,
															showonhome = ourPage.showonhome,
															sortorder = ourPage.sortorder,
															thumbnailimage = ourPage.thumbnailimage,
															title = ourPage.title,
															DBName = AdminDC.Connection.Database
														};
			Logger.LogInformation("Archiving Page - " + iKey);
			archiveDC.pages.InsertOnSubmit(archivePage);
			archiveDC.SubmitChanges();
		}

		#endregion

		#region button events

		/// <summary>
		/// save away our data / insert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void SaveEdit(object sender, EventArgs e)
		{
			var clearSiteMapCache = false;
			var idsThatCauseSiteMapCacheClear = new[] { "active", "showinnav", "showinfooter", "showonhome", "showinheader"};
			var ourSender = (Button)sender;
			var ourPageDataContext =new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			var ourData = new page();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.pages.Single(p => p.page_key == PKey);
			}
			var keyvalues = new List<KeyValueData>();
			foreach (AdminTab tab in Table.Tabs)
			{
				var ourTab = (TabPanel)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab == null) continue;
				foreach (var field in tab.Fields)
				{
					//skip the url field as it's readonly
					if (field.ID.ToLower() == "page_url") continue;

					var ourControl = ourTab.FindControl("control" + field.ID);

					if (ourControl == null) continue;
					
					//if we are a key value get our data out and stash it for later
					if (field.Attributes.ContainsKey("keyvalue"))
					{
						keyvalues.Add(new KeyValueData
						{
							LinkKey = PKey,
							Value = GetDataValue(ourControl, field.Type, typeof(String)) as String,
							LinkTypeID = "pagelookup",
							LookupID = field.Attributes["lookupid"]
						});
						continue;
					}

					var ourProperty = ourData.GetType().GetProperty(field.ID);
					
					if (ourProperty != null)
					{
						//get our new value
						var newValue = GetDataValue(ourControl, field.Type, ourProperty.PropertyType);
						//if we haven't already got a clear sitemap cache value and our current id is that of one we must check compare the old and new values and assign to clearSiteMap
						if(!clearSiteMapCache && idsThatCauseSiteMapCacheClear.Contains(field.ID)){
							clearSiteMapCache = newValue.Equals(ourProperty.GetValue(ourData, null));
						}
						ourProperty.SetValue(ourData, newValue , null);
					}
					else
					{
						Logger.LogError("Update Error", new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				var prefix = ConfigurationManager.AppSettings["urlprefixPage"] ?? String.Empty;
				ourData.page_url= String.Format("{0}{1}", prefix, SQLHelpers.URLSafe(ourData.title));
				//we always set the sitefkey as we must always have a site
				//if(MultiTenancyEnabled){
					ourData.site_fkey = SiteFKey;
				//}
				ourPageDataContext.pages.InsertOnSubmit(ourData);
				ourData.page_fkey = FKey;


			}

			var labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				var ourChanges = ourPageDataContext.GetChangeSet();

				labelStatus.Text = "Nothing to Save";

				ourPageDataContext.SubmitChanges();

				var updateType = UpdateType.None;
				if (ourChanges.Inserts.Count > 0)
				{
					updateType = UpdateType.Inserted;

					PKey = ourData.page_key;
					if (ourData.page_fkey != null) FKey = (int)ourData.page_fkey;
					var ourControlFKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "controlpage_fkey");
					ourControlFKey.Value = FKey.ToString();

					var strPKeyField = TablePrimaryKeyField;

					var ourPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "pkey");
					var ourControlPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "control" + strPKeyField);


					try
					{
						ourControlPKey.Value = PKey.ToString();
						ourPKey.Value = PKey.ToString();
						
					}
					catch
					{
						var ex = new Exception(String.Format("{0} doesn't contain a hidden control called {1}", Table.ID, TablePrimaryKeyField));
						Logger.LogError("Page Update Failed", ex);
					}
				}
				if (ourChanges.Updates.Count > 0)
				{
					updateType = UpdateType.Updated;
				}
				if (keyvalues.Count > 0)
				{
					var keyValueRepository = new KeyValueRepository();
					Logger.LogInformation("Updating Key Values");
					keyValueRepository.UpdateKeyValues(keyvalues);
					if (updateType.Equals(UpdateType.None))
					{
						updateType = UpdateType.Updated;
					}
				}

				switch (updateType)
				{
					case UpdateType.None:
						labelStatus.Text = "Nothing to Save";
						break;
					case UpdateType.Inserted:
						labelStatus.Text = String.Format("{0} Inserted", Table.ID);
						break;
					case UpdateType.Updated:
						labelStatus.Text = String.Format("{0} Updated", Table.ID);
						break;
				}


			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed", Table.ID);
				Logger.LogError("Page Update Failed", ex);
			}
			//following an insert or an update to particular field we must reset a site's sitemap cache to allow our changes to pull through
			if(clearSiteMapCache){
				//easiest way to force this is to open the main web.config and save it - performance eek but currently this is a work around
				//ideally need to look at cache dependencies
				WebConfigurationManager.OpenWebConfiguration("/").Save(ConfigurationSaveMode.Minimal, true);
				System.Diagnostics.Debug.WriteLine("Restarted Site Following Page Navigation Changes", WebConfigurationManager.AppSettings["sitename"] + " Admin System");
			}
		}

		#endregion


	}

}