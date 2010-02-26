using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mjjames.AdminSystem.DataEntities;
using mjjames.AdminSystem.classes;
using System.Configuration;

namespace mjjames.AdminSystem.dao
{
    public class SiteUsersRepository
    {
		private readonly ILogger _logger = new Logger("SiteUsersRepository");
		private readonly DataContexts.AdminDataContext _admin = new DataContexts.AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);

		public IQueryable<site> GetAcitveSitesForUser(string userName){
			var sites = from s in _admin.site_users
						where s.aspnet_User.UserName == userName
						&& s.active
						select s.site;
			return sites;
		}
    }
}