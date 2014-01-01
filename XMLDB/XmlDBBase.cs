using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using AjaxControlToolkit;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataContexts;

namespace mjjames.AdminSystem
{
    public class XmlDBBase
    {
        protected string InternalTableName;
        protected string XMLFilePath;
        protected XDocument XML;
        protected AdminDataContext AdminDC;
        protected int PKey;
        protected int FKey;
        protected AdminTable Table;
        protected string Connectionstring = String.Empty;
        protected readonly Logger Logger = new Logger("XMLDB");
        protected int SiteFKey;
        protected bool MultiTenancyTableEnabled = true;
        protected readonly bool MultiTenancyEnabled;
        //private string _tableName;
        /// <summary>
        /// Provide a ConnectionString for the DataSources
        /// </summary>
        public string ConnectionString { set { Connectionstring = value; } get { return Connectionstring; } }

        /// <summary>
        /// sets the table to use within the xml
        /// </summary>
        public string TableName { set { InternalTableName = value; BuildAdminTable(); } get { return InternalTableName; } }

        /// <summary>
        /// for overridding the xml file 
        /// </summary>
        public string CustomXMLFile { set { XMLFilePath = value; } get { return XMLFilePath; } }

        /// <summary>
        /// provide the tables primary key for databinding
        /// </summary>
        public int PrimaryKey { set { PKey = value; } get { return PKey; } }

        /// <summary>
        /// provides the tables foreign key for linking
        /// </summary>
        public int ForeignKey { set { FKey = value; } get { return FKey; } }

        public int SiteKey { set { SiteFKey = value; } get { return SiteFKey; } }

        public string TableLabel { get { return Table != null ? Table.Label : String.Empty; } }

        public string TablePrimaryKeyField { get { return Table != null ? Table.Defaults.FirstOrDefault(t => t.Attributes.ContainsKey("primarykey")).ID : String.Empty; } }


        public List<AdminField> TableDefaults
        {
            get
            {
                return Table != null ? Table.Defaults : null;
            }
        }

        public bool TableQuickEdit { get { return Table.QuickEdit; } }

        public XmlDBBase()
            : this(true)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public XmlDBBase(bool multiTenancyTableEnabled)
        {
            MultiTenancyTableEnabled = multiTenancyTableEnabled;

            XMLFilePath = ConfigurationManager.AppSettings["adminConfigXML"];
            try
            {
                if (XMLFilePath == null)
                {
                    var exception = new Exception("Error: No Admin Config XML Specified");
                    Logger.LogError("XMLDB Error", exception);
                    throw exception;
                }
                var sFilePath = HttpContext.Current.Server.MapPath(XMLFilePath);

                XML = XDocument.Load(sFilePath);
                Logger.LogInformation("adminConfigxml: " + sFilePath);
                AdminDC = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            }
            catch (Exception e)
            {
                Logger.LogError("XMLDB Error", e);
                HttpContext.Current.Response.Write(string.Format("<h1>{0}</h1><p>{1}</p><p>{2}</p>", "XML DB Error", e.Message, e.InnerException));
                HttpContext.Current.Response.End();
            }

            MultiTenancyEnabled = ConfigurationManager.AppSettings["EnableMultiTenancy"] != null && ConfigurationManager.AppSettings["EnableMultiTenancy"].Equals("true", StringComparison.CurrentCultureIgnoreCase);

        }

        /// <summary>
        /// builds the admin table object from the xml file 
        /// </summary>
        private void BuildAdminTable()
        {
            var xmlQuery = from table in XML.Descendants("table")
                           where table.Attribute("id").Value == InternalTableName
                           select new AdminTable
                           {
                               EmailButton = XmlConvert.ToBoolean(table.Attribute("emailbutton") != null ? table.Attribute("emailbutton").Value : "false"),
                               QuickEdit = XmlConvert.ToBoolean(table.Attribute("quickedit") != null ? table.Attribute("quickedit").Value : "false"),
                               ID = table.Attribute("id").Value,
                               Name = table.Attribute("basetype") != null ? table.Attribute("basetype").Value : table.Attribute("id").Value,
                               Label = table.Attribute("label").Value,
                               Filter = table.Attribute("filter") != null ? table.Attribute("filter").Value : "",
                               Defaults = (from fields in table.Element("defaults").Elements("field")
                                           select new AdminField
                                           {
                                               ID = fields.Attribute("id").Value,
                                               Label = fields.Attribute("label").Value,
                                               Type = fields.Attribute("type").Value,
                                               Attributes = table.Element("defaults")
                                                                   .Elements("field")
                                                                   .Single(field => field.Attribute("id").Value == fields.Attribute("id").Value)
                                                                   .Attributes().ToDictionary(x => x.Name.LocalName, x => x.Value)
                                           }).ToList(),
                               Tabs = (from tabs in table.Element("tabs").Elements("tab")
                                       select new AdminTab
                                       {
                                           ID = tabs.Attribute("id").Value,
                                           Label = tabs.Attribute("label").Value,
                                           Fields = (from fields in tabs.Elements("field")
                                                     select new AdminField
                                                     {

                                                         ID = fields.Attribute("id").Value,
                                                         Label = table.Element("defaults")
                                                                           .Elements("field")
                                                                           .Single(field => field.Attribute("id").Value == fields.Attribute("id").Value)
                                                                           .Attribute("label")
                                                                           .Value,
                                                         Type = table.Element("defaults")
                                                                           .Elements("field")
                                                                           .Single(field => field.Attribute("id").Value == fields.Attribute("id").Value)
                                                                           .Attribute("type")
                                                                           .Value,
                                                         Attributes = table.Element("defaults")
                                                                           .Elements("field")
                                                                           .Single(field => field.Attribute("id").Value == fields.Attribute("id").Value)
                                                                           .Attributes().ToDictionary(x => x.Name.LocalName, x => x.Value)

                                                     }).ToList()
                                       }).ToList()

                           };
            Table = xmlQuery.FirstOrDefault();
        }


        #region DBEditor methods
        /// <summary>
        /// Renders a WebControl for the field specified
        /// </summary>
        /// <param name="field">XMLNode to render a control for</param>
        /// <param name="ourPage"></param>
        /// <returns>PlaceHolder containing rendered control</returns>
        /// TODO need to cast from object to a type
        private PlaceHolder RenderControl(AdminField field, object ourPage)
        {

            Logger.LogDebug("Rendering Control: " + field.Type);
            var renderedControl = new PlaceHolder();
            var bRenderControl = true;

            var ourLabel = new Label();
            var ourContainer = new WebControl(HtmlTextWriterTag.Div) { CssClass = "control-group" };

            ourLabel.Text = field.Label;
            ourLabel.CssClass = "control-label " + field.Type.ToLower();
            ourLabel.ID = "label" + field.ID;
            ourLabel.AssociatedControlID = "control" + field.ID;

            ourContainer.Controls.Add(ourLabel);
            var controlContainer = new WebControl(HtmlTextWriterTag.Div) { CssClass = "controls" };
            ourContainer.Controls.Add(controlContainer);

            var controlParams = new[] { field, ourPage };
            try
            {
                //Create CultureInfo and TextInfo classes to use ToTitleCase method
                var cultureInfo = Thread.CurrentThread.CurrentCulture;
                var textInfo = cultureInfo.TextInfo;
                var controlName = String.Format("mjjames.AdminSystem.dataControls.{0}Control", textInfo.ToTitleCase(field.Type));
                var ourType = Activator.CreateInstance("Synthesis", controlName);

                if (ourType != null)
                {
                    var controlHandle = ourType.Unwrap();
                    controlHandle.GetType().GetProperty("PKey").SetValue(controlHandle, PKey, null);
                    //see if the control has a site key property
                    var siteKeyProperty = controlHandle.GetType().GetProperty("SiteKey");
                    //if it does set it
                    if (siteKeyProperty != null)
                    {
                        siteKeyProperty.SetValue(controlHandle, SiteKey, null);
                    }
                    var ourControl = (Control)controlHandle.GetType().GetMethod("GenerateControl").Invoke(controlHandle, controlParams);
                    //we don't want to use the horizontal formatting for hidden fields
                    if (ourControl is HiddenField || ourControl is FredCK.FCKeditorV2.FCKeditor)
                    {
                        ourContainer.CssClass = "";
                        controlContainer.CssClass = "";
                    }
                    if (ourControl is FredCK.FCKeditorV2.FCKeditor)
                    {
                        ourLabel.CssClass = "";
                    }
                    if (ourControl is CheckBox)
                    {
                        ourLabel.CssClass = "control-label";
                    }
                    if (ourControl is TextBox)
                    {
                        ((TextBox)ourControl).CssClass += " input-xlarge";
                    }

                    controlContainer.Controls.Add(ourControl);

                }
                else
                {
                    Logger.LogError("RenderControl Failed ", new TypeLoadException("Unknown ControlType: " + field.Type));

                }
            }
            catch (TypeLoadException ex)
            {
                Logger.LogError("RenderControl Failed ", new Exception("Unknown ControlType: " + field.Type, ex));
                bRenderControl = false;
            }

            if (bRenderControl)
            {
                renderedControl.Controls.Add(ourContainer);
            }

            return renderedControl;
        }

        /// <summary>
        /// generate the controls for the tab / snippet
        /// </summary>
        /// <returns>placeholder containing controls</returns>
        public WebControl GenerateControls(List<AdminField> fields)
        {
            var phControls = new WebControl(HtmlTextWriterTag.Div) { CssClass = "form-horizontal" };
            var ourData = GetData();

            foreach (var field in fields)
            {
                phControls.Controls.Add(RenderControl(field, ourData));
            }

            return phControls;
        }


        /// <summary>
        /// generate the tabs
        /// </summary>
        /// <returns>placeholder containing the tabs and their controls</returns>
        public PlaceHolder GenerateTabs()
        {

            var phTabs = new PlaceHolder { ID = "tabsPlaceholder" };
            var tabContainer = new WebControl(HtmlTextWriterTag.Div)
            {
                CssClass = "tabbable"
            };
            var tabsNavigation = new WebControl(HtmlTextWriterTag.Ul)
            {
                CssClass = "nav nav-tabs"
            };
            var tabContent = new WebControl(HtmlTextWriterTag.Div)
            {
                CssClass = "tab-content"
            };
            tabContainer.Controls.Add(tabsNavigation);
            tabContainer.Controls.Add(tabContent);

            if (Table != null)
            {
                var firstTab = true;
                foreach (AdminTab tab in Table.Tabs)
                {
                    var tabPane = new WebControl(HtmlTextWriterTag.Div)
                    {
                        CssClass = "tab-pane",
                        ID = tab.ID,
                        ClientIDMode = ClientIDMode.Static
                    };

                    var tabNavItem = new WebControl(HtmlTextWriterTag.Li);
                    var tabNavItemLink = new HyperLink()
                    {
                        Text = tab.Label,
                        NavigateUrl = "#" + tabPane.ClientID,
                    };
                    tabNavItemLink.Attributes.Add("data-toggle", "tab");

                    if (firstTab)
                    {
                        tabNavItem.CssClass = "active";
                        firstTab = false;
                        tabPane.CssClass += " active";
                    }

                    tabNavItem.Controls.Add(tabNavItemLink);
                    tabsNavigation.Controls.Add(tabNavItem);

                    Logger.LogDebug("Rendering Tab:" + tab.ID);
                    tabPane.Controls.Add(GenerateControls(tab.Fields));

                    tabContent.Controls.Add(tabPane);
                }
            }
            phTabs.Controls.Add(tabContainer);
            return phTabs;
        }

        /// <summary>
        /// generates the table DBEditor page includes buttons
        /// </summary>
        /// <returns>PlaceHolder containing page</returns>
        public PlaceHolder GeneratePage()
        {
            var ourPage = new PlaceHolder { ID = "pagePlaceHolder" };
            if (Table != null)
            {

                ourPage.Controls.Add(GenerateTabs());
                var formactions = new WebControl(HtmlTextWriterTag.Div)
                {
                    CssClass = "form-actions"
                };

                var buttonContainer = new WebControl(HtmlTextWriterTag.Div)
                {
                    CssClass = "pull-right btn-group"
                };

                var saveButton = new Button { Text = "Save", CommandName = "SaveEdit", CssClass = "btn btn-success" };


                saveButton.Click += SaveEdit;

                if (PrimaryKey == 0)
                {
                    saveButton.Click += RedirectToEdit; //this should only work for an insert
                }
                var cancelButton = new Button { Text = "Cancel", CommandName = "CancelEdit", CssClass = "btn btn-inverse" };
                cancelButton.Click += CancelEdit;

                if (Table.EmailButton)
                {
                    var emailButton = new Button { Text = "Email", CommandName = "emailButton", CssClass = "btn btn-info" };
                    emailButton.Click += SaveEdit;
                    emailButton.Click += EmailNewsletter;
                    buttonContainer.Controls.Add(emailButton);
                }

                buttonContainer.Controls.Add(saveButton);

                if (PKey > 0) //this is optional
                {
                    var deleteButton = new Button { Text = "Delete", CommandName = "DeleteEdit", CssClass = "buttonDelete btn btn-danger" };
                    deleteButton.Click += DeleteEdit;
                    var csm = ((Page)HttpContext.Current.CurrentHandler).Page.ClientScript;
                    csm.RegisterStartupScript(typeof(XmlDBBase), "DeleteScript",
                                                            "<script type=\"text/javascript\"> $(\".buttonDelete\").click(function(){ var bDelete = confirm(\"Are You Sure You Want To Delete This Item?\");	return bDelete;	}); </script>");

                    buttonContainer.Controls.Add(deleteButton);
                }


                buttonContainer.Controls.Add(cancelButton);
                formactions.Controls.Add(buttonContainer);
                ourPage.Controls.Add(formactions);

            }
            else
            {
                var errorMessage = new LiteralControl("<h1>DBEditor Could Not Be Loaded</h1><p> Please Try Again</p>");
                Logger.LogError("XMLDB ERROR:", new Exception("DBEditor Couldn't Be Loaded - Missing Table"));
                ourPage.Controls.Add(errorMessage);
            }
            return ourPage;
        }


        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exits
        /// </summary>
        /// <param name="root"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        public static Control FindControlRecursive(Control root, string id)
        {

            if (root.ID == id)
                return root;

            foreach (Control ctl in root.Controls)
            {
                using (Control foundCtl = FindControlRecursive(ctl, id))
                {
                    if (foundCtl != null)
                        return foundCtl;
                }
            }
            return null;
        }
        /// <summary>
        /// Takes a control, the field type and the type we want and returns the value as an object
        /// </summary>
        /// <param name="ourControl">HTMLControl we want to get a value from</param>
        /// <param name="sFieldType">The Type of Control</param>
        /// <param name="ourType">The datatype we need</param>
        /// <returns>An object with our Value in</returns>
        protected static object GetDataValue(Control ourControl, string sFieldType, Type ourType)
        {
            var dataParams = new object[] { ourControl, ourType };
            //Create CultureInfo and TextInfo classes to use ToTitleCase method
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            var controlName = String.Format("mjjames.AdminSystem.dataControls.{0}Control", textInfo.ToTitleCase(sFieldType));
            var ourDType = Activator.CreateInstance("Synthesis", controlName);

            object dataValue = null;
            if (ourDType != null)
            {
                var controlHandle = ourDType.Unwrap();
                dataValue = controlHandle.GetType().GetMethod("GetDataValue").Invoke(null, dataParams);
            }
            else
            {
                var logger = new Logger("XMLDB");
                logger.LogError("Base GetDataValue Failed To Load Control", new Exception(String.Format("Unknown Control - mjjames.AdminSystem.dataControls.{0}Control", sFieldType.ToLower())));
            }
            return dataValue;
        }

        #endregion DBEditor methods

        #region datasources

        /// <summary>
        /// Gets the data for this editor
        /// </summary>
        /// <returns>a general object that needs casting to the correct type on use</returns>
        protected virtual object GetData()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Makes A LinqDataSource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PageLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetData();
        }


        /// <summary>
        /// Builds and returns a SqlDataSource for the table
        /// </summary>
        /// <returns></returns>
        public SqlDataSource DataSource(bool listingmode, bool select, bool update, bool insert, bool delete)
        {
            var sds = new SqlDataSource { ConnectionString = Connectionstring };

            if (select)
            {
                sds.SelectCommand = BuildSelectCommand(listingmode);
            }
            if (delete)
            {
                sds.DeleteCommand = BuildDeleteCommand();
            }
            if (insert)
            {
                sds.InsertCommand = BuildInsertCommand();
            }
            if (update)
            {
                sds.UpdateCommand = BuildUpdateCommand();
            }

            sds.DataSourceMode = SqlDataSourceMode.DataSet;

            return sds;
        }


        /// <summary>
        /// Builds a Select Command
        /// </summary>
        /// <param name="bListingMode">Whether to only select items with a list</param>
        /// <returns>String for Select Command</returns>
        protected string BuildSelectCommand(bool bListingMode)
        {
            var selectCommand = String.Empty;
            var selectparams = new List<string>();

            if (Table != null)
            {
                var listfields = Table.Defaults;

                if (bListingMode)
                {
                    listfields = Table.Defaults.FindAll(t => t.Attributes.ContainsKey("list"));
                }

                selectparams.AddRange(listfields.Select(field => "[" + field.ID + "]"));

                selectCommand = String.Format("SELECT {0} FROM [{1}]", String.Join(" , ", selectparams.ToArray()), Table.Name);

                var filter = "";
                var bMultiWhere = false;

                if (MultiTenancyEnabled && MultiTenancyTableEnabled)
                {
                    filter = " [site_fkey] = " + SiteKey;
                    bMultiWhere = true;
                }

                if (listfields.Find(f => f.Attributes.ContainsKey("listfilter")) != null)
                {


                    foreach (var af in listfields.FindAll(lf => lf.Attributes.ContainsKey("listfilter")))
                    {
                        if (bMultiWhere)
                        {
                            filter += " AND ";
                        }

                        filter += String.Format(" [{0}] = @{1} ", af.ID, af.ID);
                        bMultiWhere = true;
                    }

                }

                if (!String.IsNullOrEmpty(Table.Filter))
                {
                    filter += ApplyDataFilters(Table.Filter);
                }

                if (!String.IsNullOrEmpty(filter))
                {
                    selectCommand += "WHERE " + filter;
                }

                var sortAttributes = Table.Defaults.Where(d => d.Attributes.Any(a => a.Key == "sort"));
                selectCommand += " ORDER BY ";
                foreach (var sortItem in sortAttributes)
                {
                    selectCommand += string.Format("[{0}] {1} ,", sortItem.ID, sortItem.Attributes["sort"]);
                }

                var primaryKey = Table.Defaults.First(d => d.Attributes.Any(a => a.Key == "primarykey"));
                selectCommand += string.Format("[{0}] desc", primaryKey.ID);

            }
            return selectCommand;
        }

        protected virtual string ApplyDataFilters(string filterName)
        {
            throw new NotImplementedException();
        }

        private string BuildDeleteCommand()
        {
            var deleteCommand = String.Empty;
            if (Table != null)
            {
                deleteCommand = String.Format("DELETE FROM [{0}] WHERE [{1}] = @{2}", Table.ID, TablePrimaryKeyField, TablePrimaryKeyField);
            }
            return deleteCommand;
        }

        private string BuildInsertCommand()
        {
            var insertCommand = String.Empty;
            if (Table != null)
            {

            }
            return insertCommand;
        }

        private string BuildUpdateCommand()
        {
            var updateCommand = String.Empty;
            if (Table != null)
            {
                ///TODO need to write an update command still
            }
            return updateCommand;
        }

        #endregion

        #region button events

        /// <summary>
        /// Handles a Cancel CLick -> return to previous page or app root if none found
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelEdit(object sender, EventArgs e)
        {
            var ourSender = (Button)sender;
            var prevPage = ourSender.Page.PreviousPage;
            ourSender.Page.Response.Redirect(prevPage == null ? "~/" : prevPage.Request.RawUrl);
        }


        /// <summary>
        /// save away our data / insert - abstract
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SaveEdit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// After a successful save we need to redirect to the edit page, this is due to the photogallery control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RedirectToEdit(object sender, EventArgs e)
        {
            //having a primary key at this point means the insert was a success
            if (PKey <= 0) return;
            var url = String.Format("{0}&{1}={2}", HttpContext.Current.Request.Url, TablePrimaryKeyField, PrimaryKey);
            HttpContext.Current.Response.Redirect(url, true);
        }

        /// <summary>
        /// email's the newsletter - abstract
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EmailNewsletter(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Archive the Data - abstract
        /// </summary>
        public virtual void ArchiveData(int iKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// remove our data from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteEdit(object sender, EventArgs e)
        {

            var ourSender = (Button)sender;
            var ourPageDataContext = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);

            var labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
            try
            {
                ArchiveData(PKey);

                string strDelete = String.Format("DELETE FROM [{0}] WHERE [{1}] = {2}", Table.ID, TablePrimaryKeyField, PKey);

                Logger.LogDebug("Deleting Item: " + strDelete);

                ourPageDataContext.ExecuteQuery<object>(strDelete);

                labelStatus.Text = String.Format("{0} Removed", Table.ID);
                // if we delete a page we need to reset the sitemap
                if (Table.ID.Equals("pages", StringComparison.InvariantCultureIgnoreCase))
                {
                    GenericFunctions.ResetSiteMap();
                }

                var prevPage = ourSender.Page.PreviousPage;
                ourSender.Page.Response.Redirect(prevPage == null ? "~/" : prevPage.Request.RawUrl, false);
            }
            catch (Exception ex)
            {
                labelStatus.Text = String.Format("{0} Removal Failed", Table.ID);
                Logger.LogError("Delete Failed", ex);

            }
        }

        /// <summary>
        /// Generates a URL for the type, check for duplicates and ensure a unique url is generated
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="ourData">Data</param>
        /// <param name="dataContext">AdminDataContext</param>
        protected virtual void generateURL<T>(T ourData, AdminDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal enum UpdateType
        {
            None,
            Inserted,
            Updated
        }
    }

}