using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
	    if (ConfigurationManager.AppSettings["SiteName"] == null) return;
	    HtmlHead head = Page.Header;
	    head.Title = ConfigurationManager.AppSettings["SiteName"] + ": Admin - Welcome Page";
	}

    protected string GetVersionNumber()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
