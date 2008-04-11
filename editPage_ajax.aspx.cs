using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;


public partial class editPage_ajax : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Edit Page";
		}
        mjjames.admin.xmlDB page = new mjjames.admin.xmlDB();
        page.TableName = "pages";
		
		
		if (Request.QueryString.GetValues("id") != null && Request.QueryString["id"].Length > 0)
		{
			page_key.Value = Request.QueryString["id"];
			buttonSubPages.Visible = true;
			
		}
		if (page_key.Value.Length > 0)
		{
			page.PrimaryKey = int.Parse(page_key.Value);
		}
		
		//page.ForeignKey = 1;
		if (Request.QueryString.GetValues("fkey") != null && Request.QueryString.GetValues("fkey").Length > 0)
		{
			page.ForeignKey = int.Parse(Request.QueryString["fkey"]);
		}
		placeholderTabs.Controls.Add(page.GeneratePage());
		
    }

	protected void showSubPages(Object sender, EventArgs e)
	{
		Response.Redirect("~/listpage.aspx?fkey=" + Request.QueryString["id"]);
	}
	protected void showPageList(Object sender, EventArgs e)
	{
		Response.Redirect("~/listpage.aspx?fkey=" + Request.QueryString["fkey"]);
	}



}
