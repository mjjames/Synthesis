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

public partial class error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		HtmlHead head = (HtmlHead)Page.Header;
		head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - An Error Has Occurred";
    }
}
