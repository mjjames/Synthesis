using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Dimebrain.TweetSharp.Model;
using mjjames.AdminSystem.classes;
using mjjames.ControlLibrary.AdminWebControls;
using System.Linq;

namespace mjjames.AdminSystem
{
	public partial class SystemSettings : Page
	{

		protected Configuration.SystemSettings _settings = new Configuration.SystemSettings();
		private TwitterAuth _twitterAuth;
		private readonly TwitterAuthentication _twitterAuthentication = new TwitterAuthentication(ConfigurationManager.AppSettings["twitterConsumerKey"], ConfigurationManager.AppSettings["twitterConsumerSecret"]);
		private readonly NavigationSettings _navsettings = new NavigationSettings();
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public string GetSetting(string key)
		{
			return _settings[key] ?? String.Empty;
		}

		protected bool GetSettingAsBool(string key)
		{
			bool value;
			bool.TryParse(_settings[key], out value);
			return value;
		}

		protected void SetSelectedValue(object sender, EventArgs e)
		{
			DropDownList ddl = (DropDownList) sender;
			
			string value =_settings[ddl.ID.Substring(1).Replace("__", ":")];

			foreach (ListItem item in ddl.Items)
			{
				item.Selected = item.Value.Equals(value);
			}
			
		}

		protected void LoadConfigSettings(object sender, EventArgs e)
		{
			settingLabel.InnerText = "Config Settings";
			HidePanels();
			panelConfig.Visible = true;
			panelConfig.DataBind();
		}
		
		protected void SetAdminToolbar(object sender, EventArgs e)
		{
			_navItems.DataSource = _navsettings.GetSettings();
		}
		
		protected void SetAccessLevel(object sender, EventArgs e)
		{
			DropDownList ddl = (DropDownList) sender;
			AdminToolboxState state = ((AdminToolboxState) ((ListViewDataItem) ddl.Parent).DataItem);
			string value = state.name + "-|-" + state.accesslevel.ToString();
			foreach (ListItem item in ddl.Items)
			{
				item.Value = state.name + "-|-" + item.Value; //stashing the field name in the value, bit dirty but hey
				
				if (!item.Value.Equals(value)) continue;
				item.Selected = true;
			}
		}

		/// <summary>
		/// Hides all the update panels
		/// </summary>
		private void HidePanels()
		{
			panelInfo.Visible = false;
			panelConfig.Visible = false;
			panelTwitter.Visible = false;
			panelNavigation.Visible = false;
		}

		protected void SaveConfig(object sender, EventArgs e)
		{
			List<Control> controls = new List<Control>();

			foreach (TabPanel tab in settingsTabContainer.Controls)
			{
				foreach (Control control in tab.Controls[1].Controls) //tab panels are made of a header and a content template - content template = 1
				{
					if (control.GetType().Equals(typeof(TextBox)) 
						|| control.GetType().Equals(typeof(CheckBox)) 
						|| control.GetType().Equals(typeof(DropDownList)))
					{
						controls.Add(control);
					}
				}
			}

			foreach (var control in controls)
			{
				string key = control.ID.Substring(1).Replace("__", ":"); //note dirty hack on _ into : - stupid fcksettings
				switch (control.GetType().Name)
				{
					case "TextBox":
						_settings[key] = ((TextBox) control).Text;
						break;
					case "CheckBox":
						_settings[key] = ((CheckBox) control).Checked.ToString();
						break;
					case "DropDownList":
						_settings[key] = ((DropDownList) control).SelectedValue;
						break;
				}
			}

			try
			{
				_settings.Save();
				configStatus.Text = "Config Settings Saved";
			}
			catch (Exception)
			{
				configStatus.Text = "Sorry an Error Occurred Whilst Saving the Config Settings";
			}
		}

		protected void LoadTwitterAuthentication(object sender, EventArgs e)
		{
			settingLabel.InnerText = "Twitter Authentication";
			HidePanels();
			panelTwitter.Visible = true;
			LoadTwitter();
		}
		protected void twitterVerify(object sender, EventArgs e)
		{
			string pin = twitterPin.Text;
			_twitterAuth = _twitterAuthentication.FinishAuthenticate(pin, Session["twittertoken"].ToString());
			
			if (_twitterAuth.Error != null)
			{
				litStatus.Text = _twitterAuth.Error.ErrorMessage;
				return;
			}
			panelAuthenticate.Visible = false;
			panelAuthenticated.Visible = true;
			_litTwitterScreenName.Text = _twitterAuth.User.ScreenName;
			
			Session.Remove("twittertoken");

			//persist OAuth token
			_settings["twitterAuthenticationToken"] = _twitterAuth.AccessToken.Token;
			_settings["twitterAuthenticationTokenSecret"] = _twitterAuth.AccessToken.TokenSecret;
			_settings.Save();
		}

		protected void CheckTwitterSettings(object sender, EventArgs e)
		{
			LoadTwitter();
		}
		
		private void LoadTwitter()
		{
			if (!panelTwitter.Visible || Session["twittertoken"] != null) // why work if we aren't visible
			{
				return;
			}
			if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["twitterConsumerKey"]) ||
				string.IsNullOrEmpty(ConfigurationManager.AppSettings["twitterConsumerSecret"]))
			{
				upTwitter.Visible = false;
				_InvalidTwitterSettings.Visible = true;
				return;
			}

			upTwitter.Visible = true;
			_InvalidTwitterSettings.Visible = false;

			if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["twitterAuthenticationToken"])
				&& !String.IsNullOrEmpty(ConfigurationManager.AppSettings["twitterAuthenticationTokenSecret"]))
			{
				LoadTwitterUser();
				panelAuthenticated.Visible = true;
				return;
			}

			panelAuthenticate.Visible = true;
			SetupTwitterAuthentication();
		}
		
		
		private void SetupTwitterAuthentication()
		{
			Session.Add("twittertoken", _twitterAuthentication.GetToken());
			hlAuthenticate.NavigateUrl = _twitterAuthentication.BeginAuthenticate();
			hlAuthenticate.Target = "_blank"; //nasty 

		}

		private void LoadTwitterUser()
		{
			//if we have a screen name why work when we don't need to
			if(!String.IsNullOrEmpty(_litTwitterScreenName.Text))
			{
				return;
			}
			OAuthToken token = new OAuthToken()
			                   	{
			                   		Token =ConfigurationManager.AppSettings["twitterAuthenticationToken"],
			                   		TokenSecret = ConfigurationManager.AppSettings["twitterAuthenticationTokenSecret"]
			                   	};
			TwitterAuth auth = _twitterAuthentication.GetAuthenticatedUser(token);
			if(auth.Error !=null)
			{
				litStatus.Text = auth.Error.ErrorMessage;
				return;
			}
			_litTwitterScreenName.Text = auth.User.ScreenName;
			
		}


		protected void RemoveTwitterAuthentication(object sender, EventArgs e)
		{
			_settings.Remove("twitterAuthenticationToken");
			_settings.Remove("twitterAuthenticationTokenSecret");
			_settings.Save();

			litStatus.Text = "Twitter Authentication Removed";
						
			panelAuthenticated.Visible = false;
			SetupTwitterAuthentication();
			panelAuthenticate.Visible = true;
		}


		protected void LoadNavigation(object sender, EventArgs e)
		{
			settingLabel.InnerText = "Navigation Settings";
			HidePanels();
			
			
			_navItems.DataBind();
			panelNavigation.Visible = true;
		}

		protected void SaveNavigationSettings(object sender, EventArgs e)
		{
			AdminToolboxStateCollection newsettings = new AdminToolboxStateCollection();
			foreach (var setting in _navItems.Items)
			{
				int newValue = 0;
				string key = String.Empty;
				
				foreach (var control in setting.Controls)
				{
					DropDownList ddl = control as DropDownList;
					
					if(ddl == null) continue;
					newValue = int.Parse(ddl.SelectedValue.Substring(ddl.SelectedValue.IndexOf("-|-") + 3)); //the +3 is the length of what we are matching
					key = ddl.SelectedValue.Substring(0, ddl.SelectedValue.IndexOf("-|-"));
				}
				var url = _navsettings.GetSettings().Cast<AdminToolboxState>().FirstOrDefault(ats => ats.name.Equals(key)).url;
				AdminToolboxState toolboxitem = new AdminToolboxState()
				                                	{
				                                		name = key,
				                                		accesslevel = newValue,
				                                		url = url
				                                	};
				                                	
				newsettings.Add(toolboxitem);
			}

			_navsettings.Save(newsettings);
		}
	}
}