using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
	    Title = "Welcome";
	}

    protected string GetVersionNumber()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
