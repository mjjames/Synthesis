using System;

namespace mjjames.AdminSystem.classes
{
	public class ApplicationUser
	{
		public Guid UserID { get; set;}
		public string UserName { get; set;}
		public Guid RoleID { get; set;}
		public bool LockedOut { get; set;}
		public DateTime LastLogin { get; set;}
	}
}
