﻿using System;
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
using mjjames.AdminSystem.Services;
using System.Web;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
    public class XmlDBarticles : XmlDBBase
    {
        #region datasources

        /// <summary>
        /// Gets the data for this editor
        /// </summary>
        /// <returns>a general object that needs casting to the correct type on use</returns>
        protected override object GetData()
        {

            article ourArticle = new article();
            if (PKey > 0)
            {
                ourArticle = (from p in AdminDC.articles
                              where p.article_key == PKey
                              select p).SingleOrDefault();
            }

            return ourArticle;
        }

        public override void ArchiveData(int iKey)
        {
            article a = (from art in AdminDC.articles
                         where art.article_key == iKey
                         select art).SingleOrDefault();

            DataEntities.Archive.article oldArticle = new DataEntities.Archive.article
            {
                active = a.active,
                article_key = a.article_key,
                body = a.body,
                end_date = a.end_date,
                include_in_feed = a.include_in_feed,
                shortdescription = a.shortdescription,
                showonhome = a.showonhome,
                sortorder = a.sortorder,
                start_date = a.start_date,
                thumbnailimage = a.thumbnailimage,
                title = a.title,
                url = a.url,
                virtualurl = a.virtualurl,
                DBName = AdminDC.Connection.Database
            };

            DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
            archiveDC.articles.InsertOnSubmit(oldArticle);
            archiveDC.SubmitChanges();

            AdminDC.articles.DeleteOnSubmit(a);
            AdminDC.SubmitChanges();
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
            article ourData = new article();
            if (PKey > 0)
            {
                ourData = ourPageDataContext.articles.Single(p => p.article_key == PKey);
            }

            foreach (AdminTab tab in Table.Tabs)
            {
                WebControl ourTab = (WebControl)FindControlRecursive(ourSender.Page, tab.ID);
                if (ourTab == null) continue;
                foreach (AdminField field in tab.Fields)
                {
                    Control ourControl = ourTab.FindControl("control" + field.ID);

                    //if we cant find a control for that ID or its of type photogallery skip it
                    if (ourControl == null || field.Type.Equals("photogallery", StringComparison.InvariantCultureIgnoreCase)) continue;

                    PropertyInfo ourProperty = ourData.GetType().GetProperty(field.ID);
                    if (ourProperty != null)
                    {
                        Logger.LogInformation("Saving Content In: " + ourControl.ID);
                        ourProperty.SetValue(ourData, GetDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
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
                generateURL<article>(ourData, AdminDC);
                ourPageDataContext.articles.InsertOnSubmit(ourData);
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
                    AuditLogService.LogItem("Articles", 
                        Models.AuditEvent.Created,
                        HttpContext.Current.User.Identity.Name,
                        ourData.title);

                    PKey = ourData.article_key;

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
                        Exception ex =
                            new Exception(String.Format("{0} doesn't contain a hidden control called {1}", Table.ID, TablePrimaryKeyField));
                        Logger.LogError("Unknown Field", ex);
                        throw ex;
                    }

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["twitterPublishArticles"]) && ourData.active)
                    {
                        TwitterPublisher tp = new TwitterPublisher(ConfigurationManager.AppSettings["twitterConsumerKey"],
                                                                    ConfigurationManager.AppSettings["twitterConsumerSecret"],
                                                                    ConfigurationManager.AppSettings["twitterAuthenticationToken"],
                                                                    ConfigurationManager.AppSettings["twitterAuthenticationTokenSecret"]);

                        ourPageDataContext.Refresh(RefreshMode.OverwriteCurrentValues, ourData);
                        //todo: make this use the site details and not use the config
                        string url = String.Format("http://{0}{1}", ConfigurationManager.AppSettings["DomainName"], ourData.url);

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
                    labelStatus.Text = String.Format("{0} Updated", Table.Label);
                    AuditLogService.LogItem("Articles",
                        Models.AuditEvent.Updated,
                        HttpContext.Current.User.Identity.Name,
                        ourData.title);
                }



            }
            catch (Exception ex)
            {
                labelStatus.Text = String.Format("{0} Update Failed", Table.Label);
                Logger.LogError("Failed Update", ex);
            }
        }

        #endregion

        /// <summary>
        /// Checks to see if we already have data with this url, if it does makes the url unique
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="ourData">data</param>
        /// <param name="dataContext">db context</param>
        override protected void generateURL<T>(T ourData, AdminDataContext dataContext)
        {
            var data = ourData as article;

            string prefix = ConfigurationManager.AppSettings["urlprefixArticle"] ?? String.Empty;
            prefix = prefix.Replace("[*year]*", data.start_date.GetValueOrDefault(DateTime.Now).Year.ToString());
            prefix = prefix.Replace("[*month]*", data.start_date.GetValueOrDefault(DateTime.Now).Month.ToString());
            data.url = String.Format("{0}{1}", prefix, SQLHelpers.URLSafe(data.title));

            //pull out all of our sibling pages and then try to find pages that start with our page-url 
            var siblings = (from p in dataContext.articles
                            where p.site_fkey == data.site_fkey
                            select p.url).ToArray();

            //if we don't have an item that matches our url exactly there's no need to do anything else
            if (!siblings.Contains(data.url))
            {
                return;
            }

            var existingUrls = siblings.Where(u => u.StartsWith(data.url));

            //now if we have siblings with the same starting url suffix our url with a numerical value, i.e the total existing sibling pages + 1
            if (existingUrls.Count() > 0)
            {
                data.url += "-" + existingUrls.Count();
            }
        }


    }

}