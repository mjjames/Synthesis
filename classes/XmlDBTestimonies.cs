using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBtestimonies : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			testimony ourTestimony = new testimony();
			if (PKey > 0)
			{
				ourTestimony = (from p in AdminDC.testimonies
							  where p.testimony_key == PKey
							  select p).SingleOrDefault();
			}
			
			return ourTestimony;
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
			testimony ourData = new testimony();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.testimonies.Single(p => p.testimony_key == PKey);
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
						Logger.LogError("Error Updating Content", new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				ourPageDataContext.testimonies.InsertOnSubmit(ourData);
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


					PKey = ourData.testimony_key;
				
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
						Logger.LogError("Content has no Primary Key", ex);
						throw ex;
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
				Logger.LogError("Update Failed", ex);
			}
		}

		#endregion

		public override void ArchiveData(int iKey)
		{
			testimony oldTestimony = (from t in AdminDC.testimonies
										where t.testimony_key == iKey
										select t).SingleOrDefault();
										
			DataEntities.Archive.testimony archiveTestimony = new DataEntities.Archive.testimony
			                                                  	{
				active = oldTestimony.active,
				description = oldTestimony.description,
				project_fkey = oldTestimony.project_fkey,
				testimony_key = oldTestimony.testimony_key,
				title = oldTestimony.title,
				url = oldTestimony.url,
				video_id = oldTestimony.video_id,
				DBName = AdminDC.Connection.Database
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
			
			archiveDC.testimonies.InsertOnSubmit(archiveTestimony);
			AdminDC.testimonies.DeleteOnSubmit(oldTestimony);
			
			archiveDC.SubmitChanges();
			AdminDC.SubmitChanges();
		}

	}

}