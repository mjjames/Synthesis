using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;

namespace mjjames.AdminSystem.authentication
{
	public partial class Login : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "System Login";
		}
		/// <summary>
		/// Validate Our User
		/// </summary>
		/// <param name="userName">Username</param>
		/// <param name="passWord">password</param>
		/// <returns></returns>
		private static bool ValidateUser(string userName, string passWord)
		{
			return Membership.ValidateUser(userName, passWord);
		}

		/// <summary>
		/// Login Button Handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnLogin_Click(object sender, EventArgs e)
		{
			if (ValidateUser(inputUserName.Value, inputPassword.Value))
			{
				FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, inputUserName.Value, DateTime.Now, DateTime.Now.AddMinutes(30), false, "Synthesis");
			
				string cookiestr = FormsAuthentication.Encrypt(tkt);
				HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
			
				ck.Path = FormsAuthentication.FormsCookiePath;
				Response.Cookies.Add(ck);

			
				string strRedirect = Request["ReturnUrl"] ?? "~/";
				Response.Redirect(strRedirect, true);
			}
			else
			{
				lblMsg.Text = "System Login: <span class=\"text-error\">Login Failed</span>";
			}
		}
	}
}