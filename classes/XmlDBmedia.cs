using System;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.dataentities;
using mjjames.core.dataentites;
using System.Configuration;
using mjjames.core;
using System.Collections.Generic;
using System.Net.Mail;
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
		/// <summary>
		/// constructor
		/// </summary>
		public XmlDBmedia()
			: base()
		{

		}


		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			object ourData = new object();
			
			media ourMedia = new media();
			if (_iPKey > 0)
			{
				ourMedia = (from p in adminDC.medias
							  where p.media_key == _iPKey
							  select p).SingleOrDefault();
			}
			ourData = ourMedia;

			return ourData;
		}




		#endregion

		public override void ArchiveData(int iKey)
		{
			DataEntities.media oldMedia = (from m in adminDC.medias
											where m.media_key == iKey
											select m).SingleOrDefault();
			
			DataEntities.Archive.media archiveMedia = new mjjames.AdminSystem.DataEntities.Archive.media(){
				active = oldMedia.active,
				description = oldMedia.description,
				filename = oldMedia.filename,
				media_key = oldMedia.media_key,
				mediatype_lookup = oldMedia.mediatype_lookup,
				title = oldMedia.title,
				DBName = adminDC.Mapping.DatabaseName
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new mjjames.AdminSystem.DataContexts.Archive.archiveDataContext();
			
			archiveDC.medias.InsertOnSubmit(archiveMedia);
			adminDC.medias.DeleteOnSubmit(oldMedia);
			
			archiveDC.SubmitChanges();
			adminDC.SubmitChanges();
		}

		#region button events


		/// <summary>
		/// save away our data / insert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void saveEdit(object sender, EventArgs e)
		{
			Button ourSender = (Button)sender;
			adminDataContext ourPageDataContext = new adminDataContext();
			media ourData = new media();
			if (_iPKey > 0)
			{
				ourData = ourPageDataContext.medias.Single(p => p.media_key == _iPKey);
			}

			var ourfields = from fields in atTable.Tabs
							select new
							{
								ID = fields.ID
							};

			foreach (AdminTab tab in atTable.Tabs)
			{
				TabPanel ourTab = (TabPanel)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab != null)
				{
					foreach (AdminField field in tab.Fields)
					{
						Control ourControl = (Control)ourTab.FindControl("control" + field.ID);

						if (ourControl != null)
						{
							PropertyInfo ourProperty = ourData.GetType().GetProperty(field.ID);
							if (ourProperty != null)
							{
								HttpContext.Current.Trace.Warn("Saving Content In: " + ourControl.ID);
								ourProperty.SetValue(ourData, getDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
							}
							else
							{
								HttpContext.Current.Trace.Warn("Error Saving Content: " + ourControl.ID);
							}
						}
					}
				}
			}

			if (_iPKey == 0)
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
					labelStatus.Text = String.Format("{0} Inserted", atTable.ID);
					string strPKeyField = String.Empty;


					_iPKey = ((media)ourData).media_key;

					strPKeyField = TablePrimaryKeyField;

					HiddenField ourPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "pkey");
					HiddenField ourControlPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "control" + strPKeyField);

					try
					{
						ourControlPKey.Value = _iPKey.ToString();
						ourPKey.Value = _iPKey.ToString();
					}
					catch
					{
						throw new Exception(String.Format("{0} doesn't contain a hidden control called {1}", atTable.ID, TablePrimaryKeyField));
					}
				}
				if (ourChanges.Updates.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Updated", atTable.ID);
				}


			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed: {1}", atTable.ID, ex);
			}
		}



		#endregion


	}

}