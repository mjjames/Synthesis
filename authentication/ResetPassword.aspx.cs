using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using mjjames.core;

namespace mjjames.AdminSystem.Authentication
{
	public partial class ResetPassword : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if(!ValidateRequest()) return;
			_ChangePassword.Visible = true;
            Title = "Reset Password";
		}
		
		private bool ValidateRequest()
		{
			if(String.IsNullOrEmpty(Request["v"]) || String.IsNullOrEmpty(Request["u"])) Response.Redirect("~/", true);
			
			UserAdministration ua = new UserAdministration();

			DateTime timeOut;


			if (!DateTime.TryParse(ua.DecodeTimeOut(Request["v"]), out timeOut) || timeOut <= DateTime.Now.ToUniversalTime())
			{
				_Status.Text =
					"<p> This Password Reset Request has passed the reset time out </p><p> please contact your system administrator for a new reset request</p>";
				return false;
			}

			aspnet_User user = null;
			
			if(helpers.IsGuid(Request["u"]))
			{
				Guid userID = new Guid(Request["u"]);

				user = (from u in new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString).aspnet_Users
				            where u.UserId.Equals(userID)
				            select u).SingleOrDefault();
			}
			
			if(user == null)
			{
				_Status.Text =
					"<p>Error - Invaild user provided</p><p> please contact your system administrator for a new reset request</p>";
				return false;
			}
			
			return true;
		}

		protected void SavePassword(object sender, EventArgs e)
		{
			if(!Page.IsValid)
			{
				_Summary.Visible = true;
				return;
			}
			UserAdministration ua = new UserAdministration();

			var user = (from u in new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString).aspnet_Users
			           where u.UserId.Equals(new Guid(Request["u"]))
			           select u).First();
			
			
			bool resetSuccess  = ua.ResetPassword(user.UserName, _txtPassword.Text);
			
			if(resetSuccess)
			{
				_Status.Text = "<p>Your password was successfully changed, please <a href=\"default.aspx\">continue to login</a></p>";
				_ChangePassword.Visible = false;
				return;
			}

			_Status.Text =
				"<p>Sorry an Error Occurred changing your password</p><p>Please try again or contact your system administrator</p>";
			
		}
	}
}
