using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using log4net;

namespace mjjames.AdminSystem.Repositories
{
	public class DonationsRepository : IDonation
	{
		private readonly AdminDataContext _adc = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		#region IDonation Members

		public IEnumerable<Donation> GetDonationsByDateRange(DateTime startDate, DateTime endDate)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Donation> GetDonationsByDonatee(Donatee donatee)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IRepository<Donation> Members

		public Donation GetByKey(int id)
		{
			throw new NotImplementedException();
		}

		public void Add(Donation entity)
		{
			throw new NotImplementedException();
		}

		public void Remove(Donation entity)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Donation> GetAll()
		{
			throw new NotImplementedException();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
