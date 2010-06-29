using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.core;
using System.Collections.Specialized;
using System.Web.Security;
using System.Configuration;

namespace mjjames.AdminSystem.fckplugins.InternalUrl
{
	public partial class InternalUrl : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//if we have no site key we have an error - assume this is because of an expired session so log the user out
			if (Session["userSiteKey"] == null)
			{
				FormsAuthentication.SignOut();
				Response.Redirect("/admin/authentication/default.aspx?ReturnUrl=" + Server.UrlEncode(Page.Request.Url.PathAndQuery), true);
			}
		}
		protected void LoadListing(object sender, EventArgs e)
		{

			var strQuery = String.Format("SELECT [page_key] AS [id], [page_fkey] AS [parent], [navtitle] AS [title], CAST([page_url] AS nvarchar) AS [url], '' AS [roles] FROM [pages] WHERE [site_fkey] = @siteKey ORDER BY [parent], [title]");
			var config = new NameValueCollection();
			config.Add("query", strQuery);
			config.Add("urlwriting", "true");
			config.Add("connectionStringName", "ourDatabase");

            var siteKey = int.Parse(Session["userSiteKey"].ToString());

			var cssmp = new CustomSqlSiteMapProvider
							{
								SiteKey = siteKey,
                                SiteRootURL = LookupSitePath(siteKey)
							};

			cssmp.Initialize("Admin Internal Site Page Navigation SiteMap", config);
			navigationSiteMap.Provider = cssmp;

			Page.Trace.Write("CUSTOM SITEMAP QUERY: " + strQuery);

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
			treeview.NodeStyle.CssClass = "leaf";
			treeview.DataBind();

			treePanel.ContentTemplateContainer.Controls.Add(treeview);
		}


        /// <summary>
        /// Given a site key looks up its root path
        /// </summary>
        /// <param name="siteKey"></param>
        /// <returns></returns>
        private string LookupSitePath(int siteKey)
        {
            var cache = HttpContext.Current.Cache;
            var cacheKey = "siteRootPath-" + siteKey;
            if (cache[cacheKey] != null)
            {
                return cache[cacheKey] as String;
            }
            var context = new AdminSystem.DataContexts.AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            var siteData = (from s in context.sites
                            where s.site_key == siteKey && s.active
                            select s.hostname).FirstOrDefault();
            if (siteData == null)
            {
                return "";
            }
            var url = new Uri(siteData);
            cache[cacheKey] = url.AbsolutePath;
            return url.AbsolutePath;

        }
	}
}