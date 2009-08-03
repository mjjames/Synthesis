using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System;
using System.Collections;
using System.Linq;
using System.Web.Configuration;
using log4net;

namespace mjjames.AdminSystem.Configuration
{
	public class SystemSettings
	{
		private KeyValueConfigurationCollection _settings;
		private System.Configuration.Configuration _configuration;
		private List<String> _keys;

		private ILog _logger = LogManager.GetLogger("SystemSettings");

		public SystemSettings()
		{

			_configuration = WebConfigurationManager.OpenWebConfiguration("/");
			AppSettingsSection appSettings = _configuration.GetSection("appSettings") as AppSettingsSection;
			if (appSettings == null)
			{
				throw new Exception("Invalid App Settings");
			}
			_settings = appSettings.Settings;
			_keys = _settings.AllKeys.ToList();
		}


		/// <summary>
		/// default named accessor
		/// </summary>
		/// <param name="key">Key of System Setting</param>
		/// <returns>Value of System Setting</returns>
		public string this[string key]
		{
			get
			{
				return _settings[key] != null ? _settings[key].Value : String.Empty;
			}
			set
			{
				//if the collection has the key update, if not add
				if (_keys.Contains(key))
				{
					_settings[key].Value = value;
				}
				else
				{
					//if we have an empty item don't add it
					if (String.IsNullOrEmpty(value))
					{
						return;
					}
					_settings.Add(key, value);
					_keys.Add(key);
				}

			}
		}

		/// <summary>
		/// removes a system setting
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			_settings.Remove(key);
			_keys.Remove(key);
		}

		/// <summary>
		/// Saves any app setting changes back to the config
		/// </summary>
		public void Save()
		{
			try
			{
				//due to .net only allowing add or remove operations, to update the app settings we have to remove them and then readd....
				string file = _configuration.AppSettings.SectionInformation.ConfigSource;
				_configuration.Sections.Remove("appSettings");
				AppSettingsSection appSettings = (AppSettingsSection)_configuration.Sections.Get("appSettings");
				appSettings.SectionInformation.ConfigSource = file;
				foreach (KeyValueConfigurationElement setting in _settings)
				{
					appSettings.Settings.Add(setting.Key, setting.Value);
				}
				//_configuration.Sections.Add("appSettings", appSettings);
				_configuration.Save();
				ConfigurationManager.RefreshSection("appSettings");
			}
			catch (Exception e)
			{
				_logger.Error(e);
				throw;
			}
		}
	}
}