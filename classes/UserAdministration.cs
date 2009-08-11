using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Web.Security;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using mjjames.core;
using mjjames.core.dataentites;

namespace mjjames.AdminSystem.classes
{
	public class UserAdministration
	{
		private readonly AdminDataContext _adc = new AdminDataContext();

		public List<ApplicationUser> LoadUsers()
		{
			var users = from u in _adc.vw_aspnet_MembershipUsers
						join ur in _adc.vw_aspnet_UsersInRoles on u.UserId equals ur.UserId
						where
							u.ApplicationId.Equals(
							_adc.vw_aspnet_Applications.Single(a => a.ApplicationName.Equals("/")).ApplicationId)
						select new ApplicationUser
								{
									UserID = u.UserId,
									UserName = u.UserName,
									RoleID = ur.RoleId,
									LastLogin = u.LastLoginDate,
									LockedOut = u.IsLockedOut
								};

			return users.ToList();
		}

		public void UpdateUser(Guid userID, string userName, Guid roleID, bool lockedOut)
		{
			var user = (from u in _adc.aspnet_Users
						where u.UserId == userID
						select u).SingleOrDefault();

			if (lockedOut && !user.aspnet_Membership.IsLockedOut)
			{
				user.aspnet_Membership.LastLockoutDate = DateTime.Now;
			}
			user.aspnet_Membership.IsLockedOut = lockedOut;
			user.UserName = userName;

			//assuming that users can only have one role 
			_adc.aspnet_UsersInRoles_RemoveUsersFromRoles("/", user.UserName,
														  user.aspnet_UsersInRoles.First().aspnet_Role.RoleName);


			var role =
				_adc.aspnet_Roles.FirstOrDefault(
					r => r.RoleId == roleID);

			_adc.aspnet_UsersInRoles_AddUsersToRoles("/", user.UserName, role.RoleName, null);

			_adc.SubmitChanges();

		}

		public void DeleteUser(Guid userID)
		{
			var user = (from u in _adc.aspnet_Memberships
						where u.UserId == userID
						select u).SingleOrDefault();

			if (user != null)
			{
				_adc.aspnet_Memberships.DeleteOnSubmit(user);
			}

			_adc.SubmitChanges();

		}

		public void SetDefaultRole(string userName)
		{

			var user = (from u in _adc.aspnet_Users
						where u.UserName.Equals(userName)
						select u).SingleOrDefault();

			var role = (from r in _adc.vw_aspnet_Roles
						where r.RoleName == "editors"
						select r).SingleOrDefault();

			if (role == null || user == null) return;


			user.aspnet_UsersInRoles.Add(new aspnet_UsersInRole()
			{
				UserId = user.UserId,
				RoleId = role.RoleId
			});

			_adc.SubmitChanges();
		}

		public List<vw_aspnet_Role> LoadRoles()
		{
			return (from r in _adc.vw_aspnet_Roles
					where
					   r.ApplicationId.Equals(
					   _adc.vw_aspnet_Applications.Single(a => a.ApplicationName.Equals("/")).ApplicationId)
					select r).ToList();

		}

		public void SendResetPasswordEmail(string resetAddress, Guid userID)
		{
			var user = (from u in _adc.aspnet_Users
						where u.UserId.Equals(userID)
						select u).FirstOrDefault();

			if (String.IsNullOrEmpty(user.aspnet_Membership.Email)) return;

			string siteName = ConfigurationManager.AppSettings["SiteName"];

			string resetEmailMessage =
				String.Format(
					"<p>To reset your {0} Admin Tool Password please click the following link or copy and paste it into your browser. </p> <p>{1}</p> <p>Please note this link timeout in 24 hours</p>",
					siteName, resetAddress);

			email resetEmail = new email()
								{
									fromemail = ConfigurationManager.AppSettings["SiteFromEmail"],
									fromname = siteName,
									subject = String.Format("{0} Admin Tool - Reset Password", siteName),
									reciprients =
										new List<MailAddress>(new List<MailAddress>() { new MailAddress(user.aspnet_Membership.Email, user.UserName) }),
									body = resetEmailMessage
								};

			Emailer emailer = new Emailer()
								{
									Email = resetEmail
								};

			emailer.SendMail();
		}

		/// <summary>
		/// Resets the user's password
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="applicationID"></param>
		public bool ResetPassword(string userName, string newPassword)
		{
			try
			{
				// Woah dodgy stuff, ASP.Net won't let you change the password without knowing your old one, so instead you have to force it to
				// reset it to a random one and then set the new one ... cracking
				MembershipUser mu = Membership.GetUser(userName);
				mu.ChangePassword(mu.ResetPassword(), newPassword);
				return true;
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.Print(e.Message);
			}

			return false;
		}
		
		public string GenerateTimeOut()
		{
			string encTimeOut = helpers.EncodeTo64(DateTime.Now.AddDays(1).ToUniversalTime().ToString());
			return string.Format("h68{0}7h8", encTimeOut);
		}
		
		public string DecodeTimeOut(string encodedTimeOut)
		{
			return helpers.DecodeFrom64(encodedTimeOut.Substring(3, encodedTimeOut.Length - 6));
		}
		
	}
}
