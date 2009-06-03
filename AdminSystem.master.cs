using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

public partial class AdminSystem : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Page.User.Identity.IsAuthenticated)
		{
			signout.Visible = true;
			adminToolbar.Visible = true;
		}
		
	}

	protected void btnSignOut_ServerClick(object sender, System.EventArgs e)
	{
		FormsAuthentication.SignOut();
		Response.Redirect("/admin/authentication/default.aspx", true);
	}

}
