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
    public class XmlDBlookup : XmlDBBase
    {
        public XmlDBlookup()
        {
            this.MultiTenancyTableEnabled = false;
        }
        protected override object GetData()
        {
            var lookup = new lookup();
            if (PKey > 0)
            {
                lookup = (from l in AdminDC.lookups
                          where l.lookup_key == PKey
                          select l).FirstOrDefault();
            }
            return lookup;
        }

        public override void ArchiveData(int iKey)
        {
            var lookup = (from l in AdminDC.lookups
                          where l.lookup_key == iKey
                          select l).FirstOrDefault();

            DataEntities.Archive.lookup oldLookup = new DataEntities.Archive.lookup
            {
                active = Convert.ToBoolean(lookup.active),
                lookup_id = lookup.lookup_id,
                lookup_key = lookup.lookup_key,
                title = lookup.title,
                type = lookup.type,
                value = lookup.value
            };

            DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();
            archiveDC.lookups.InsertOnSubmit(oldLookup);
            archiveDC.SubmitChanges();

            AdminDC.lookups.DeleteOnSubmit(lookup);
            AdminDC.SubmitChanges();
        }

        protected override void SaveEdit(object sender, EventArgs e)
        {
            Button ourSender = (Button)sender;
            AdminDataContext ourPageDataContext = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            lookup ourData = new lookup();
            if (PKey > 0)
            {
                ourData = ourPageDataContext.lookups.Single(p => p.lookup_key == PKey);
            }

            foreach (AdminTab tab in Table.Tabs)
            {
                WebControl ourTab = (WebControl)FindControlRecursive(ourSender.Page, tab.ID);
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
                ourPageDataContext.lookups.InsertOnSubmit(ourData);
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


                    PKey = ourData.lookup_key;

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