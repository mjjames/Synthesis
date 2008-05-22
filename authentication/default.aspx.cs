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
using System.Data.SqlClient;

public partial class login : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - System Login";
		}
	}
	/// <summary>
	/// Validate Our User
	/// </summary>
	/// <param name="userName">Username</param>
	/// <param name="passWord">password</param>
	/// <returns></returns>
	private bool ValidateUser(string userName, string passWord)
	{
		return Membership.ValidateUser(userName, passWord);
	}

	/// <summary>
	/// Login Button Handler
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnLogin_Click(object sender, System.EventArgs e)
	{
		if (ValidateUser(inputUserName.Value, inputPassword.Value))
		{
			
			FormsAuthenticationTicket tkt;
			string cookiestr;
			string strRedirect;
			HttpCookie ck;


			tkt = new FormsAuthenticationTicket(1, inputUserName.Value, DateTime.Now, DateTime.Now.AddMinutes(30), inputRememberMe.Checked, "MJJames Admin Tool");
			
			cookiestr = FormsAuthentication.Encrypt(tkt);
			ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
			
			if (inputRememberMe.Checked)
			{
				ck.Expires = tkt.Expiration;
			}
			
			ck.Path = FormsAuthentication.FormsCookiePath;
			Response.Cookies.Add(ck);

			
			strRedirect = Request["ReturnUrl"];
			if (strRedirect == null)
			{
				strRedirect = "~/";
			}
			Response.Redirect(strRedirect, true);
		}
		else
		{
			lblMsg.Text = "System Login: Login Failed";
		}
	}
}
