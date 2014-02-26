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
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.DataContexts;
using System.Collections.Generic;
using mjjames.AdminSystem.Repositories;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
    public class XmlDBmedia : XmlDBBase
    {
        #region datasources

        /// <summary>
        /// Gets the data for this editor
        /// </summary>
        /// <returns>a general object that needs casting to the correct type on use</returns>
        protected override object GetData()
        {

            media ourMedia = new media();
            if (PKey > 0)
            {
                ourMedia = (from p in AdminDC.medias
                            where p.media_key == PKey
                            select p).SingleOrDefault();
            }

            return ourMedia;
        }




        #endregion

        public override void ArchiveData(int iKey)
        {
            media oldMedia = (from m in AdminDC.medias
                              where m.media_key == iKey
                              select m).SingleOrDefault();

            DataEntities.Archive.media archiveMedia = new DataEntities.Archive.media
                                                        {
                                                            active = oldMedia.active,
                                                            description = oldMedia.description,
                                                            filename = oldMedia.filename,
                                                            media_key = oldMedia.media_key,
                                                            mediatype_lookup = oldMedia.mediatype_lookup,
                                                            title = oldMedia.title,
                                                            DBName = AdminDC.Connection.Database
                                                        };

            DataContexts.Archive.archiveDataContext archiveDC = new DataContexts.Archive.archiveDataContext();

            archiveDC.medias.InsertOnSubmit(archiveMedia);
            AdminDC.medias.DeleteOnSubmit(oldMedia);

            archiveDC.SubmitChanges();
            AdminDC.SubmitChanges();
        }

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
            media ourData = new media();
            if (PKey > 0)
            {
                ourData = ourPageDataContext.medias.Single(p => p.media_key == PKey);
            }
            var keyvalues = new List<KeyValueData>();
            foreach (AdminTab tab in Table.Tabs)
            {
                WebControl ourTab = (WebControl)FindControlRecursive(ourSender.Page, tab.ID);
                if (ourTab == null) continue;
                foreach (AdminField field in tab.Fields)
                {
                    Control ourControl = ourTab.FindControl("control" + field.ID);

                    //if we cant find a control for that ID or its of type photogallery skip it
                    if (ourControl == null || field.Type.Equals("photogallery", StringComparison.InvariantCultureIgnoreCase)) continue;

                    //if we are a key value get our data out and stash it for later
                    if (field.Attributes.ContainsKey("keyvalue"))
                    {
                        keyvalues.Add(new KeyValueData
                        {
                            LinkKey = PKey,
                            Value = GetDataValue(ourControl, field.Type, typeof(String)) as String,
                            LinkTypeID = "medialookup",
                            LookupID = field.Attributes["lookupid"]
                        });
                        continue;
                    }

                    PropertyInfo ourProperty = ourData.GetType().GetProperty(field.ID);
                    if (ourProperty != null)
                    {
                        Logger.LogInformation("Saving Content In: " + ourControl.ID);
                        ourProperty.SetValue(ourData, GetDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
                    }
                    else
                    {
                        Logger.LogError("Save Content Failed", new Exception("Error Saving Content: " + ourControl.ID));
                    }
                }
            }

            if (PKey == 0)
            {
                ourData.site_fkey = SiteFKey;
                ourPageDataContext.medias.InsertOnSubmit(ourData);
            }

            Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
            try
            {
                ChangeSet ourChanges = ourPageDataContext.GetChangeSet();

                labelStatus.Text = "Nothing to Save";


                ourPageDataContext.SubmitChanges();
                var updateType = UpdateType.None;
                if (ourChanges.Inserts.Count > 0)
                {
                    updateType = UpdateType.Inserted;


                    PKey = ourData.media_key;
                    //when we do an insert update any keyvalues we have to have the correct primary key
                    keyvalues = keyvalues.Select(kv => new KeyValueData()
                    {
                        LinkKey = PKey,
                        LinkTypeID = kv.LinkTypeID,
                        LookupID = kv.LookupID,
                        Value = kv.Value
                    }).ToList();

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
                    }
                }
                if (ourChanges.Updates.Count > 0)
                {
                    updateType = UpdateType.Updated;
                }
                if (keyvalues.Count > 0)
                {
                    var keyValueRepository = new KeyValueRepository();
                    Logger.LogInformation("Updating Key Values");
                    keyValueRepository.UpdateKeyValues(keyvalues);
                    if (updateType.Equals(UpdateType.None))
                    {
                        updateType = UpdateType.Updated;
                    }
                }

                switch (updateType)
                {
                    case UpdateType.None:
                        labelStatus.Text = "Nothing to Save";
                        break;
                    case UpdateType.Inserted:
                        labelStatus.Text = String.Format("{0} Inserted", Table.Label);
                        break;
                    case UpdateType.Updated:
                        labelStatus.Text = String.Format("{0} Updated", Table.Label);
                        break;
                }

            }
            catch (Exception ex)
            {
                labelStatus.Text = String.Format("{0} Update Failed", Table.Label);
                Logger.LogError("Update Error", ex);
            }
        }

        protected override string ApplyDataFilters(string filterName)
        {
            var filter = "";
            switch (filterName.ToLower())
            {
                case "podcasts":
                    var podcastKey = from l in AdminDC.lookups
                                     where l.lookup_id == "podcast"
                                        && l.type == "media_type"
                                     select l.lookup_key;
                    filter = String.Format("[mediatype_lookup] = {0}", podcastKey.FirstOrDefault());
                    break;
                case "featuredpodcasts":
                    var featuredKey = from l in AdminDC.lookups
                                      where l.lookup_id == "featuredpodcast"
                                         && l.type == "media_type"
                                      select l.lookup_key;
                    filter = String.Format("[mediatype_lookup] = {0}", featuredKey.FirstOrDefault());
                    break;
                case "downloads":
                    var downloadKey = from l in AdminDC.lookups
                                      where l.lookup_id == "download"
                                         && l.type == "media_type"
                                      select l.lookup_key;
                    filter = String.Format("[mediatype_lookup] = {0}", downloadKey.FirstOrDefault());
                    break;
                default:
                    filter = "";
                    break;
            }
            return filter;
        }

        #endregion


    }

}