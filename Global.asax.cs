using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using mjjames.AdminSystem.classes;
using System.Web.UI;
using System.Web.Routing;
using mjjames.AdminSystem.App_Start;

namespace mjjames.AdminSystem
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.8.0.min.js",
                DebugPath = "https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.8.0.js",
                CdnPath = "http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.8.0.min.js",
                CdnSupportsSecureConnection = true,
                CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.8.0.js"
            });
            RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var error = sender as HttpApplication;
			if (error != null)
			{	
				var log = new Logger();
				log.LogError("Uncaught Appication Error: " + error.Context.Error.Message, error.Context.Error);
			}
		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}

		public override void Init()
		{
			base.Init();
			this.BeginRequest += new EventHandler(OnBeginRequest);
		}
		void OnBeginRequest(object sender, EventArgs e)
		{
			com.flajaxian.FileUploader.RegisterAspCookies();
		}
	}
}