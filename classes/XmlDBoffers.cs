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

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBoffers : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			offer ourOffer = new offer();
			if (PKey > 0)
			{
				ourOffer = (from p in AdminDC.offers
							  where p.offer_key == PKey
							  select p).SingleOrDefault();
			}
			return ourOffer;
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
			offer ourData = new offer();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.offers.Single(p => p.offer_key == PKey);
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
						Logger.LogError("Update Failed", new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				string prefix = ConfigurationManager.AppSettings["urlprefixOffer"] ?? String.Empty;
				ourData.url = String.Format("{0}{1}", prefix, SQLHelpers.URLSafe(ourData.title));
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
					labelStatus.Text = String.Format("{0} Inserted", Table.ID);
				
					PKey = ourData.offer_key;
				
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
			offer oldOffer =(from o in AdminDC.offers
								where o.offer_key == iKey
								select o).SingleOrDefault();
			
			DataEntities.Archive.offer archiveOffer = new DataEntities.Archive.offer
			                                          	{
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
				DBName = AdminDC.Connection.Database
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
			
			archiveDC.offers.InsertOnSubmit(archiveOffer);
			AdminDC.offers.DeleteOnSubmit(oldOffer);
			
			archiveDC.SubmitChanges();
			AdminDC.SubmitChanges();
									
		}

	}

}