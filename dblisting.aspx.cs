using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using mjjames.AdminSystem.classes;
using mjjames.core;
using System.Collections.Specialized;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.dataControls;
using mjjames.ControlLibrary.WebControls;

namespace mjjames.AdminSystem
{
	public partial class ListPage : Page
	{
		private string _sType = String.Empty;
		private XmlDBBase _xmldb;
		private readonly ILogger _logger = new Logger("DBListing");
		

		protected override void OnInit(EventArgs e)
		{

			base.OnInit(e);

			if (!String.IsNullOrEmpty(Request.QueryString["type"]))
			{
				_sType = Request.QueryString["type"];
			}

			_xmldb = Activator.CreateInstance(null, "mjjames.AdminSystem.XmlDB" + _sType).Unwrap() as XmlDBBase;
			if (_xmldb == null)
			{
				InvalidCastException exception = new InvalidCastException(String.Format("Can not cast: {0} to {1}", "XmlDB" + _sType, "XmlDBBase"));
				_logger.LogError("DBListing Unable To Cast", exception);
				throw exception;
			}

			_xmldb.TableName = _sType;
			_xmldb.ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString;


		}

		protected void Page_Load(object sender, EventArgs e)
		{
			HtmlHead head = Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"] + ": Admin - Page Listing";
		}

		protected void SetupTable(object sender, EventArgs e)
		{

			SqlDataSource datasource = _xmldb.DataSource(true, true, false, false, true);
			sdsData.SelectCommand = datasource.SelectCommand;
			sdsData.UpdateCommand = datasource.UpdateCommand;
			sdsData.InsertCommand = datasource.InsertCommand;
			sdsData.DeleteCommand = datasource.DeleteCommand;
			pageListing.DataKeyNames = new[] { _xmldb.TablePrimaryKeyField };
			sdsData.Deleting += SdsDataDeleting;

			CommandField cfSelect = new CommandField {ShowSelectButton = true, SelectText = "Edit"};

			pageListing.Columns.Add(cfSelect);

			if (_xmldb.TableDefaults != null)
			{

				foreach (AdminField field in _xmldb.TableDefaults.FindAll(t => t.Attributes.ContainsKey("list")))
				{
					BoundField dcf = new BoundField();
					string type;
					bool bType = field.Attributes.TryGetValue("type", out type);
					bool bRenderField = true;

					if (!Page.IsPostBack && field.Attributes.ContainsKey("listfilter"))
					{
						QueryStringParameter p = new QueryStringParameter(field.ID, field.ID);
						if (!sdsData.SelectParameters.Contains(p))
						{ //if post back this should already be in the params so dont add another one
							sdsData.SelectParameters.Add(p);
						}
					}

					dcf.HeaderText = field.Label;
					dcf.SortExpression = field.ID;
					dcf.DataField = field.ID;

					if (!bType) continue;
					// Specific Functionality Depends on Type
					switch (type)
					{
						case "hidden":
							bRenderField = false;
							break;
						case "datetime":
							dcf.DataFormatString = "{0:dd/MM/yyyy}";
							break;
						case "dropdown":
							DropdownControl ddc = new DropdownControl();
							DropDownList ddl = (DropDownList)ddc.GenerateControl(field, new object());
							TemplateField tcf = new TemplateField
							                    	{
							                    		ItemTemplate =
							                    			new GridViewDropDownListTemplate(DataControlRowType.DataRow, field.Label, field.ID, ddl,
							                    			                                 true),
							                    		HeaderText = field.Label,
							                    		SortExpression = field.ID
							                    	};

							bRenderField = false;
							pageListing.Columns.Add(tcf);
							break;
						default:
							break;
					}
					if (bRenderField)
					{
						pageListing.Columns.Add(dcf);
					}
				}
			}
			CommandField cfDelete = new CommandField {ShowDeleteButton = true};

			cfDelete.ControlStyle.CssClass = "buttonDelete";
			pageListing.Columns.Add(cfDelete);

			UpdateLabels(_xmldb.TableLabel, _xmldb);

		}

		void SdsDataDeleting(object sender, SqlDataSourceCommandEventArgs e)
		{
			_xmldb.ArchiveData((int)e.Command.Parameters["@" + _xmldb.TablePrimaryKeyField].Value);
		}

		protected void UpdateLabels(string label, XmlDBBase xmldb)
		{
			dbEditorLabel.Text = label;
			buttonAddPage.Text = String.Format("Add {0}", label);
			buttonAddPage.ToolTip = String.Format("Add a new {0}", label);
			levelLabel.InnerText = String.Format("{0}s at this level", label);

			string sForeignKeys = String.Empty;
			var listfilter = xmldb.TableDefaults.Find(t => t.Attributes.ContainsKey("listfilter"));

			if (listfilter != null)
			{
				sForeignKeys = listfilter.ID;
			}
			else
			{ //if we have no filter then add is always available
				buttonAddPage.NavigateUrl = String.Format("{0}{1}", buttonAddPage.NavigateUrl, _sType);
				buttonAddPage.Visible = true;
			}
			if (!String.IsNullOrEmpty(Request.QueryString[sForeignKeys]))
			{
				if (!buttonAddPage.NavigateUrl.Contains(sForeignKeys + "="))
				{
					buttonAddPage.NavigateUrl = String.Format("{0}{1}&{2}={3}", buttonAddPage.NavigateUrl, _sType, sForeignKeys, Request.QueryString[sForeignKeys]);
					buttonAddPage.Visible = true;
				}
				if (!linkbuttonBack.NavigateUrl.Contains(xmldb.TablePrimaryKeyField + "="))
				{
					linkbuttonBack.NavigateUrl = String.Format("{0}{1}&{2}={3}", linkbuttonBack.NavigateUrl, _sType, xmldb.TablePrimaryKeyField, Request.QueryString[sForeignKeys]);
				}
				if (Request.QueryString[sForeignKeys].Equals("0"))
				{
					linkbuttonBack.Visible = false;
					buttonAddPage.Visible = false;
				}
			}
			else
			{
				linkbuttonBack.Visible = false;
			}

		}

		//on select load up DBEditor
		protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string strURL = String.Format("~/DBEditor.aspx?{0}={1}&{2}", _xmldb.TablePrimaryKeyField, pageListing.SelectedValue, Request.QueryString);

			Response.Redirect(strURL);
		}

		//loads the quickedit listing
		protected void loadListing(object sender, EventArgs e)
		{
			string strParent = String.Empty;
			string strTitle = String.Empty;
			bool bQuickEdit = false;

			NameValueCollection config = new NameValueCollection();

			if (_xmldb.TableDefaults != null)
			{
				strParent = _xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("foreignkey")) != null ? _xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("foreignkey")).ID : "0";
				strTitle = _xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("treelistingtitle")) != null ? _xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("treelistingtitle")).ID : String.Empty;
				bQuickEdit = _xmldb.TableQuickEdit;
			}

			if (bQuickEdit)
			{
			

				if (_sType.Equals("newsletters", StringComparison.CurrentCultureIgnoreCase))
				{
					navigationSiteMap.SiteMapProvider = "newsletterNavigation";
					navigationSiteMap.ShowStartingNode = false;
				}
				else
				{
					string strQuery = String.Format("SELECT [{0}] AS [id], {1} AS [parent], [{2}] AS [title], CAST([{3}] AS nvarchar) AS [url], '' AS [roles] FROM [{4}] ORDER BY [parent], [title]", _xmldb.TablePrimaryKeyField, strParent, strTitle, _xmldb.TablePrimaryKeyField, _xmldb.TableName);
					string strURLPrefix = String.Format("~/DBEditor.aspx?type={0}&{1}=", _sType, _xmldb.TablePrimaryKeyField);

					config.Add("query", strQuery);
					config.Add("urlprefix", strURLPrefix);
					config.Add("connectionStringName", "ourDatabase");


					CustomSqlSiteMapProvider cssmp = new CustomSqlSiteMapProvider();
					cssmp.Initialize("Admin Navigation SiteMap", config);
					navigationSiteMap.Provider = cssmp;

					Page.Trace.Write("CUSTOM SITEMAP QUERY: " + strQuery);
				}
				TreeView treeview = new TreeView
				                    	{
				                    		ID = "treeListing",
				                    		PopulateNodesFromClient = true,
				                    		EnableClientScript = true,
				                    		ShowExpandCollapse = true,
				                    		ShowLines = true,
				                    		ExpandDepth = 1,
				                    		CssClass = "treeView",
				                    		EnableViewState = false,
				                    		CollapseImageToolTip = "hide child items",
				                    		ExpandImageToolTip = "show child items",
				                    		DataSource = navigationSiteMap
				                    	};

				treeview.DataBind();

				treePanel.ContentTemplateContainer.Controls.Add(treeview);
			}
			else
			{
				LiteralControl notavail = new LiteralControl("<p class=\"notavail\">Not Available</p>");
				treePanel.ContentTemplateContainer.Controls.Add(notavail);

				leftCol.Visible = false; //just not render quick edit
			}	

		}

	


	}
}