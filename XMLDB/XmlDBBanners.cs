using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mjjames.AdminSystem.DataEntities;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.DataContexts;
using System.Configuration;
using mjjames.AdminSystem.dataentities;
using AjaxControlToolkit;
using System.Web.UI;
using System.Reflection;
using System.Data.Linq;

namespace mjjames.AdminSystem
{
    public class XmlDBbanners : XmlDBBase
    {
        protected override object GetData()
        {
            var banner = new banner();
            if (PKey > 0)
            {
                banner = (from b in AdminDC.banners
                          where b.bannerdid == PKey
                          select b).FirstOrDefault();
            }
            return banner;
        }

        public override void ArchiveData(int iKey)
        {
            banner ban = (from b in AdminDC.banners
                          where b.bannerdid == iKey
                          select b).SingleOrDefault();

            DataEntities.Archive.banner oldBanner = new DataEntities.Archive.banner
            {
                alttext = ban.alttext,
                bannerdid = ban.bannerdid,
                category = ban.category,
                image = ban.image,
                name = ban.name,
                randomness = ban.randomness,
                url = ban.url,
                DBName = AdminDC.Connection.Database
            };

            DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
            archiveDC.banners.InsertOnSubmit(oldBanner);
            archiveDC.SubmitChanges();

            AdminDC.banners.DeleteOnSubmit(ban);
            AdminDC.SubmitChanges();
        }

        protected override void SaveEdit(object sender, EventArgs e)
        {
            Button ourSender = (Button)sender;
            AdminDataContext ourPageDataContext = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            banner ourData = new banner();
            if (PKey > 0)
            {
                ourData = ourPageDataContext.banners.Single(p => p.bannerdid == PKey);
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
                        Logger.LogError("Error Saving Content: " + ourControl.ID, new Exception("Error Saving Content: " + ourControl.ID));
                    }
                }
            }

            if (PKey == 0)
            {
                ourData.site_fkey = SiteFKey;
                ourPageDataContext.banners.InsertOnSubmit(ourData);
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


                    PKey = ourData.bannerdid;

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
                }
                if (ourChanges.Updates.Count > 0)
                {
                    labelStatus.Text = String.Format("{0} Updated", Table.ID);
                }



            }
            catch (Exception ex)
            {
                labelStatus.Text = String.Format("{0} Update Failed", Table.ID);
                Logger.LogError("Failed Update", ex);
            }
        }
    }
}