using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;


public partial class editPage_ajax : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		string sID = "";
		int _iPageFKey = 0;

		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"] + ": Admin - Edit Page";
		}
		mjjames.admin.xmlDB page = new mjjames.admin.xmlDB();
		page.TableName = "pages";


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
			_iPageFKey = int.Parse(Request.QueryString["fkey"]);
		}
		placeholderTabs.Controls.Add(page.GeneratePage());
		page.ForeignKey = _iPageFKey;
		///ToDo This isnt working once you hit 3rd level
		///
		linkbuttonBack.PostBackUrl = string.Format("~/listpage.aspx?fkey={0}", _iPageFKey);
		linkbuttonSubPages.PostBackUrl = string.Format("~/listpage.aspx?fkey={0}", sID); 
		base.OnInit(e);
	}
    protected void Page_Load(object sender, EventArgs e)
    {
		
        
    }
}
