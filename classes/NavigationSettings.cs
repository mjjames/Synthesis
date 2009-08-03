using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using log4net;
using mjjames.ControlLibrary.AdminWebControls;

namespace mjjames.AdminSystem.classes
{
	public class NavigationSettings
	{
		private ILog _logger = LogManager.GetLogger("SystemSettings");
		public AdminToolboxStateCollection GetSettings()
		{
			return AdminToolbox.GetConfig().adminControls;
		}
		
		public void Save(AdminToolboxStateCollection newsettings)
		{
			try
			{
				System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/");
				AdminToolbox toolbox = configuration.GetSection("mjjames/adminToolbox") as AdminToolbox;
				toolbox.adminControls = newsettings;
				foreach (AdminToolboxState setting in newsettings)
				{
					toolbox.adminControls.Add(setting);
				}
				configuration.Save();
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
