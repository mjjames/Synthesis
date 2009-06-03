using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using mjjames.AdminSystem;
using mjjames.core;
using System.Collections.Specialized;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.dataControls;
using mjjames.ControlLibrary.WebControls;

public partial class listPage : System.Web.UI.Page
{
	private string sType = String.Empty;
	private int iPKey = 0;
	private XmlDBBase xmldb;

	protected override void OnInit(EventArgs e)
	{

		base.OnInit(e);

		if (!String.IsNullOrEmpty(Request.QueryString["type"]))
		{
			sType = Request.QueryString["type"];
		}

		xmldb = Activator.CreateInstance(null, "mjjames.AdminSystem.XmlDB" + sType).Unwrap() as XmlDBBase;
		if (xmldb == null)
		{
			throw new InvalidCastException(String.Format("Can not cast: {0} to {1}", "XmlDB" + sType, "XmlDBBase"));
		}

		xmldb.TableName = sType;
		xmldb.ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString;


	}

	protected void Page_Load(object sender, EventArgs e)
	{
		HtmlHead head = (HtmlHead)Page.Header;
		head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Page Listing";
	}

	protected void SetupTable(object sender, EventArgs e)
	{

		SqlDataSource datasource = xmldb.DataSource(true, true, false, false, true);
		sdsData.SelectCommand = datasource.SelectCommand;
		sdsData.UpdateCommand = datasource.UpdateCommand;
		sdsData.InsertCommand = datasource.InsertCommand;
		sdsData.DeleteCommand = datasource.DeleteCommand;
		pageListing.DataKeyNames = new[] { xmldb.TablePrimaryKeyField };
		sdsData.Deleting += new SqlDataSourceCommandEventHandler(sdsData_Deleting);

		CommandField cfSelect = new CommandField();
		cfSelect.ShowSelectButton = true;
		cfSelect.SelectText = "Edit";

		pageListing.Columns.Add(cfSelect);

		if (xmldb.TableDefaults != null)
		{

			foreach (AdminField field in xmldb.TableDefaults.FindAll(t => t.Attributes.ContainsKey("list")))
			{
				BoundField dcf = new BoundField();
				string Type = String.Empty;
				bool bType = field.Attributes.TryGetValue("type", out Type);
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

				if (bType)
				{
					// Specific Functionality Depends on Type
					switch (Type)
					{
						case "hidden":
							bRenderField = false;
							break;
						case "datetime":
							dcf.DataFormatString = "{0:dd/MM/yyyy}";
							break;
						case "dropdown":
							dropdownControl ddc = new dropdownControl();
							DropDownList ddl = (DropDownList)ddc.generateControl(field, new object());
							TemplateField tcf = new TemplateField();

							tcf.ItemTemplate = new GridViewDropDownListTemplate(DataControlRowType.DataRow, field.Label, field.ID, ddl, true);
							tcf.HeaderText = field.Label;
							tcf.SortExpression = field.ID;
							
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
		}
		CommandField cfDelete = new CommandField();

		cfDelete.ShowDeleteButton = true;
		cfDelete.ControlStyle.CssClass = "buttonDelete";
		pageListing.Columns.Add(cfDelete);

		UpdateLabels(xmldb.TableLabel, xmldb);

	}

	void sdsData_Deleting(object sender, SqlDataSourceCommandEventArgs e)
	{
		xmldb.ArchiveData((int)e.Command.Parameters["@" + xmldb.TablePrimaryKeyField].Value);
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
			buttonAddPage.PostBackUrl = String.Format("{0}{1}", buttonAddPage.PostBackUrl, sType);
			buttonAddPage.Enabled = true;
			buttonAddPage.Visible = true;
		}
		if (!String.IsNullOrEmpty(Request.QueryString[sForeignKeys]))
		{
			if (!buttonAddPage.PostBackUrl.Contains(sForeignKeys + "="))
			{
				buttonAddPage.PostBackUrl = String.Format("{0}{1}&{2}={3}", buttonAddPage.PostBackUrl, sType, sForeignKeys, Request.QueryString[sForeignKeys]);
				buttonAddPage.Enabled = true;
				buttonAddPage.Visible = true;
			}
			if (!linkbuttonBack.PostBackUrl.Contains(xmldb.TablePrimaryKeyField + "="))
			{
				linkbuttonBack.PostBackUrl = String.Format("{0}{1}&{2}={3}", linkbuttonBack.PostBackUrl, sType, xmldb.TablePrimaryKeyField, Request.QueryString[sForeignKeys]);
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

	//on select load up dbeditor
	protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
	{
		string strURL = String.Format("~/dbeditor.aspx?{0}={1}&{2}", xmldb.TablePrimaryKeyField, pageListing.SelectedValue.ToString(), Request.QueryString.ToString());

		Response.Redirect(strURL);
	}

	//loads the quickedit listing
	protected void loadListing(object sender, EventArgs e)
	{
		string strQuery = String.Empty;
		string strURLPrefix = String.Empty;
		string strParent = String.Empty;
		string strTitle = String.Empty;
		bool bQuickEdit = false;

		NameValueCollection config = new NameValueCollection();

		if (xmldb.TableDefaults != null)
		{
			strParent = xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("foreignkey")) != null ? xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("foreignkey")).ID : "0";
			strTitle = xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("treelistingtitle")) != null ? xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("treelistingtitle")).ID : String.Empty;
			bQuickEdit = xmldb.TableQuickEdit;
		}

		if (bQuickEdit)
		{
			

			if (sType.Equals("newsletters", StringComparison.CurrentCultureIgnoreCase))
			{
				navigationSiteMap.SiteMapProvider = "newsletterNavigation";
				navigationSiteMap.ShowStartingNode = false;
			}
			else
			{
				strQuery = String.Format("SELECT [{0}] AS [id], {1} AS [parent], [{2}] AS [title], CAST([{3}] AS nvarchar) AS [url], '' AS [roles] FROM [{4}] ORDER BY [parent], [title]", xmldb.TablePrimaryKeyField, strParent, strTitle, xmldb.TablePrimaryKeyField, xmldb.TableName);
				strURLPrefix = String.Format("~/dbeditor.aspx?type={0}&{1}=", sType, xmldb.TablePrimaryKeyField);

				config.Add("query", strQuery);
				config.Add("urlprefix", strURLPrefix);
				config.Add("connectionStringName", "ourDatabase");


				CustomSqlSiteMapProvider cssmp = new CustomSqlSiteMapProvider();
				cssmp.Initialize("Admin Navigation SiteMap", config);
				navigationSiteMap.Provider = cssmp;

				Page.Trace.Write("CUSTOM SITEMAP QUERY: " + strQuery);
			}
			TreeView treeview = new TreeView();

			treeview.ID = "treeListing";
			treeview.PopulateNodesFromClient = true;
			treeview.EnableClientScript = true;
			treeview.ShowExpandCollapse = true;
			treeview.ShowLines = true;
			treeview.ExpandDepth = 1;
			treeview.CssClass = "treeView";
			treeview.EnableViewState = false;
			treeview.CollapseImageToolTip = "hide child items";
			treeview.ExpandImageToolTip = "show child items";
			treeview.DataSource = navigationSiteMap;
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
