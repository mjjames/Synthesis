using System;
using System.Configuration;
using System.Runtime.Remoting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using mjjames.AdminSystem.classes;
using mjjames.core;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem
{
	public partial class DBEditor : System.Web.UI.Page
	{
		private string _sType = String.Empty;
		private XmlDBBase _xmldb;
		private readonly ILogger _logger = new Logger("DBEditor");		

		protected override void OnInit(EventArgs e)
		{
			string sID = String.Empty;
			int pKey = 0;

			base.OnInit(e);

			if (Request.QueryString.GetValues("type") != null && Request.QueryString["type"].Length > 0)
			{
				_sType = Request.QueryString["type"];
			}

			ObjectHandle oh = Activator.CreateInstance(null, "mjjames.AdminSystem.XmlDB" + _sType);
		
			if(oh == null)
			{
				Exception exception = new Exception(String.Format("Can not load XmlDB{0}", _sType));
				_logger.LogError("Invalid XMLDB", exception);
				throw exception;
			}
		
			_xmldb = oh.Unwrap() as XmlDBBase;

			if (_xmldb == null)
			{
				InvalidCastException exception = new InvalidCastException(String.Format("Can not cast: {0} to {1}", "XmlDB" + _sType, "XmlDBBase"));
				_logger.LogError("DBEditor Cast Error", exception);
				throw exception;
			}

			_xmldb.TableName = _sType;


			if (!String.IsNullOrEmpty(Request.QueryString[_xmldb.TablePrimaryKeyField]))
			{
				sID = Request.QueryString[_xmldb.TablePrimaryKeyField];
				pkey.Value = sID;
			}

			if (pkey.Value.Length > 0)
			{
				int.TryParse(pkey.Value, out pKey);
				_xmldb.PrimaryKey = pKey;
			}
			if(Session["userSiteKey"] != null){
				_xmldb.SiteKey = int.Parse(Session["userSiteKey"].ToString());
			}

			placeholderTabs.Controls.Add(_xmldb.GeneratePage());


			AdminField field = null;

			if (_xmldb.TableDefaults != null)
			{
				field = _xmldb.TableDefaults.Find(t => t.Attributes.ContainsKey("listfilter"));
			}

			if (field != null)
			{
				if (pKey > 0)
				{
					linkbuttonSubPages.Visible = true;
					linkbuttonSubPages.NavigateUrl = string.Format("~/dblisting.aspx?{0}={1}&type={2}", field.ID, sID, _sType);
				}

				int fKey;
				if (!String.IsNullOrEmpty(Request.QueryString[field.ID]))
				{
					int.TryParse(Request.QueryString[field.ID], out  fKey);
					_xmldb.ForeignKey = fKey;
				}
				else
				{
					fKey = _xmldb.ForeignKey;
				}

				linkbuttonBack.NavigateUrl = string.Format("~/dblisting.aspx?{0}={1}&type={2}", field.ID, fKey, _sType);


			}
			else
			{
				linkbuttonBack.NavigateUrl = string.Format("~/dblisting.aspx?type={0}", _sType);
			}



		}

		protected void Page_Load(object sender, EventArgs e)
		{
			HiddenField hiddenField = helpers.FindControlRecursive(placeholderTabs, "control" + _xmldb.TablePrimaryKeyField) as HiddenField;
			if (hiddenField != null)
			{
				if (!String.IsNullOrEmpty(hiddenField.Value))
				{
					_xmldb.PrimaryKey = int.Parse(hiddenField.Value);
		
				}
			}
			UpdateLabels();
		}

		private void UpdateLabels()
		{
			string sName = _xmldb.TableLabel;

			if (ConfigurationManager.AppSettings["SiteName"] != null)
			{
				HtmlHead head = Page.Header;
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
                    config.Add("DisableRootURLFix", "true");


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