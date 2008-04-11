using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using AjaxControlToolkit;
using FredCK.FCKeditorV2;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.admin
{
	public class xmlDB
	{
		private string sTableName;
		private string sXMLFile;
		private XDocument xdXML;
		private adminDataClassesDataContext adminDC;
		private int iPKey;
		private int iFKey;
		private AdminTable atTable;

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
		public int PrimaryKey { set { iPKey = value; } get { return iPKey; } }

		/// <summary>
		/// provides the tables foreign key for linking
		/// </summary>
		public int ForeignKey { set { iFKey = value; } get { return iFKey; } }

		/// <summary>
		/// constructor
		/// </summary>
		public xmlDB()
		{
			sXMLFile = (string)ConfigurationManager.AppSettings["adminConfigxml"];
			string sFilePath = HttpContext.Current.Server.MapPath(sXMLFile);
			xdXML = XDocument.Load(sFilePath);
			HttpContext.Current.Trace.Write("adminConfigxml: " + sFilePath);
			
			adminDC = new adminDataClassesDataContext();
		}

		private void buildAdminTable()
		{
			var xmlQuery = from table in xdXML.Descendants("table")
						   where table.Attribute("id").Value == sTableName
						   select new AdminTable
						   {
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
			atTable = xmlQuery.First();
		}

		/// <summary>
		/// Renders a WebControl for the field specified
		/// </summary>
		/// <param name="node">XMLNode to render a control for</param>
		/// <param name="ourPage"></param>
		/// <returns>PlaceHolder containing rendered control</returns>
		private PlaceHolder RenderControl(AdminField field, page ourPage)
		{
			HttpContext.Current.Trace.Write("Rendering Control: " + field.Type);
			PlaceHolder renderedControl = new PlaceHolder();
			bool bRenderControl = true;
			PropertyInfo ourProperty;

			Label ourLabel = new Label();
			WebControl ourContainer = new WebControl(HtmlTextWriterTag.Div);
			ourContainer.CssClass = "row";

			ourLabel.Text = field.Label;
			ourLabel.CssClass = "label";
			ourLabel.ID = "label" + field.ID;
			ourLabel.AssociatedControlID = "control" + field.ID;

			ourContainer.Controls.Add(ourLabel);

			switch (field.Type)
			{
				case "text":

					TextBox ourControl = new TextBox();
					ourControl.ID = "control" + field.ID;
					ourControl.CssClass = "field";
					if (field.Attributes.ContainsKey("maxlength"))
					{
						int iMaxLength = int.Parse(field.Attributes["maxlength"]);
						if (iMaxLength > 0)
						{
							ourControl.MaxLength = iMaxLength;
							ourControl.Columns = iMaxLength;
						}
					}
					if (field.Attributes.ContainsKey("rows"))
					{
						int iRows = int.Parse(field.Attributes["rows"]);
						if (iRows > 0)
						{
							ourControl.TextMode = TextBoxMode.MultiLine;
							ourControl.Rows = iRows;
							ourControl.Wrap = true;
						}
					}
					ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
					if (iPKey > 0 && ourProperty != null)
					{
						string ourValue = (string)ourProperty.GetValue(ourPage, null);
						ourControl.Text = "" + ourValue;
					}
					ourContainer.Controls.Add(ourControl);
					break;

				case "rte":
					FCKeditor fckEditor = new FCKeditor();
					fckEditor.ID = "control" + field.ID;
					fckEditor.CustomConfigurationsPath = "/admininc/fckSettings.js";
					fckEditor.BasePath = "~/fckeditor/";
					fckEditor.ToolbarCanCollapse = false;
					fckEditor.ToolbarSet = "mjjames";
					fckEditor.EnableSourceXHTML = true;
					fckEditor.EnableXHTML = true;
					fckEditor.FormatOutput = true;
					fckEditor.FormatSource = true;
					fckEditor.EnableSourceXHTML = true;
					fckEditor.Config["HtmlEncodeOutput"] = "true";

					ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
					if (iPKey > 0 && ourProperty != null)
					{
						string ourValue = (string)ourProperty.GetValue(ourPage, null);
						fckEditor.Value = HttpContext.Current.Server.HtmlDecode("" + ourValue);
					}

					ourContainer.Controls.Add(fckEditor);
					break;

				case "checkbox":
					CheckBox ourCheckBox = new CheckBox();
					ourCheckBox.ID = "control" + field.ID;
					ourCheckBox.CssClass = "field";

					ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(bool));
					if (iPKey > 0 && ourProperty != null)
					{
						bool ourValue = (bool)ourProperty.GetValue(ourPage, null);
						ourCheckBox.Checked = ourValue;
					}

					ourContainer.Controls.Add(ourCheckBox);
					break;

				case "hidden":
					HiddenField ourHidden = new HiddenField();
					ourHidden.ID = "control" + field.ID;
					HttpContext.Current.Trace.Warn("Rendering ControlID: "+ ourHidden.ID);
					ourProperty = ourPage.GetType().GetProperty(field.ID);
					if (iPKey > 0 &&  ourProperty != null)
					{
						string ourValue = (ourProperty.GetValue(ourPage, null) + "").ToString();
						ourHidden.Value = "" + ourValue;
						HttpContext.Current.Trace.Warn("Rendering Control Value: " + ourHidden.Value);
					}
					ourContainer.Controls.Add(ourHidden);
					break;

				case "file":
					UpdatePanel fileUpload = new UpdatePanel();
					FileUpload ourUploader = new FileUpload();
					Button uploadButton = new Button();
					HiddenField fileHidden = new HiddenField();

					fileUpload.ID = "panel" + field.ID;
					

					uploadButton.ID = "button" + field.ID;
					uploadButton.Text = "Upload";
					uploadButton.Click += fileUploader;
					uploadButton.CommandName = "submit";
					ourUploader.ID = "control" + field.ID;
					fileHidden.ID = "hidden" + field.ID;

					fileUpload.UpdateMode = UpdatePanelUpdateMode.Conditional;
					ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);
					ourSM.RegisterPostBackControl(uploadButton);

					fileUpload.ContentTemplateContainer.Controls.Add(ourUploader);
					fileUpload.ContentTemplateContainer.Controls.Add(uploadButton);
					fileUpload.ContentTemplateContainer.Controls.Add(fileHidden);

					ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
					string ourFileValue = "";

					if (iPKey > 0 && ourProperty != null)
					{
						ourFileValue = (string)ourProperty.GetValue(ourPage, null);
						fileHidden.Value = ourFileValue;
					}

					if (field.Attributes.ContainsKey("preview"))
					{
						if (field.Attributes["preview"] == "enabled")
						{
							Image imagePreview = new Image();
							imagePreview.AlternateText = "No Preview Available";
							imagePreview.CssClass = "previewImg";
							imagePreview.ID = "image" + field.ID;
							imagePreview.Width = 200;
							imagePreview.ImageUrl = null;

							string strDir = ConfigurationManager.AppSettings["uploaddir"];
							imagePreview.ImageUrl = strDir + ourFileValue;
							
							fileUpload.ContentTemplateContainer.Controls.Add(imagePreview);
						}
					}
					ourContainer.Controls.Add(fileUpload);
					break;

				default:
					//lets just skip over unknowns
					HttpContext.Current.Trace.Warn("Unknown ControlType");
					bRenderControl = false;
					break;
			}

			if (bRenderControl)
			{
				renderedControl.Controls.Add(ourContainer);
			}

			return renderedControl;
		}


		/// <summary>
		/// Checks File Size
		/// </summary>
		/// <param name="iFSize">File Size</param>
		/// <returns>Boolean indicating valid or not</returns>
		private bool checkFileSize(long iFSize)
		{
			bool bCheck = false;

			long iMaxSize = long.Parse(ConfigurationManager.AppSettings["maxFileSize"]) * 1024 * 1024;
			if (iFSize <= iMaxSize)
			{
				bCheck = true;
			}
			return bCheck;
		}

		/// <summary>
		/// Uploads the provided fileupload file
		/// </summary>
		/// <param name="sender">Button Calling Upload</param>
		/// <param name="e"></param>
		protected void fileUploader(Object sender, EventArgs e)
		{
			Button ourSender = (Button)sender;

			FileUpload ourFile = (FileUpload)ourSender.Parent.FindControl(ourSender.ID.Replace("button", "control"));
			if (ourFile != null)
			{
				
				if (ourFile.PostedFile.ContentLength > 0)
				{
					bool bValidFileType = false;
					bool bValidFileSize = false;
					try
					{
						bValidFileType = ConfigurationManager.AppSettings["fileTypes"].Contains(ourFile.PostedFile.ContentType.ToLower());
						bValidFileSize = checkFileSize(ourFile.PostedFile.ContentLength);
						if (bValidFileSize && bValidFileType)
						{
							string strDir = ConfigurationManager.AppSettings["uploaddir"] + "files/";
							ourFile.SaveAs(HttpContext.Current.Server.MapPath(strDir) + ourFile.PostedFile.FileName);

							Image ourImage = (Image)ourSender.Parent.FindControl(ourSender.ID.Replace("button", "image"));
							HiddenField ourHiddenFile = (HiddenField)ourSender.Parent.FindControl(ourSender.ID.Replace("button", "hidden"));
							if (ourHiddenFile != null)
							{
								ourHiddenFile.Value = strDir + ourFile.FileName;
							}
							if (ourImage != null)
							{
								ourImage.ImageUrl = strDir + ourFile.FileName;
								ourImage.AlternateText = "Preview";
							}

						}
						else
						{
							if (!bValidFileType)
							{
								throw new Exception("Invalid File Type");
							}
							if (!bValidFileSize)
							{
								throw new Exception("File Too Large");
							}
							
						}
					}
					catch (Exception ex)
					{
						LiteralControl labelStatus = new LiteralControl();
						labelStatus.Text = "An Error Occured While Uploading Your Thumbnail: ";
						labelStatus.Text += ex.Message.ToString();
						ourSender.Parent.Controls.Add(labelStatus);
					}
				}
				else
				{

				}
			}
		}

		/// <summary>
		/// generate the controls for the tab / snippet
		/// </summary>
		/// <returns>placeholder containing controls</returns>
		public PlaceHolder GenerateControls(List<AdminField> fields)
		{
			PlaceHolder phControls = new PlaceHolder();

			var pages = from p in adminDC.pages
						select p;

			page ourPage = new page();
			if (iPKey > 0)
			{
				ourPage = pages.Single(p => p.page_key == iPKey);
			}

			if (iFKey > 0)
			{
				ourPage.page_fkey = iFKey;
			}
			


			HttpContext.Current.Trace.Write("Rendering Tab");

			foreach (AdminField field in fields)
			{
				phControls.Controls.Add(RenderControl(field, ourPage));
				//databind here
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
			foreach (AdminTab tab in atTable.Tabs)
			{
				TabPanel tpTab = new TabPanel();
				tpTab.ID = tab.ID;
				tpTab.HeaderText = tab.Label;
				tpTab.Controls.Add(GenerateControls(tab.Fields));
				tabContainer.Tabs.Add(tpTab);
			}

			phTabs.Controls.Add(tabContainer);
			return phTabs;
		}

		/// <summary>
		/// generates the table page includes buttons
		/// </summary>
		/// <returns>PlaceHolder containing page</returns>
		public PlaceHolder GeneratePage()
		{
			PlaceHolder ourPage = new PlaceHolder();
			ourPage.Controls.Add(GenerateTabs());

			Button saveButton = new Button();
			saveButton.Text = "Save";
			saveButton.CommandName = "saveEdit";
			saveButton.Click += saveEdit;

			Button cancelButton = new Button();
			cancelButton.Text = "Cancel";
			cancelButton.CommandName = "cancelEdit";
			cancelButton.Click += cancelEdit;

			ourPage.Controls.Add(saveButton);
			ourPage.Controls.Add(cancelButton);
			
			if (iPKey > 0) //this is optional
			{
				Button deleteButton = new Button();
				deleteButton.Text = "Delete";
				deleteButton.CommandName = "deleteEdit";
				deleteButton.Click += deleteEdit;
				ourPage.Controls.Add(deleteButton);
			}
			
			

			return ourPage;
		}


		/// <summary>
		/// Finds a Control recursively. Note finds the first match and exists
		/// </summary>
		/// <param name="ContainerCtl"></param>
		/// <param name="IdToFind"></param>
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
		private static object getDataValue(Control ourControl, string sFieldType, Type ourType)
		{

			switch (sFieldType)
			{
				case "text":
					TextBox ourTextBox = (TextBox)ourControl;
					if (ourTextBox.Text == "" && !ourType.FullName.Contains("String"))
					{
						return null;
					}
					if (ourType.FullName.Contains("Int"))
					{

						return int.Parse("" + ourTextBox.Text);
					}
					else
					{
						return Convert.ChangeType(ourTextBox.Text, ourType);
					}
					break;
				case "rte":
					FCKeditor ourFCK = (FCKeditor)ourControl;
					return ourFCK.Value;
					break;
				case "hidden":
					HiddenField ourHidden = (HiddenField)ourControl;
					HttpContext.Current.Trace.Write("Saving Content Value: " + ourHidden.Value);
					if (ourType.FullName.Contains("Int"))
					{
						if (ourHidden.Value == "")
						{
							return null;
						}
						HttpContext.Current.Trace.Write("Saving Content Value As Int: " + int.Parse(ourHidden.Value));
						return int.Parse("" + ourHidden.Value);
					}
					else
					{
						return Convert.ChangeType(ourHidden.Value, ourType);
					}
					break;
				case "checkbox":
					CheckBox ourCheck = (CheckBox)ourControl;
					return ourCheck.Checked;
					break;
				case "file":
					//we don't use file inputs we use hidden ones for the actual files
					Control ourFileControl = ourControl.Parent.FindControl(ourControl.ID.Replace("control", "hidden"));
					HiddenField ourHiddenFile = (HiddenField) ourFileControl;
					return Convert.ChangeType(ourHiddenFile.Value, ourType);
					break;
				default:
					return null;
					break;

			}
		}

		// Events

		/// <summary>
		/// Makes A LinqDataSource
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void PageLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
		{
			var pages = from p in adminDC.pages
						select p;

			page ourPage = pages.Single(p => p.page_key == iPKey);
			e.Result = ourPage;
		}

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
		/// save away our data / insert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void saveEdit(object sender, EventArgs e)
		{
			Button ourSender = (Button)sender;
			adminDataClassesDataContext ourPageDataContext = new adminDataClassesDataContext();
			page ourPageData = new page();
			if (iPKey > 0)
			{
				ourPageData = ourPageDataContext.pages.Single(p => p.page_key == iPKey);
			}
			
			var ourfields = from fields in atTable.Tabs
						 select new {
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
							PropertyInfo ourProperty = ourPageData.GetType().GetProperty(field.ID);
							if (ourProperty != null)
							{
								HttpContext.Current.Trace.Warn("Saving Content In: " + ourControl.ID);
								ourProperty.SetValue(ourPageData, getDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
							}
						}
					}
				}
			}
			
			if (iPKey == 0)
			{
				ourPageDataContext.pages.InsertOnSubmit(ourPageData);
				ourPageData.page_fkey = iFKey;
			}

			Label labelStatus = (Label)FindControlRecursive(ourSender.Page,("labelStatus"));
			try
			{
				ChangeSet ourChanges = ourPageDataContext.GetChangeSet();

				labelStatus.Text = "Nothing to Save";
				
				ourPageDataContext.SubmitChanges();

				if (ourChanges.Inserts.Count > 0)
				{
					labelStatus.Text = "Page Inserted";
					iPKey = ourPageData.page_key;
					iFKey = (int)ourPageData.page_fkey;
					HiddenField ourPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "page_key");
					HiddenField ourControlPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "controlpage_key");
					ourControlPKey.Value = iPKey.ToString();
					HiddenField ourControlFKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "controlpage_fkey");
					ourControlFKey.Value = iFKey.ToString();
					try
					{
						ourPKey.Value = iPKey.ToString();
					}
					catch
					{
						throw new Exception("Page doesn't contain a hidden control called page_fkey");
					}
				}
				if (ourChanges.Updates.Count > 0)
				{
					labelStatus.Text = "Page Updated";
				}
				
				
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Page Update Failed: " + ex.ToString();
			}
		}

		/// <summary>
		/// remove our data from the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void deleteEdit(object sender, EventArgs e)
		{

			Button ourSender = (Button)sender;
			adminDataClassesDataContext ourPageDataContext = new adminDataClassesDataContext();
			page ourPageData = new page();
			if (iPKey > 0)
			{
				ourPageData = ourPageDataContext.pages.Single(p => p.page_key == iPKey);
			}
			Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				ChangeSet ourChanges = ourPageDataContext.GetChangeSet();
				ourPageDataContext.pages.DeleteOnSubmit(ourPageData);
				ourPageDataContext.SubmitChanges();
				labelStatus.Text = "Page Removed";
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
				labelStatus.Text = "Page Removal Failed: " + ex.ToString();
			}
		}
	}


	public class AdminTable
	{
		public List<AdminField> Defaults { set; get; }
		public List<AdminTab> Tabs { set; get; }
	}

	public class AdminTab
	{
		public List<AdminField> Fields { set; get; }
		public string ID { set; get; }
		public string Label { set; get; }
	}

	public class AdminField
	{

		public string ID { set; get; }
		public string Type { set; get; }
		public string Label { set; get; }

		private Dictionary<string, string> _Attributes;

		public Dictionary<string, string> Attributes
		{
			set { _Attributes = value; }
			get { return _Attributes; }
		}

		public void SetAttribute(string name, string value)
		{
			_Attributes.Add(name, value);
		}

		public void SetAttributes(IEnumerable<XAttribute> Attributes)
		{

		}



	}


}