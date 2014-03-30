using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace mjjames.AdminSystem.dataControls
{
    public class SiteusersControl : IDataControl
    {
        public int PKey
        {
            get;
            set;
        }

        public System.Web.UI.Control GenerateControl(dataentities.AdminField field, object ourPage)
        {
            ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);

            UpdatePanel lookupPanel = new UpdatePanel { ChildrenAsTriggers = true };

            if (PKey == 0)
            {
                lookupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl
                        {
                            Text = "<p class=\"information\">Please save your changes before adding any users to your site</p>",
                            ID = "control" + field.ID
                        });
            }
            else
            {
                var dataSource = GetDataSource();
                var listing = GetListing(field);
                listing.DataSource = dataSource;
                lookupPanel.ContentTemplateContainer.Controls.Add(dataSource);
                lookupPanel.ContentTemplateContainer.Controls.Add(listing);
                lookupPanel.DataBind();



                if (ourSM != null) ourSM.RegisterAsyncPostBackControl(listing);
            }

            return lookupPanel;
        }

        private ListView GetListing(dataentities.AdminField field)
        {
            var listing = new ListView
            {
                ID = "control" + field.ID,
                EnableViewState = false,
                //CssClass = "listingTable table table-bordered table-hover table-striped",
                //DataKeyNames = new[] {"siteuser_key" },
                LayoutTemplate = new ListViewTableLayout(new[] { "User", "Role", "" }),
                ItemTemplate = new ListViewTableItemTemplate(new[] { "UserName", "RoleName" }, "siteuser_key"),
                InsertItemPosition = InsertItemPosition.LastItem,
                InsertItemTemplate = new SiteUsersInsertTemplate()
            };
            listing.ItemInserting += listing_ItemInserting;
            listing.ItemDeleting += listing_ItemDeleting;
            return listing;
        }

        private void listing_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            //TODO: the eventargs has no values but has the index of the item within the sql data source
            //however... if we stop using the sql data source this would no longer be the case. PONDER
            //e.Cancel = true;
            var ds = (System.Data.DataView)((SqlDataSource)((ListView)sender).DataSource).Select(DataSourceSelectArguments.Empty);
            var row = ds[e.ItemIndex];
            var key = row.Row.ItemArray[0];

            var context = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            var userToDelete = context.site_users.First(su => su.siteuser_key == (int)key);
            context.site_users.DeleteOnSubmit(userToDelete);
            context.SubmitChanges();
            ((ListView)sender).DataBind();
        }

        void listing_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            var userId = ((DropDownList)e.Item.FindControl(SiteUsersInsertTemplate.UserIdControlId)).SelectedValue;
            var roleName = ((DropDownList)e.Item.FindControl(SiteUsersInsertTemplate.RoleNameControlId)).SelectedValue;
            //MaybeTodo: Use a nice class ;0
            var context = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
            var role = context.aspnet_Roles.First(r => r.LoweredRoleName == roleName.ToLower());
            context.site_users.InsertOnSubmit(new DataEntities.site_user
            {
                site_fkey = PKey,
                userid = Guid.Parse(userId),
                active = true,
                roleid = role.RoleId
            });
            context.SubmitChanges();
            ((ListView)sender).DataBind();
        }

        private SqlDataSource GetDataSource()
        {
            return new SqlDataSource()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString,
                SelectCommand = "SELECT   [siteuser_key] " +
                                        ",[site_fkey] " +
                                        ",u.[userid] " +
                                        ",u.[UserName] " +
                                        ",r.[roleid] " +
                                        ",r.[RoleName] " +
                                "FROM [dbo].[site_users] su " +
                                "inner join " +
                                "  [dbo].[aspnet_Users] u " +
                                "  on u.UserId = su.UserId " +
                                "inner join " +
                                "  [dbo].[aspnet_Roles] r" +
                                "  on r.RoleId = su.roleid " +
                                "where [site_fkey] = @Site_FKey and active = 1",
                SelectParameters =
                {
                    new Parameter("Site_FKey", System.Data.DbType.Int32, "" + PKey)
                }
            };
        }
    }
}