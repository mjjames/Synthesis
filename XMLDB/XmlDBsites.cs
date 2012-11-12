using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataEntities;
using System.Web;

/// <summary>
/// Summary description for xmlDB
/// </summary>
/// 
namespace mjjames.AdminSystem
{
	public class XmlDBsites : XmlDBBase
	{

		public XmlDBsites()
		{
			//the site table cant be filtered by site_fkey so turn it off ;)
			MultiTenancyTableEnabled = false;
		}

		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{

			site ourSite = new site();
			if (PKey > 0)
			{
				ourSite = (from p in AdminDC.sites
						   where p.site_key == PKey
						   select p).SingleOrDefault();
			}

			return ourSite;
		}


		#endregion

		#region button events


		/// <summary>
		/// save away our data / insert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void SaveEdit(object sender, EventArgs e)
		{
			Button ourSender = (Button)sender;
			var idsThatCauseSiteMapCacheClear = new[] { "active" };
			var clearSiteMapCache = false;
			AdminDataContext ourPageDataContext = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			site ourData = new site();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.sites.Single(p => p.site_key == PKey);
			}

			foreach (AdminTab tab in Table.Tabs)
			{
				var ourTab = (WebControl)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab == null) continue;
				foreach (AdminField field in tab.Fields)
				{
					Control ourControl = ourTab.FindControl("control" + field.ID);

					if (ourControl == null) continue;
					PropertyInfo ourProperty = ourData.GetType().GetProperty(field.ID);
					if (ourProperty != null)
					{
						//get our new value
						var newValue = GetDataValue(ourControl, field.Type, ourProperty.PropertyType);
						//if we haven't already got a clear sitemap cache value and our current id is that of one we must check 
						//compare the old and new values and assign to clearSiteMap we only want true if the values aren't equal as thats a change
						if (!clearSiteMapCache && idsThatCauseSiteMapCacheClear.Contains(field.ID))
						{
							clearSiteMapCache = !newValue.Equals(ourProperty.GetValue(ourData, null));
						}
						ourProperty.SetValue(ourData, newValue, null);
					}
					else
					{
						Logger.LogError("Error Saving Content: " + ourControl.ID, new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				ourPageDataContext.sites.InsertOnSubmit(ourData);
			}

			Label labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				ChangeSet ourChanges = ourPageDataContext.GetChangeSet();

				labelStatus.Text = "Nothing to Save";
				ourPageDataContext.SubmitChanges();

				if (ourChanges.Inserts.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Inserted", Table.ID);


					PKey = ourData.site_key;

					string strPKeyField = TablePrimaryKeyField;

					HiddenField ourPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "pkey");
					HiddenField ourControlPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "control" + strPKeyField);

					try
					{
						ourControlPKey.Value = PKey.ToString();
						ourPKey.Value = PKey.ToString();
					}
					catch
					{
						Exception ex =
							new Exception(String.Format("{0} doesn't contain a hidden control called {1}", Table.ID, TablePrimaryKeyField));
						Logger.LogError("Unknown Field", ex);
						throw ex;
					}

					//now we have created our site automatically generate a "default" home page
					var defaultHomePage = new page()
					{
						site_fkey = PKey,
						page_fkey = 0,
						pageid = "HOME",
						navtitle = "home",
						title = "home",
						active = true,
						showinnav = true,
						page_url = "home"
					};
					ourPageDataContext.pages.InsertOnSubmit(defaultHomePage);

					//now add the current user as a siteadmin
					//lookup the userid 
					var userid = ourPageDataContext.aspnet_Users.FirstOrDefault(u => u.LoweredUserName == HttpContext.Current.User.Identity.Name.ToLower());
					//lookup the role id for the site admin
					var roleid = ourPageDataContext.aspnet_Roles.FirstOrDefault(r => r.LoweredRoleName == "site admin");
					if (userid != null && roleid != null)
					{
						var siteUserLevel = new site_user()
						{
							active = true,
							roleid = roleid.RoleId,
							site_fkey = PKey,
							userid = userid.UserId
						};
						ourPageDataContext.site_users.InsertOnSubmit(siteUserLevel);
					}
					else
					{
						Logger.LogError("Unable to assign user as siteadmin to new site: " + PKey, new Exception("Unable to find user or role: username: " + HttpContext.Current.User.Identity.Name + " Role: site admin"));
					}

					//now add the system admins to the site
					var systemAdmins = (from ur in ourPageDataContext.aspnet_UsersInRoles
										where ur.aspnet_Role.LoweredRoleName == "system admin"
										select new
										{
											roleid = ur.RoleId,
											userid = ur.UserId,
										}).ToArray();

					ourPageDataContext.site_users.InsertAllOnSubmit(systemAdmins.Select(p => new site_user
					{
						active = true,
						roleid = p.roleid,
						site_fkey = PKey,
						userid = p.userid

					}));
					ourPageDataContext.SubmitChanges();
				}
				if (ourChanges.Updates.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Updated", Table.ID);
				}

				//following an insert or an update to particular field we must reset a site's sitemap cache to allow our changes to pull through
				if (clearSiteMapCache)
				{
					GenericFunctions.ResetSiteMap();
				}

			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed", Table.ID);
				Logger.LogError("Failed Update", ex);
			}

		}

		#endregion

		public override void ArchiveData(int iKey)
		{
			throw new Exception("Site's can not be deleted as they are referenced by everything, please mark as inactive");
		}
	}

}