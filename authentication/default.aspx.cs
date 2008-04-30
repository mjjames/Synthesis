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
		/*
		SqlConnection conn;
		SqlCommand cmd;
		string lookupPassword = null;

		// Check for invalid userName.
		// userName must not be null and must be between 1 and 15 characters.
		if ((null == userName) || (0 == userName.Length) || (userName.Length > 15))
		{
			System.Diagnostics.Trace.WriteLine("[ValidateUser] Input validation of userName failed.");
			return false;
		}

		// Check for invalid passWord.
		// passWord must not be null and must be between 1 and 25 characters.
		if ((null == passWord) || (0 == passWord.Length) || (passWord.Length > 25))
		{
			System.Diagnostics.Trace.WriteLine("[ValidateUser] Input validation of passWord failed.");
			return false;
		}

		try
		{
			
			conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ourDatabase"].ToString());
			conn.Open();

			// Create SqlCommand to select pwd field from users table given supplied userName.
			cmd = new SqlCommand("Select password from adminusers where username=@userName", conn);
			cmd.Parameters.Add("@userName", SqlDbType.VarChar, 25);
			cmd.Parameters["@userName"].Value = userName;

			// Execute command and fetch pwd field into lookupPassword string.
			lookupPassword = (string)cmd.ExecuteScalar();

			// Cleanup command and connection objects.
			cmd.Dispose();
			conn.Dispose();
		}
		catch (Exception ex)
		{
			// Add error handling here for debugging.
			// This error message should not be sent back to the caller.
			System.Diagnostics.Trace.WriteLine("[ValidateUser] Exception " + ex.Message);
		}

		// If no password found, return false.
		if (null == lookupPassword)
		{
			System.Diagnostics.Trace.WriteLine("[Failed Logon] " + DateTime.Now.ToLocalTime().ToString());
			return false;
		}

		// Compare lookupPassword and input passWord, using a case-sensitive comparison.
		return (0 == string.Compare(lookupPassword, passWord, false));
		*/
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
			lblMsg.Text += "Login Failed";
		}
	}
}
