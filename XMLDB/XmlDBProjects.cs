using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;
using System.Collections.Generic;
using mjjames.AdminSystem.Repositories;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBprojects : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			project ourProject = new project();
			if (PKey > 0)
			{
				ourProject = (from p in AdminDC.projects
							  where p.project_key == PKey
							  select p).SingleOrDefault();
			}

			return ourProject;
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
			Button ourSender = (Button)sender;
			AdminDataContext ourPageDataContext =new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			project ourData = new project();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.projects.Single(p => p.project_key == PKey);
			}
			var keyvalues = new List<KeyValueData>();
			foreach (AdminTab tab in Table.Tabs)
			{
				TabPanel ourTab = (TabPanel)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab == null) continue;
				foreach (AdminField field in tab.Fields)
				{
					Control ourControl = ourTab.FindControl("control" + field.ID);

					if (ourControl == null) continue;

					//if we are a key value get our data out and stash it for later
					if (field.Attributes.ContainsKey("keyvalue"))
					{
						keyvalues.Add(new KeyValueData
						{
							LinkKey = PKey,
							Value = GetDataValue(ourControl, field.Type, typeof(String)) as String,
							LinkTypeID = "projectlookup",
							LookupID = field.Attributes["lookupid"]
						});
						continue;
					}

					PropertyInfo ourProperty = ourData.GetType().GetProperty(field.ID);
					if (ourProperty != null)
					{
						Logger.LogInformation("Saving Content In: " + ourControl.ID);
						ourProperty.SetValue(ourData, GetDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
					}
					else
					{
						Logger.LogError("Update Failed", new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				string prefix = ConfigurationManager.AppSettings["urlprefixProject"] ?? String.Empty;
				prefix = prefix.Replace("[*year]*", ourData.start_date.GetValueOrDefault(DateTime.Now).Year.ToString());
				ourData.url = String.Format("{0}{1}", prefix, SQLHelpers.URLSafe(ourData.title));
				if (MultiTenancyEnabled)
				{
					ourData.site_fkey = SiteFKey;
				}
				ourPageDataContext.projects.InsertOnSubmit(ourData);
			}

			Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				ChangeSet ourChanges = ourPageDataContext.GetChangeSet();

				labelStatus.Text = "Nothing to Save";

				ourPageDataContext.SubmitChanges();

				var updateType = UpdateType.None;
				if (ourChanges.Inserts.Count > 0)
				{
					updateType = UpdateType.Inserted;

					PKey = ourData.project_key;
				
					string strPKeyField = TablePrimaryKeyField;

					HiddenField ourPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "pkey");
					HiddenField ourControlPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "control" + strPKeyField);

					try
					{
						ourControlPKey.Value = PKey.ToString();
						ourPKey.Value = PKey.ToString();
					}
					catch
					{
						Exception ex = new Exception(String.Format("{0} doesn't contain a hidden control called {1}", Table.ID, TablePrimaryKeyField));
						Logger.LogError("Update Failed", ex);
						throw ex;
					}

					if (Convert.ToBoolean(ConfigurationManager.AppSettings["twitterPublishProjects"]) && ourData.active)
					{
						TwitterPublisher tp = new TwitterPublisher(ConfigurationManager.AppSettings["twitterConsumerKey"],
																	ConfigurationManager.AppSettings["twitterConsumerSecret"],
																	ConfigurationManager.AppSettings["twitterAuthenticationToken"],
																	ConfigurationManager.AppSettings["twitterAuthenticationTokenSecret"]);

						ourPageDataContext.Refresh(RefreshMode.OverwriteCurrentValues, ourData);

						string url = String.Format("http://{0}/{1}", ConfigurationManager.AppSettings["DomainName"], ourData.url);

						int length = ourData.title.Length;
						if (length > 100)
						{
							length = 100;
						}
						string message = String.Format("{0} - {1}", ourData.title.Substring(0, length), url);

						if (!tp.PublishMessage(message))
						{
							labelStatus.Text += " - Twitter Update Failed";
						}
						labelStatus.Text += " - Twitter Update Succeeded";

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
				Logger.LogError("Project Update Failed", ex);
			}
		}

		#endregion

		public override void ArchiveData(int iKey)
		{
			project oldProject = (from p in AdminDC.projects
									where p.project_key == iKey
									select p).SingleOrDefault();
			
			DataEntities.Archive.project archiveProject = new DataEntities.Archive.project
			                                              	{
				active = oldProject.active,
				description = oldProject.description,
				end_date = oldProject.end_date,
				include_in_rss = oldProject.include_in_rss,
				photogallery_id = oldProject.photogallery_id,
				project_key = oldProject.project_key,
				start_date = oldProject.start_date,
				title = oldProject.title,
				url = oldProject.url,
				video_id = oldProject.video_id,
				DBName = AdminDC.Connection.Database
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
			archiveDC.projects.InsertOnSubmit(archiveProject);
			AdminDC.projects.DeleteOnSubmit(oldProject);
			
			archiveDC.SubmitChanges();
			AdminDC.SubmitChanges();
		}


	}

}