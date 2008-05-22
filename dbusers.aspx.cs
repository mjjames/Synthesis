using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class dbusers : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!User.IsInRole("administrators"))
		{
			dbuserDropDownList.Visible = false;
			labelDBUser.Visible = false;
		}
		if (Request.QueryString["createroles"] == "true")
		{
			Roles.CreateRole("Administrators");
			Roles.CreateRole("Editors");
		}
	}

	protected void changeView(object sender, EventArgs e)
	{
		DropDownList dbuserList = (DropDownList) sender;
		if (dbuserList.SelectedValue == "createUser")
		{
			ChangePassword.Visible = false;
			CreateUserWizard.Visible = true;
		}
		else
		{
			ChangePassword.Visible = true;
			CreateUserWizard.Visible = false;
		}
	}
}
