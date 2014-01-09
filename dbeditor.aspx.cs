using System;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using mjjames.AdminSystem.classes;
using mjjames.core;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem
{
	public partial class DBEditor : Page
	{
		private string _sType = String.Empty;
		private XmlDBBase _xmldb;
		private readonly ILogger _logger = new Logger("DBEditor");		

		protected override void OnInit(EventArgs e)
		{
			var sID = String.Empty;
			var pKey = 0;

			base.OnInit(e);
            if (Page.RouteData.Values.ContainsKey("type"))
			{
                _sType = Page.RouteData.Values["type"].ToString();
			}

			var oh = Activator.CreateInstance(null, "mjjames.AdminSystem.XmlDB" + _sType);
		
			if(oh == null)
			{
				var exception = new Exception(String.Format("Can not load XmlDB{0}", _sType));
				_logger.LogError("Invalid XMLDB", exception);
				throw exception;
			}
		
			_xmldb = oh.Unwrap() as XmlDBBase;

			if (_xmldb == null)
			{
				var exception = new InvalidCastException(String.Format("Can not cast: {0} to {1}", "XmlDB" + _sType, "XmlDBBase"));
				_logger.LogError("DBEditor Cast Error", exception);
				throw exception;
			}

            var id = Page.RouteData.Values.ContainsKey("id") ? Page.RouteData.Values["id"].ToString() : "";
            _xmldb.TableName = String.IsNullOrWhiteSpace(id) ? _sType : id;
            _xmldb.ArchiveDataEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["Synthesis:ArchiveDataEnabled"]);

			if (Page.RouteData.Values.ContainsKey("key"))
			{
				sID = Page.RouteData.Values["key"].ToString();
				pkey.Value = sID;
			}

			if (pkey.Value.Length > 0)
			{
				int.TryParse(pkey.Value, out pKey);
				_xmldb.PrimaryKey = pKey;
			}
			//if we have no site key we have an error - assume this is because of an expired session so log the user out
			if(Session["userSiteKey"] == null){
				FormsAuthentication.SignOut();
				Response.Redirect("~/authentication/default.aspx?ReturnUrl=" + Server.UrlEncode(Page.Request.Url.PathAndQuery), true);
			}
			
			_xmldb.SiteKey = int.Parse(Session["userSiteKey"].ToString());

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
                    linkbuttonSubPages.NavigateUrl = GetRouteUrl("DBListing", new { Type = _sType, FKey = sID }); 
                }

				int fKey;

                if (Page.RouteData.Values.ContainsKey("fkey") && Page.RouteData.Values["fkey"] != null)
				{
					int.TryParse(Page.RouteData.Values["fkey"].ToString(), out  fKey);
					_xmldb.ForeignKey = fKey;
				}
				else
				{
					fKey = _xmldb.ForeignKey;
				}
                linkbuttonBack.NavigateUrl = GetRouteUrl("DBListing", new { Type = _sType, FKey = fKey }); 
			}
			else
			{
                linkbuttonBack.NavigateUrl = GetRouteUrl("DBListing", new { Type = _sType}); 
			}

            //if (!String.IsNullOrWhiteSpace(id))
            //{
            //    linkbuttonBack.NavigateUrl += "&id=" + id;
            //}

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var hiddenField = helpers.FindControlRecursive(placeholderTabs, "control" + _xmldb.TablePrimaryKeyField) as HiddenField;
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

			Title = String.Format("{0} Editor: Edit View", sName);
			
			dbeditorLabel.Text = sName;
            if (_xmldb.TableDefaults.Find(d => d.Attributes.ContainsKey("foreignkey")) != null)
            {
                linkbuttonBack.Text = String.Format("View sibling {0}s", sName.ToLower());
                linkbuttonBack.ToolTip = String.Format("View sibling {0}s, these are {0}s that are at the same navigational level as this {0}", sName.ToLower());
            }
            else
            {
                linkbuttonBack.Text = String.Format("Back To {0} Listing", sName);
                linkbuttonBack.ToolTip = String.Format("Back To {0} Listing", sName);
            }
			linkbuttonSubPages.Text = String.Format("Child {0}s", sName);
			linkbuttonSubPages.ToolTip = String.Format("Show Child {0}s", sName);

		}

		protected void LoadListing(object sender, EventArgs e)
		{
			var strParent = String.Empty;
			var strTitle = String.Empty;
			var bQuickEdit = false;
			var config = new NameValueCollection();

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
                    var strQuery = _xmldb.GetQuickEditSiteMapQuery();

                    //generate a url prefix based on the route
                    var strURLPrefix = GetRouteUrl("DBEditor", new
                    {
                        Type = _sType,
                        Key = 0,
                        FKey = 0
                    });
                    //but as we pass key and fkey of 0 it will end in /0/0 remove this for the sitemap
                    //to add the key
                    strURLPrefix = strURLPrefix.Replace("/0/0", "/");

                        
					config.Add("query", strQuery);
					config.Add("urlprefix", strURLPrefix);
					config.Add("connectionStringName", "ourDatabase");
					config.Add("DisableRootURLFix", "true");

					var cssmp = new CustomSqlSiteMapProvider
									{
										SiteKey = int.Parse(Session["userSiteKey"].ToString())
									};

					cssmp.Initialize("Admin Navigation SiteMap", config);
					navigationSiteMap.Provider = cssmp;

					Page.Trace.Write("CUSTOM SITEMAP QUERY: " + strQuery);
				}
				var treeview = new TreeView
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
				var notavail = new LiteralControl("<p class=\"notavail\">Not Available</p>");
				treePanel.ContentTemplateContainer.Controls.Add(notavail);

				leftCol.Visible = false; //just not render quick edit
			}

		}
	}
}