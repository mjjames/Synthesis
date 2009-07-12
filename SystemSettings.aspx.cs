using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

public partial class SystemSettings : System.Web.UI.Page
{
	
	protected mjjames.AdminSystem.Configuration.SystemSettings _settings = new mjjames.AdminSystem.Configuration.SystemSettings();
	
	protected void Page_Load(object sender, EventArgs e)
	{

	}

	protected void loadTwitterSettings(object sender, EventArgs e)
	{
		ControlCollection controls = GetControlsContainer();
		settingLabel.InnerText = "Twitter Settings";
	}
	
	private ControlCollection GetControlsContainer()
	{
		ControlCollection controls = upSettings.ContentTemplateContainer.Controls;
		controls.Clear(); //clear previously loaded controls
		return controls;
	}
	
	public string GetSetting(string key)
	{
		return _settings[key] ?? String.Empty;
	}
	
	protected void LoadConfigSettings(object sender, EventArgs e)
	{
		settingLabel.InnerText = "Config Settings";
		panelConfig.Visible = true;
		info.Visible = false;
		panelConfig.DataBind();
	}

	protected void SaveConfig(object sender, EventArgs e)
	{	
		List<Control> controls = new List<Control>();

		foreach (TabPanel tab in settingsTabContainer.Controls )
		{
			foreach (Control control in tab.Controls[1].Controls) //tab panels are made of a header and a content template - content template = 1
			{
				if (control.GetType().Equals(typeof(TextBox)))
				{
					controls.Add(control);
				}	
			}			
		}

		foreach (var control in controls)
		{
			string key = control.ID.Substring(1).Replace("__", ":"); //note dirty hack on _ into : - stupid fcksettings
			_settings[key] = ((TextBox) control).Text;
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
}
