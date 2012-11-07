using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;

namespace mjjames.AdminSystem
{
	public partial class DBUsers : System.Web.UI.Page
	{
		private readonly AdminDataContext _adc =new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		private List<vw_aspnet_Role> _roles;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (User.IsInRole("Site Admin") || User.IsInRole("System Admin")) return;
			dbuserDropDownList.Visible = false;
			labelDBUser.Visible = false;

            Title = "User Administration";
		}

		protected void ChangeView(object sender, EventArgs e)
		{
			DropDownList dbuserList = (DropDownList)sender;
			ChangePassword.Visible = false;
			CreateUserWizard.Visible = false;
			_UserLising.Visible = false;
		    errorMessage.Text = "";
			switch (dbuserList.SelectedValue)
			{
				case "createUser":
					CreateUserWizard.Visible = true;
					settingLabel.Text = "Create New User";
					break;
				case "manageUser":
					_UserLising.Visible = true;
					settingLabel.Text = "Manage Users";
					LoadUsers();
					break;

				default:
					ChangePassword.Visible = true;
					settingLabel.Text = "Change Your Password";
					break;
			}
		}

		protected void LoadUsers()
		{
			errorMessage.Text = "";
			UserAdministration ua = new UserAdministration();

			_UserLising.DataSource = ua.LoadUsers();
			_UserLising.DataKeyNames = new[] { "UserID" };
			_UserLising.DataBind();
		}

		protected void LoadRoleData(object sender, EventArgs e)
		{
			if (!_UserLising.Visible) return;
			if (_roles == null)
			{
				UserAdministration ua = new UserAdministration();
				_roles = ua.LoadRoles();
			}
			DropDownList ddl = sender as DropDownList;
			if (ddl == null) return;

			ddl.DataSource = _roles;
			ddl.DataTextField = "RoleName";
			ddl.DataValueField = "RoleID";
		}

		protected void SetRole(object sender, EventArgs e)
		{
			DropDownList ddl = sender as DropDownList;
			if (ddl == null) return;
			ListViewDataItem lvdl = ddl.Parent as ListViewDataItem;
			if (lvdl == null) return;
			ApplicationUser user = lvdl.DataItem as ApplicationUser;
			if (user == null) return;
			ListItem item = ddl.Items.OfType<ListItem>().SingleOrDefault(i => i.Value.Equals(user.RoleID.ToString()));
			if (item == null) return;
			item.Selected = true;
		}

		protected void EditDataBind(object sender, ListViewEditEventArgs e)
		{
			if (!(e.NewEditIndex >= 0)) return;
			LoadUsers();
			ListView lv = (ListView)sender;
			lv.EditIndex = e.NewEditIndex;
			lv.DataBind();
		}

		protected void UpdateUsers(object sender, ListViewUpdateEventArgs e)
		{
			/*
			 *	A few notes -
			 *	due to the way im loading the data source none of the event args are being populated
			 *	as a result everything is manual ;(
			 *	need to at some point find out why
			 */
			ListView lv = ((ListView)sender);

			Guid userID = (Guid)lv.DataKeys[e.ItemIndex].Value;
			bool lockedOut = ((CheckBox)lv.EditItem.FindControl("_lockedOut")).Checked;
			string userName = ((TextBox)lv.EditItem.FindControl("_userName")).Text;
			Guid roleID = new Guid(((DropDownList)lv.EditItem.FindControl("_roles")).SelectedValue);

			UserAdministration ua = new UserAdministration();

			ua.UpdateUser(userID, userName, roleID, lockedOut);

			lv.EditIndex = -1;
			LoadUsers();

		}

		protected void DeleteUser(object sender, ListViewDeleteEventArgs e)
		{
			ListView lv = ((ListView)sender);

			Guid userID = (Guid)lv.DataKeys[e.ItemIndex].Value;

			UserAdministration ua = new UserAdministration();

			ua.DeleteUser(userID);

			LoadUsers();
		}

		protected void SetDefaultRole(object sender, EventArgs e)
		{

			string userName = this.CreateUserWizard.UserName;
			UserAdministration ua = new UserAdministration();
			ua.SetDefaultRole(userName);

		}

		protected void ResetPassword(Guid userID)
		{
			UserAdministration ua = new UserAdministration();
			
			string hostName = Request.Url.Host;
			string timeOut = ua.GenerateTimeOut();
            
			string resetURL = String.Format("http://{0}/{1}?u={1}&v={2}", hostName, this.ResolveUrl("~/authentication/resetPassword.aspx"), userID, timeOut);
			errorMessage.Text = ua.SendResetPasswordEmail(resetURL, userID) ? "Password Reset Email Sent" : "Error: Unable to send password reset email <br /> Please ensure the user has a valid email address";
		}

        protected void CancelEdit(object sender, ListViewCancelEventArgs e)
        {
            _UserLising.EditIndex = -1;
			LoadUsers();
        }

		/// <summary>
		/// Handles event for custom commands
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CustomCommands(object sender, ListViewCommandEventArgs e)
		{

			ListViewDataItem dataItem = (ListViewDataItem)e.Item;
			Guid userID = (Guid)_UserLising.DataKeys[dataItem.DisplayIndex].Value;
			switch (e.CommandName)
			{
				case "ResetPassword":
					ResetPassword(userID);
					break;
				default:
					break;
			}
		}
	}
}