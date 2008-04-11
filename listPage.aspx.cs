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

public partial class listPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		HtmlHead head = (HtmlHead)Page.Header;
        head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Page Listing";
        
        if (Request.QueryString["fkey"] != null && Request.QueryString["fkey"] != "")
        {
			if(!buttonAddPage.PostBackUrl.Contains("fkey=")){
				buttonAddPage.PostBackUrl = buttonAddPage.PostBackUrl + "?fkey=" + Request.QueryString["fkey"];
			}
			if (!linkbuttonBack.PostBackUrl.Contains("id="))
			{
				linkbuttonBack.PostBackUrl = linkbuttonBack.PostBackUrl + "?id=" + Request.QueryString["fkey"];
			}
        }
        else
        {
            linkbuttonBack.Visible = false;
        }
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
		string strURL = "~/editpage_ajax.aspx?id=" + pageListing.SelectedValue.ToString();
        if (Request.QueryString["fkey"] != null && Request.QueryString["fkey"] != "")
        {
            strURL = strURL + "&fkey=" + Request.QueryString["fkey"];
        }
            Response.Redirect(strURL);   
    }

    protected void getFKey(object sender, EventArgs e)
    {
        if (Request.QueryString["fkey"] != null && Request.QueryString["fkey"] != "")
        {
            SqlDataSource1.DataSourceMode= SqlDataSourceMode.DataSet;
            SqlDataSource1.FilterExpression= "[page_fkey] ="+Request.QueryString["fkey"];            
        }
        else
        {
            SqlDataSource1.DataSourceMode = SqlDataSourceMode.DataSet;
            SqlDataSource1.FilterExpression= "[page_fkey] = 0";   
        }
        
    }
}
