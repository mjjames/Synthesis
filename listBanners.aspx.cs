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

public partial class listBanners : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Edit Page";
		}
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strURL = "~/editbanner.aspx?id=" + bannerListing.SelectedValue.ToString();
        Response.Redirect(strURL);   
    }
}
