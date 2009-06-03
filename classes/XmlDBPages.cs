using System;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web;
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
	public class XmlDBpages : XmlDBBase
	{
		/// <summary>
		/// constructor
		/// </summary>
		public XmlDBpages()
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
			///TODO replace this out as its rubbish but dynamic linq isnt easy
			
			object ourData = new object();


			page ourPage = new page();

			if (_iPKey > 0)
			{
				ourPage = (from p in adminDC.pages
						   where p.page_key == _iPKey
						   select p).SingleOrDefault();
			}

			if (_iFKey > 0)
			{
				ourPage.page_fkey = _iFKey;
			}
			else
			{
				_iFKey = ourPage.page_fkey == null ? 0 : ourPage.page_fkey.Value;
			}

			ourData = ourPage;
			return ourData;
		}

		public override void ArchiveData(int iKey)
		{
			page ourPage = (from p in adminDC.pages
							where p.page_key == iKey
							select p).SingleOrDefault();
							
			DataContexts.Archive.archiveDataContext archiveDC = new mjjames.AdminSystem.DataContexts.Archive.archiveDataContext();
			DataEntities.Archive.page archivePage = new mjjames.AdminSystem.DataEntities.Archive.page();
			
			archivePage.page_key = ourPage.page_key;
			archivePage.page_fkey = ourPage.page_fkey;
			archivePage.accesskey = ourPage.accesskey;
			archivePage.active = ourPage.active;
			archivePage.body = ourPage.body;
			archivePage.linkurl = ourPage.linkurl;
			archivePage.metadescription = ourPage.metadescription;
			archivePage.metakeywords = ourPage.metakeywords;
			archivePage.navtitle = ourPage.navtitle;
			archivePage.page_url = ourPage.page_url;
			archivePage.pageid = ourPage.pageid;
			archivePage.password = ourPage.password;
			archivePage.passwordprotect = ourPage.passwordprotect;
			archivePage.showinfeaturednav = ourPage.showinfeaturednav;
			archivePage.showinfooter = ourPage.showinfooter;
			archivePage.showinnav = ourPage.showinnav;
			archivePage.showonhome = ourPage.showonhome;
			archivePage.sortorder = ourPage.sortorder;
			archivePage.thumbnailimage = ourPage.thumbnailimage;
			archivePage.title = ourPage.title;
			archivePage.DBName = adminDC.Mapping.DatabaseName;
			
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
		protected override void saveEdit(object sender, EventArgs e)
		{
			Button ourSender = (Button)sender;
			adminDataContext ourPageDataContext = new adminDataContext();
			page ourData = new page();
			if (_iPKey > 0)
			{
				ourData = ourPageDataContext.pages.Single(p => p.page_key == _iPKey);
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

				ourPageDataContext.pages.InsertOnSubmit(ourData);
				ourData.page_fkey = _iFKey;


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


					_iPKey = ((page)ourData).page_key;
					_iFKey = (int)((page)ourData).page_fkey;
					HiddenField ourControlFKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "controlpage_fkey");
					ourControlFKey.Value = _iFKey.ToString();

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