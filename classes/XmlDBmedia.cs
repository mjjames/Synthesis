using System;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.dataentities;
using System.Configuration;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBmedia : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			
			media ourMedia = new media();
			if (PKey > 0)
			{
				ourMedia = (from p in AdminDC.medias
							  where p.media_key == PKey
							  select p).SingleOrDefault();
			}

			return ourMedia;
		}




		#endregion

		public override void ArchiveData(int iKey)
		{
			media oldMedia = (from m in AdminDC.medias
											where m.media_key == iKey
											select m).SingleOrDefault();
			
			DataEntities.Archive.media archiveMedia = new DataEntities.Archive.media
			                                          	{
				active = oldMedia.active,
				description = oldMedia.description,
				filename = oldMedia.filename,
				media_key = oldMedia.media_key,
				mediatype_lookup = oldMedia.mediatype_lookup,
				title = oldMedia.title,
				DBName = AdminDC.Connection.Database
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
			
			archiveDC.medias.InsertOnSubmit(archiveMedia);
			AdminDC.medias.DeleteOnSubmit(oldMedia);
			
			archiveDC.SubmitChanges();
			AdminDC.SubmitChanges();
		}

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
			media ourData = new media();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.medias.Single(p => p.media_key == PKey);
			}

			foreach (AdminTab tab in Table.Tabs)
			{
				TabPanel ourTab = (TabPanel)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab == null) continue;
				foreach (AdminField field in tab.Fields)
				{
					Control ourControl = ourTab.FindControl("control" + field.ID);

					if (ourControl == null) continue;
					PropertyInfo ourProperty = ourData.GetType().GetProperty(field.ID);
					if (ourProperty != null)
					{
						Logger.LogInformation("Saving Content In: " + ourControl.ID);
						ourProperty.SetValue(ourData, GetDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
					}
					else
					{	
						Logger.LogError("Save Content Failed", new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				ourPageDataContext.medias.InsertOnSubmit(ourData);
			}

			Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				ChangeSet ourChanges = ourPageDataContext.GetChangeSet();

				labelStatus.Text = "Nothing to Save";

				
				ourPageDataContext.SubmitChanges();

				if (ourChanges.Inserts.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Inserted", Table.ID);


					PKey = ourData.media_key;

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
						Logger.LogError("Missing Primary Key Field", ex);
					}
				}
				if (ourChanges.Updates.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Updated", Table.ID);
				}


			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed", Table.ID);
				Logger.LogError("Update Error", ex);
			}
		}



		#endregion


	}

}