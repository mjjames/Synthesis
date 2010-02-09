using System;

namespace mjjames.AdminSystem.classes
{
	interface ILogger
	{
		void LogError(string message, Exception exception);
		void LogInformation(string message);
		void LogDebug(string message);
	}
}
