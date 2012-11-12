using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mjjames.AdminSystem.install
{
    public partial class install : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var context = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            //if we have sites our job is done
            if (context.sites.Any())
            {
                configureDB.Visible = false;
                installAlreadyComplete.Visible = true;
                return;
            }

            if (!IsPostBack)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(siteName.Text) || string.IsNullOrWhiteSpace(siteURL.Text) || string.IsNullOrWhiteSpace(email.Text))
            {
                ErrorMessage.Text = "<h3 class=\"text-error\">Error: Please ensure all fields are completed</h3>";
                return;
            }

            var url = siteURL.Text.ToLower();
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            var site = new site
            {
                active = true,
                hostname = url,
                name = siteName.Text,
                pages =
                {
                    new page{
                        page_fkey = 0,
						pageid = "HOME",
						navtitle = "home",
						title = "home",
						active = true,
						showinnav = true,
						page_url = "home"
                    }
                }
            };

            var generatedPassword = Membership.GeneratePassword(8, 2);
            var user = Membership.CreateUser(email.Text, generatedPassword, email.Text);
            Roles.AddUserToRole(email.Text, "System Admin");

            site.site_users.Add(new site_user
            {
                active = true,
                aspnet_Role = context.aspnet_Roles.FirstOrDefault(r => r.LoweredRoleName == "system admin"),
                aspnet_User = context.aspnet_Users.FirstOrDefault(u => u.UserName == email.Text)
            });

            context.sites.InsertOnSubmit(site);
            context.SubmitChanges();

            logonPassword.Text = generatedPassword;
            logonUsername.Text = email.Text;

            configureDB.Visible = false;
            installComplete.Visible = true;
        }
    }
}