using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.Repositories;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.Services;
using System.Web;

namespace mjjames.AdminSystem
{
	public class XmlDBmarketingsites : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{

			var ourSite = new marketingsite();
			if (PKey > 0)
			{
				ourSite = (from p in AdminDC.marketingsites
						   where p.marketingsite_key == PKey
						   select p).SingleOrDefault();
			}

			return ourSite;
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
			var idsThatCauseSiteMapCacheClear = new[] { "active"};

			var ourSender = (Button)sender;
			var ourPageDataContext = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			var ourData = new marketingsite();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.marketingsites.Single(p => p.marketingsite_key == PKey);
			}

			var keyvalues = new List<KeyValueData>();
			foreach (var tab in Table.Tabs)
			{
				var ourTab = (WebControl)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab == null) continue;
				foreach (var field in tab.Fields)
				{
					var ourControl = ourTab.FindControl("control" + field.ID);

					//if we cant find a control for that ID or its of type photogallery skip it
					if (ourControl == null || field.Type.Equals("photogallery", StringComparison.InvariantCultureIgnoreCase)) continue;

					//if we are a key value get our data out and stash it for later
					if (field.Attributes.ContainsKey("keyvalue"))
					{
						keyvalues.Add(new KeyValueData
										{
											LinkKey = PKey,
											Value = GetDataValue(ourControl, field.Type, typeof(String)) as String,
											LinkTypeID = "marketingsitelookup",
											LookupID = field.Attributes["lookupid"]
										});
						continue;
					}

					var ourProperty = ourData.GetType().GetProperty(field.ID);
					if (ourProperty != null)
					{
						Logger.LogInformation("Saving Content In: " + ourControl.ID);//get our new value
						var newValue = GetDataValue(ourControl, field.Type, ourProperty.PropertyType);
						//if we haven't already got a clear sitemap cache value and our current id is that of one we must check 
						//compare the old and new values and assign to clearSiteMap we only want true if the values aren't equal as thats a change
						if (!clearSiteMapCache && idsThatCauseSiteMapCacheClear.Contains(field.ID))
						{
							clearSiteMapCache = !newValue.Equals(ourProperty.GetValue(ourData, null));
						}
						ourProperty.SetValue(ourData, newValue, null);
					}
					else
					{
						Logger.LogError("Error Saving Content: " + ourControl.ID, new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
    			ourData.site_fkey = SiteFKey;
				ourPageDataContext.marketingsites.InsertOnSubmit(ourData);
			}

			var labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				var ourChanges = ourPageDataContext.GetChangeSet();


				ourPageDataContext.SubmitChanges();

				var updateType = UpdateType.None;
				if (ourChanges.Inserts.Count > 0)
				{
					updateType = UpdateType.Inserted;
                    AuditLogService.LogItem("MarketingSites",
                        Models.AuditEvent.Created,
                        HttpContext.Current.User.Identity.Name,
                        ourData.name);

					PKey = ourData.marketingsite_key;
					//when we do an insert update any keyvalues we have to have the correct primary key
					keyvalues = keyvalues.Select(kv => new KeyValueData()
					                                   	{
															LinkKey = PKey,
															LinkTypeID = kv.LinkTypeID,
															LookupID = kv.LookupID,
															Value =  kv.Value
					                                   	}).ToList();

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
						var ex =
							new Exception(String.Format("{0} doesn't contain a hidden control called {1}", Table.ID, TablePrimaryKeyField));
						Logger.LogError("Unknown Field", ex);
						throw ex;
					}
					clearSiteMapCache = true; //new page so clear the caches

				}
				if (ourChanges.Updates.Count > 0)
				{
					updateType = UpdateType.Updated;
                    AuditLogService.LogItem("MarketingSites",
                        Models.AuditEvent.Updated,
                        HttpContext.Current.User.Identity.Name,
                        ourData.name);
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
						labelStatus.Text = String.Format("{0} Inserted", Table.Label);
						break;
					case UpdateType.Updated:
						labelStatus.Text = String.Format("{0} Updated", Table.Label);
						break;
				}
				//following an insert or an update to particular field we must reset a site's sitemap cache to allow our changes to pull through
				if (clearSiteMapCache)
				{
					GenericFunctions.ResetSiteMap();
				}

			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed", Table.Label);
				Logger.LogError("Failed Update", ex);
			}
		}

		private object UpdatePrimaryKey(KeyValueData kv)
		{
			kv.LinkKey = PKey;
			return kv;
		}

		#endregion


	}

}