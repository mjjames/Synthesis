using System.Configuration;

namespace mjjames.AdminSystem.DataContexts
{
	partial class AdminDataContext
	{
		partial void OnCreated()
		{
			base.Connection.ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString;
		}
	}
}
