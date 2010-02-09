using System;
using System.Configuration;
using log4net;

namespace mjjames.AdminSystem.classes
{
	public class Logger : ILogger
	{
		private readonly ILog _log;
		
		public Logger() : this("Not Set") {}
		
		public Logger(string className){
			string logName = string.Format("{0}.log", ConfigurationManager.AppSettings["SiteName"]);
			GlobalContext.Properties["LogName"] = logName;
			GlobalContext.Properties["Class"] = className;
			_log = LogManager.GetLogger("AdminSystemLogger");
		}
		
		#region ILogger Members

		public void LogError(string message, Exception exception)
		{
			_log.Error(message, exception);
		}

		public void LogInformation(string message)
		{
			_log.Info(message);
		}

		public void LogDebug(string message)
		{
			_log.Debug(message);
		}

		#endregion
	}
}
