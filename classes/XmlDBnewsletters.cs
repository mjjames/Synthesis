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
using System.Collections.Generic;
using System.Net.Mail;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;
using mjjames.core;
using mjjames.core.dataentites;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBnewsletters : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			Newsletter ourOffer = new Newsletter();
			if (_iPKey > 0)
			{
				ourOffer = (from p in adminDC.Newsletters
							  where p.newsletter_key == _iPKey
							  select p).SingleOrDefault();
			}
			object ourData = ourOffer;

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
			Newsletter ourData = new Newsletter();
			if (_iPKey > 0)
			{
				ourData = ourPageDataContext.Newsletters.Single(p => p.newsletter_key == _iPKey);
			}

			foreach (AdminTab tab in atTable.Tabs)
			{
				TabPanel ourTab = (TabPanel)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab != null)
				{
					foreach (AdminField field in tab.Fields)
					{
						Control ourControl = ourTab.FindControl("control" + field.ID);

						if (ourControl == null) continue;
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

			if (_iPKey == 0)
			{
				ourPageDataContext.Newsletters.InsertOnSubmit(ourData);
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


					_iPKey = ourData.newsletter_key;

					string strPKeyField = TablePrimaryKeyField;

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



		/// <summary>
		/// Take the content, build the newsletter and send it to the emailer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void emailNewsletter(object sender, EventArgs e)
		{
			saveEdit(sender, e); //save first then build the email

			Button ourSender = (Button)sender;
			Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));


			string fromemail = ConfigurationManager.AppSettings["NewsletterFromAddress"] ?? String.Empty;
			string fromname = ConfigurationManager.AppSettings["SiteName"] ?? String.Empty;

			Newsletter ourNewsletter = (Newsletter) GetData();
			email newsletter = new email
			                   	{
			                   		fromemail = fromemail,
			                   		fromname = fromname,
			                   		subject = ourNewsletter.subject ?? String.Empty,
			                   		body = ourNewsletter.body ?? String.Empty,
			                   		reciprients = GetReciprients(),
			                   		unsubscribelink = ConfigurationManager.AppSettings["NewsletterUnSubscribe"]
			                   	};

			Emailer mailer = new Emailer {Email = newsletter};

			if (mailer.SendMail())
			{
				labelStatus.Text = "Newsletter Sent";
				ourNewsletter.date_sent = DateTime.Now;

			}
			else
			{
				labelStatus.Text = "Newsletter Failed To Send";
			}


		}
		#endregion

		private List<MailAddress> GetReciprients(){
			List<MailAddress> reciprients = (from r in adminDC.NewsletterReciprients
											 where r.active && r.confirmed
											 select new MailAddress(r.email, r.name)).ToList();
			return reciprients;
		}

		public override void ArchiveData(int iKey)
		{
			Newsletter oldNews = (from n in adminDC.Newsletters
									where n.newsletter_key == iKey
									select n).SingleOrDefault();
			
			DataEntities.Archive.Newsletter archiveNews = new DataEntities.Archive.Newsletter
			                                              	{
				body = oldNews.body,
				date_created = oldNews.date_created,
				date_sent = oldNews.date_sent,
				newsletter_key = oldNews.newsletter_key,
				subject = oldNews.subject,
				DBName = adminDC.Connection.Database
			};
			
			DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
			
			archiveDC.Newsletters.InsertOnSubmit(archiveNews);
			adminDC.Newsletters.DeleteOnSubmit(oldNews);
			
			archiveDC.SubmitChanges();
			adminDC.SubmitChanges();
			
			
		}


	}

}