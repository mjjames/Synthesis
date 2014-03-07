using System;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
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
            if (PKey > 0)
            {
                ourOffer = (from p in AdminDC.Newsletters
                            where p.newsletter_key == PKey
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
        protected override void SaveEdit(object sender, EventArgs e)
        {
            Button ourSender = (Button)sender;
            AdminDataContext ourPageDataContext = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            Newsletter ourData = new Newsletter();
            if (PKey > 0)
            {
                ourData = ourPageDataContext.Newsletters.Single(p => p.newsletter_key == PKey);
            }

            foreach (AdminTab tab in Table.Tabs)
            {
                var ourTab = (WebControl)FindControlRecursive(ourSender.Page, tab.ID);
                if (ourTab == null)
                {
                    continue;
                }
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
                        Exception ex = new Exception("Error Saving Content: " + ourControl.ID);
                        Logger.LogError("Update Content Failed", ex);
                    }
                }

            }

            if (PKey == 0)
            {
                ourData.site_fkey = SiteFKey;
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
                    labelStatus.Text = String.Format("{0} Inserted", Table.Label);


                    PKey = ourData.newsletter_key;

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
                        throw ex;
                    }
                }
                if (ourChanges.Updates.Count > 0)
                {
                    labelStatus.Text = String.Format("{0} Updated", Table.Label);
                }


            }
            catch (Exception ex)
            {
                labelStatus.Text = String.Format("{0} Update Failed", Table.Label);
                Logger.LogError("Update Failed", ex);
            }
        }



        /// <summary>
        /// Take the content, build the newsletter and send it to the emailer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void EmailNewsletter(object sender, EventArgs e)
        {
            SaveEdit(sender, e); //save first then build the email

            Button ourSender = (Button)sender;
            Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));


            string fromemail = ConfigurationManager.AppSettings["NewsletterFromAddress"] ?? String.Empty;
            string fromname = ConfigurationManager.AppSettings["SiteName"] ?? String.Empty;

            Newsletter ourNewsletter = (Newsletter)GetData();
            email newsletter = new email
                                {
                                    fromemail = fromemail,
                                    fromname = fromname,
                                    subject = ourNewsletter.subject ?? String.Empty,
                                    body = ourNewsletter.body ?? String.Empty,
                                    reciprients = GetReciprients(),
                                    unsubscribelink = ConfigurationManager.AppSettings["NewsletterUnSubscribe"]
                                };

            Emailer mailer = new Emailer { Email = newsletter };

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

        private List<MailAddress> GetReciprients()
        {
            List<MailAddress> reciprients = (from r in AdminDC.NewsletterReciprients
                                             where r.active && r.confirmed
                                             select new MailAddress(r.email, r.name)).ToList();
            return reciprients;
        }

        public override void ArchiveData(int iKey)
        {
            Newsletter oldNews = (from n in AdminDC.Newsletters
                                  where n.newsletter_key == iKey
                                  select n).SingleOrDefault();

            DataEntities.Archive.Newsletter archiveNews = new DataEntities.Archive.Newsletter
                                                            {
                                                                body = oldNews.body,
                                                                date_created = oldNews.date_created,
                                                                date_sent = oldNews.date_sent,
                                                                newsletter_key = oldNews.newsletter_key,
                                                                subject = oldNews.subject,
                                                                DBName = AdminDC.Connection.Database
                                                            };

            DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();

            archiveDC.Newsletters.InsertOnSubmit(archiveNews);
            AdminDC.Newsletters.DeleteOnSubmit(oldNews);

            archiveDC.SubmitChanges();
            AdminDC.SubmitChanges();


        }


    }

}