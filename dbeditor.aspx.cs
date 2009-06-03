using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using mjjames.AdminSystem;
using mjjames.core;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using mjjames.ControlLibrary.WebControls;
using mjjames.AdminSystem.dataentities;


public partial class dbeditor : System.Web.UI.Page
{
	private string sType = String.Empty;
	private XmlDBBase xmldb;

	protected override void OnInit(EventArgs e)
	{
		string sID = String.Empty;
		int _iFKey = 0;
		int _iPKey = 0;

		base.OnInit(e);

		if (Request.QueryString.GetValues("type") != null && Request.QueryString["type"].Length > 0)
		{
			sType = Request.QueryString["type"];
		}


		xmldb = Activator.CreateInstance(null, "mjjames.AdminSystem.XmlDB" + sType).Unwrap() as XmlDBBase;

		if (xmldb == null)
		{
			throw new InvalidCastException(String.Format("Can not cast: {0} to {1}", "XmlDB" + sType, "XmlDBBase"));
		}

		xmldb.TableName = sType;


		if (!String.IsNullOrEmpty(Request.QueryString[xmldb.TablePrimaryKeyField]))
		{
			sID = Request.QueryString[xmldb.TablePrimaryKeyField];
			pkey.Value = sID;
		}

		if (pkey.Value.Length > 0)
		{
			int.TryParse(pkey.Value, out _iPKey);
			xmldb.PrimaryKey = _iPKey;
		}
		placeholderTabs.Controls.Add(xmldb.GeneratePage());


		AdminField field = null;

		if (xmldb.TableDefaults != null)
		{
			field = xmldb.TableDefaults.Find(t => t.Attributes.ContainsKey("listfilter"));
		}

		if (field != null)
		{
			if (_iPKey > 0)
			{
				linkbuttonSubPages.Visible = true;
				linkbuttonSubPages.PostBackUrl = string.Format("~/dblisting.aspx?{0}={1}&type={2}", field.ID, sID, sType);
			}

			if (!String.IsNullOrEmpty(Request.QueryString[field.ID]))
			{
				int.TryParse(Request.QueryString[field.ID], out  _iFKey);
				xmldb.ForeignKey = _iFKey;
			}
			else
			{
				_iFKey = xmldb.ForeignKey;
			}

			linkbuttonBack.PostBackUrl = string.Format("~/dblisting.aspx?{0}={1}&type={2}", field.ID, _iFKey, sType);


		}
		else
		{
			linkbuttonBack.PostBackUrl = string.Format("~/dblisting.aspx?type={0}", sType);
		}



	}

	protected void Page_Load(object sender, EventArgs e)
	{
		HiddenField pkey = mjjames.core.helpers.FindControlRecursive(placeholderTabs, "control" + xmldb.TablePrimaryKeyField) as HiddenField;
		if (pkey != null)
		{
			if (!String.IsNullOrEmpty(pkey.Value))
			{
				xmldb.PrimaryKey = int.Parse(pkey.Value);
		
			}
		}
		updateLabels(sType);
	}

	private void updateLabels(string sType)
	{
		string sName = xmldb.TableLabel;

		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = String.Format("{0}:Admin - Edit {1}", ConfigurationManager.AppSettings["SiteName"], sName);
		}

		dbeditorLabel.Text = sName;
		linkbuttonBack.Text = String.Format("Back To {0} Listing", sName);
		linkbuttonBack.ToolTip = String.Format("Back To {0} Listing", sName);
		linkbuttonSubPages.Text = String.Format("Sub {0}s", sName);
		linkbuttonSubPages.ToolTip = String.Format("Show Sub {0}s", sName);

	}

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
