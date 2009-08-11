using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBoffers : XmlDBBase
	{
		/// <summary>
		/// constructor
		/// </summary>
		public XmlDBoffers()
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
			
			offer ourOffer = new offer();
			if (_iPKey > 0)
			{
				ourOffer = (from p in adminDC.offers
							  where p.offer_key == _iPKey
							  select p).SingleOrDefault();
			}
			ourData = ourOffer;

			return ourData;
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
			AdminDataContext ourPageDataContext = new AdminDataContext();
			offer ourData = new offer();
			if (_iPKey > 0)
			{
				ourData = ourPageDataContext.offers.Single(p => p.offer_key == _iPKey);
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
				ourPageDataContext.offers.InsertOnSubmit(ourData);
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


					_iPKey = ((offer)ourData).offer_key;
				
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

					if (Convert.ToBoolean(ConfigurationManager.AppSettings["twitterPublishOffers"]) && ourData.active)
					{
						TwitterPublisher tp = new TwitterPublisher(ConfigurationManager.AppSettings["twitterConsumerKey"],
																	ConfigurationManager.AppSettings["twitterConsumerSecret"],
																	ConfigurationManager.AppSettings["twitterAuthenticationToken"],
																	ConfigurationManager.AppSettings["twitterAuthenticationTokenSecret"]);

						ourPageDataContext.Refresh(RefreshMode.OverwriteCurrentValues, ourData);

						string url = String.Format("http://{0}/offers/{1}", ConfigurationManager.AppSettings["DomainName"], ourData.url);

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
					labelStatus.Text = String.Format("{0} Updated", atTable.ID);
				}


			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed: {1}", atTable.ID, ex);
			}
		}

		#endregion

		public override void ArchiveData(int iKey)
		{
			offer oldOffer =(from o in adminDC.offers
								where o.offer_key == iKey
								select o).SingleOrDefault();
			
			DataEntities.Archive.offer archiveOffer = new mjjames.AdminSystem.DataEntities.Archive.offer(){
				active = oldOffer.active,
				description = oldOffer.description,
				offer_end = oldOffer.offer_end,
				offer_key = oldOffer.offer_key,
				offer_start = oldOffer.offer_start,
				shortdescription = oldOffer.shortdescription,
				showinfeed = oldOffer.showinfeed,
				showonhome = oldOffer.showonhome,
				title = oldOffer.title,
				url = oldOffer.url,
				thumbnailimage = oldOffer.thumbnailimage,
				DBName = adminDC.Connection.Database
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new mjjames.AdminSystem.DataContexts.Archive.archiveDataContext();
			
			archiveDC.offers.InsertOnSubmit(archiveOffer);
			adminDC.offers.DeleteOnSubmit(oldOffer);
			
			archiveDC.SubmitChanges();
			adminDC.SubmitChanges();
									
		}

	}

}