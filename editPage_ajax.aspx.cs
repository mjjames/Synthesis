using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;


public partial class editPage_ajax : System.Web.UI.Page
{
    private int _iPageFKey;
    protected void Page_Load(object sender, EventArgs e)
    {
        string sID = "";

		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"] + ": Admin - Edit Page";
		}
        mjjames.admin.xmlDB page = new mjjames.admin.xmlDB {TableName = "pages"};


        if (Request.QueryString.GetValues("id") != null && Request.QueryString["id"].Length > 0)
		{
            sID = Request.QueryString["id"];
		    page_key.Value = sID;
			linkbuttonSubPages.Visible = true;
			
		}
		if (page_key.Value.Length > 0)
		{
			page.PrimaryKey = int.Parse(page_key.Value);
		}
		
		//page.ForeignKey = 1;
		if (Request.QueryString.GetValues("fkey") != null && Request.QueryString["fkey"].Length > 0)
		{
			page.ForeignKey = int.Parse(Request.QueryString["fkey"]);
		}
		placeholderTabs.Controls.Add(page.GeneratePage());
        _iPageFKey = page.ForeignKey;
        ///ToDo This isnt working
        ///
        linkbuttonBack.PostBackUrl = string.Format("~/listpage.aspx?fkey={0}", _iPageFKey);
        linkbuttonSubPages.PostBackUrl = string.Format("~/listpage.aspx?fkey={0}", sID);
    }
}
