using System;
using System.Collections.Generic;
using mjjames.AdminSystem.DataEntities;

namespace mjjames.AdminSystem.Repositories
{
	public interface IDonation : IRepository<Donation>
	{
		IEnumerable<Donation> GetDonationsByDateRange(DateTime startDate, DateTime endDate);
		IEnumerable<Donation> GetDonationsByDonatee(Donatee donatee);
		
	}
}