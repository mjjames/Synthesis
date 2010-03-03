using System;
using System.Linq;
using mjjames.AdminSystem.classes;
using System.Configuration;

namespace mjjames.AdminSystem.Repositories
{
	public class KeyValueRepository
	{
		private readonly ILogger _logger = new Logger("KeyValueRepository");
		private readonly DataContexts.AdminDataContext _admin = new DataContexts.AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);


		public string GetKeyValue(string lookupID, int linkKey, string linktypeID)
		{
			var data = (from kv in _admin.keyvalues
			            where kv.lookup.lookup_id == lookupID
			                  && kv.link_fkey == linkKey
			                  && kv.lookup1.lookup_id == linktypeID
			            select kv).FirstOrDefault();
			return data != null ? data.value : String.Empty;
		}
	}
}