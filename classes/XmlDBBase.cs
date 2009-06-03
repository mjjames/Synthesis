using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using AjaxControlToolkit;
using mjjames.AdminSystem.dataentities;
using mjjames.core;
using mjjames.core.dataentites;
using mjjames.AdminSystem.DataContexts;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBBase
	{
		protected string sTableName;
		protected string sXMLFile;
		protected XDocument xdXML;
		protected adminDataContext adminDC;
		protected int _iPKey;
		protected int _iFKey;
		protected AdminTable atTable;
		protected string _connectionstring = String.Empty;
		

		/// <summary>
		/// Provide a ConnectionString for the DataSources
		/// </summary>
		public string ConnectionString { set { _connectionstring = value; } get { return _connectionstring; } }

		/// <summary>
		/// sets the table to use within the xml
		/// </summary>
		public string TableName { set { sTableName = value; buildAdminTable();} get { return sTableName; } }

		/// <summary>
		/// for overridding the xml file 
		/// </summary>
		public string CustomXMLFile { set { sXMLFile = value; } get { return sXMLFile; } }

		/// <summary>
		/// provide the tables primary key for databinding
		/// </summary>
		public int PrimaryKey { set { _iPKey = value; } get { return _iPKey; } }

		/// <summary>
		/// provides the tables foreign key for linking
		/// </summary>
		public int ForeignKey { set { _iFKey = value; } get { return _iFKey; } }

		public string TableLabel { get { return atTable != null ? atTable.Label : String.Empty; } }

		public string TablePrimaryKeyField { get { return atTable != null ? atTable.Defaults.FirstOrDefault(t => t.Attributes.ContainsKey("primarykey")).ID : String.Empty; } }


		public List<AdminField> TableDefaults
		{
			get
			{
				return atTable != null ? atTable.Defaults : null ;
			}
		}

		public bool TableQuickEdit { get { return atTable.bQuickEdit; } }
		
		/// <summary>
		/// constructor
		/// </summary>
		public XmlDBBase()
		{
			sXMLFile = (string)ConfigurationManager.AppSettings["adminConfigXML"];
			try
			{
				if (sXMLFile == null)
				{
					throw new Exception("Error: No Admin Config XML Specified");
				}
				string sFilePath = HttpContext.Current.Server.MapPath(sXMLFile);

				xdXML = XDocument.Load(sFilePath);
				HttpContext.Current.Trace.Write("adminConfigxml: " + sFilePath);
				adminDC = new adminDataContext();
			}
			catch (Exception e)
			{
				HttpContext.Current.Response.Write(string.Format("<h1>{0}</h1><p>{1}</p><p>{2}</p>", "XML DB Error", e.Message, e.InnerException));
				HttpContext.Current.Response.End();
			}
			
		}

		/// <summary>
		/// builds the admin table object from the xml file 
		/// </summary>
		private void buildAdminTable()
		{ 
			var xmlQuery = from table in xdXML.Descendants("table")
						   where table.Attribute("id").Value == sTableName
						   select new AdminTable
						   {
							   bEmailButton = XmlConvert.ToBoolean(table.Attribute("emailbutton") != null ? table.Attribute("emailbutton").Value : "false"),
							   bQuickEdit = XmlConvert.ToBoolean(table.Attribute("quickedit") != null ? table.Attribute("quickedit").Value : "false"),
							   ID = table.Attribute("id").Value ,
							   Label = table.Attribute("label").Value,
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
			atTable = xmlQuery.FirstOrDefault();
		}


		#region dbeditor methods
		/// <summary>
		/// Renders a WebControl for the field specified
		/// </summary>
		/// <param name="field">XMLNode to render a control for</param>
		/// <param name="ourPage"></param>
		/// <returns>PlaceHolder containing rendered control</returns>
		/// TODO need to cast from object to a type
		private PlaceHolder RenderControl(AdminField field, object ourPage)
		{
			HttpContext.Current.Trace.Write("Rendering Control: " + field.Type);
			PlaceHolder renderedControl = new PlaceHolder();
			bool bRenderControl = true;
			
			ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);
					
			Label ourLabel = new Label();
			WebControl ourContainer = new WebControl(HtmlTextWriterTag.Div);

			ourContainer.CssClass = "row";

			ourLabel.Text = field.Label;
			ourLabel.CssClass = "label " + field.Type.ToLower();
			ourLabel.ID = "label" + field.ID;
			ourLabel.AssociatedControlID = "control" + field.ID;

			ourContainer.Controls.Add(ourLabel);

			object[] controlParams = new object[] { field, ourPage };
			try
			{
				object ourType = Activator.CreateInstance(null, "mjjames.AdminSystem.dataControls." + field.Type.ToLower() + "Control").Unwrap();
				ourType.GetType().GetProperty("iPKey").SetValue(ourType, _iPKey, null);
				Control ourControl = (Control)ourType.GetType().GetMethod("generateControl").Invoke(ourType, controlParams);
				ourContainer.Controls.Add(ourControl);
			}
			catch (TypeLoadException e)
			{
				HttpContext.Current.Trace.Warn("Unknown ControlType: " + field.Type);
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
		public PlaceHolder GenerateControls(List<AdminField> fields)
		{
			PlaceHolder phControls = new PlaceHolder();

			object ourData = GetData();

			foreach (AdminField field in fields)
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

			PlaceHolder phTabs = new PlaceHolder();
			TabContainer tabContainer = new TabContainer();
			if (atTable != null)
			{
				foreach (AdminTab tab in atTable.Tabs)
				{
					TabPanel tpTab = new TabPanel();
					tpTab.ID = tab.ID;
					tpTab.HeaderText = tab.Label;
					HttpContext.Current.Trace.Write("Rendering Tab:" + tab.ID );
					tpTab.Controls.Add(GenerateControls(tab.Fields));
					tabContainer.Tabs.Add(tpTab);
				}
			}

			phTabs.Controls.Add(tabContainer);
			return phTabs;
		}

		/// <summary>
		/// generates the table dbeditor page includes buttons
		/// </summary>
		/// <returns>PlaceHolder containing page</returns>
		public PlaceHolder GeneratePage()
		{
			PlaceHolder ourPage = new PlaceHolder();
			if (atTable != null)
			{

				ourPage.Controls.Add(GenerateTabs());

				Button saveButton = new Button();
				saveButton.Text = "Save";
				saveButton.CommandName = "saveEdit";
				saveButton.Click += saveEdit;

				Button cancelButton = new Button();
				cancelButton.Text = "Cancel";
				cancelButton.CommandName = "cancelEdit";
				cancelButton.Click += cancelEdit;

				if (atTable.bEmailButton)
				{
					Button emailButton = new Button();
					emailButton.Text = "Email";
					emailButton.CommandName = "emailButton";
					emailButton.Click += saveEdit;
					emailButton.Click += emailNewsletter;
					ourPage.Controls.Add(emailButton);
				}

				ourPage.Controls.Add(saveButton);
				ourPage.Controls.Add(cancelButton);


				if (_iPKey > 0) //this is optional
				{
					Button deleteButton = new Button();
					deleteButton.Text = "Delete";
					deleteButton.CommandName = "deleteEdit";
					deleteButton.CssClass = "buttonDelete";
					deleteButton.Click += deleteEdit;
					LiteralControl deleteScript = new LiteralControl();
					deleteScript.Text = "<script type=\"text/javascript\"> $(\".buttonDelete\").click(function(){ var bDelete = confirm(\"Are You Sure You Want To Delete This Item?\");	return bDelete;	}); </script>";

					ourPage.Controls.Add(deleteButton);
					ourPage.Controls.Add(deleteScript);
				}
			}
			else
			{
				LiteralControl errorMessage = new LiteralControl("<h1>DBEditor Could Not Be Loaded</h1><p> Please Try Again</p>");
				HttpContext.Current.Trace.Warn("DBEditor Couldn't Be Loaded - Missing Table");
				ourPage.Controls.Add(errorMessage);
			}
			return ourPage;
		}


		/// <summary>
		/// Finds a Control recursively. Note finds the first match and exists
		/// </summary>
		/// <param name="Root"></param>
		/// <param name="Id"></param>
		/// <returns></returns>

		public static Control FindControlRecursive(Control Root, string Id)
		{

			if (Root.ID == Id)
				return Root;

			foreach (Control Ctl in Root.Controls)
			{
				Control FoundCtl = FindControlRecursive(Ctl, Id);
				if (FoundCtl != null)
					return FoundCtl;
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
		protected static object getDataValue(Control ourControl, string sFieldType, Type ourType)
		{
			object[] dataParams = new object[] {ourControl, ourType};
			object ourDType = Activator.CreateInstance(null, "mjjames.AdminSystem.dataControls." + sFieldType.ToLower() + "Control").Unwrap();
			object dataValue = ourDType.GetType().GetMethod("getDataValue").Invoke(null, dataParams) ;
			return dataValue;
		}

		#endregion dbeditor methods

		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected virtual object GetData(){
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
			SqlDataSource sds = new SqlDataSource();
			
			sds.ConnectionString = _connectionstring;
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
		private string BuildSelectCommand(bool bListingMode)
		{
			string selectCommand = String.Empty;
			List<string> selectparams = new List<string>();

			if (atTable != null)
			{
				var listfields = atTable.Defaults;

				if (bListingMode)
				{
					listfields = atTable.Defaults.FindAll(t => t.Attributes.ContainsKey("list"));
				}

				foreach (AdminField field in listfields)
				{
					selectparams.Add("[" + field.ID + "]");
				}

				selectCommand = String.Format("SELECT {0} FROM [{1}]", String.Join(" , ", selectparams.ToArray()), atTable.ID);

				if (listfields.Find(f => f.Attributes.ContainsKey("listfilter")) != null)
				{
					string filter = " WHERE ";
					bool bMultiWhere = false;
					foreach (AdminField af in listfields.FindAll(lf => lf.Attributes.ContainsKey("listfilter")))
					{
						if (bMultiWhere)
						{
							filter += " AND ";
						}

						filter += String.Format(" [{0}] = @{1} ", af.ID, af.ID);
						bMultiWhere = true;
					}

					selectCommand += filter;
				}
			}
			return selectCommand;
		}

		private string BuildDeleteCommand()
		{
			string deleteCommand = String.Empty;
			if (atTable != null)
			{
				deleteCommand = String.Format("DELETE FROM [{0}] WHERE [{1}] = @{2}", atTable.ID, TablePrimaryKeyField, TablePrimaryKeyField);
			}
			return deleteCommand;
		}

		private string BuildInsertCommand()
		{
			string insertCommand = String.Empty;
			if (atTable != null)
			{

			}
			return insertCommand;
		}

		private string BuildUpdateCommand()
		{
			string updateCommand = String.Empty;
			if (atTable != null)
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
		protected void cancelEdit(object sender, EventArgs e)
		{
			Button ourSender = (Button)sender;
			Page prevPage = ourSender.Page.PreviousPage;
			if (prevPage == null)
			{
				ourSender.Page.Response.Redirect("~/");
			}
			else
			{
				ourSender.Page.Response.Redirect(prevPage.Request.RawUrl);
			}
		}

		
		/// <summary>
		/// save away our data / insert - abstract
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void saveEdit(object sender, EventArgs e){
			throw new NotImplementedException();	
		}

		/// <summary>
		/// email's the newsletter - abstract
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void emailNewsletter(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Archive the Data - abstract
		/// </summary>
		public virtual void ArchiveData(int iKey){
			throw new NotImplementedException();
		}

		/// <summary>
		/// remove our data from the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void deleteEdit(object sender, EventArgs e)
		{

			Button ourSender = (Button)sender;
			adminDataContext ourPageDataContext = new adminDataContext();
			
			Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				ArchiveData(_iPKey);
			
				string strDelete = String.Format("DELETE FROM [{0}] WHERE [{1}] = {2}", atTable.ID, TablePrimaryKeyField, _iPKey);
				ourPageDataContext.ExecuteQuery<object>(strDelete);
				
				labelStatus.Text = String.Format("{0} Removed", atTable.ID);

				Page prevPage = ourSender.Page.PreviousPage;
				if (prevPage == null)
				{
					ourSender.Page.Response.Redirect("~/");
				}
				else
				{
					ourSender.Page.Response.Redirect(prevPage.Request.RawUrl);
				}

			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Removal Failed: {1}", atTable.ID, ex);
					
			}
		}
		
		#endregion

	
	}

}