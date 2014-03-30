using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mjjames.AdminSystem.Templates
{
    public class SiteUsersInsertTemplate : ITemplate
    {
        private DropDownList _users;
        private DropDownList _roles;
        public static string UserIdControlId = "UserId";
        public static string RoleNameControlId = "RoleName";
        public void InstantiateIn(Control container)
        {
            var usersDataSource = GetUsersDataSource();
            var rolesDataSource = GetRolesDataSource();
            _users = new DropDownList
                                            {
                                                DataSource = usersDataSource,
                                                DataTextField = "Name",
                                                DataValueField = "Id",
                                                ID = UserIdControlId
                                            };
            _roles = new DropDownList
                                            {
                                                DataSource = rolesDataSource,
                                                DataTextField = "Name",
                                                DataValueField = "Id",
                                                ID = RoleNameControlId
                                            };

            var row = new WebControl(HtmlTextWriterTag.Tr)
            {
                Controls ={
                            new WebControl(HtmlTextWriterTag.Td)
                            {
                                Controls = {
                                            _users
                                },
                            },
                            new WebControl(HtmlTextWriterTag.Td){
                                Controls = {
                                            _roles
                                }
                            },
                            new WebControl(HtmlTextWriterTag.Td){
                                Controls = {
                                    new LinkButton{
                                        CssClass = "btn btn-primary",
                                        Controls = {
                                            new WebControl(HtmlTextWriterTag.I){
                                                CssClass = "icon-white icon-plus"
                                            }
                                        },
                                        CommandName = "Insert",
                                        ToolTip = "Add User Role"
                                    }
                                }
                            }
                }
            };
            container.Controls.Add(row);
            container.Controls.Add(usersDataSource);
            container.Controls.Add(rolesDataSource);
        }

        private LinqDataSource GetRolesDataSource()
        {
            var dataSource = new LinqDataSource
            {
                ID = "RolesDataSource",
                EnableDelete = false,
                EnableInsert = false,
                EnableObjectTracking = false,
                EnableUpdate = false
            };
            dataSource.Selecting += (o, e) =>
            {
                e.Result = Roles.GetAllRoles().Select(r => new
                {
                    Id = r,
                    Name = r
                });
            };
            return dataSource;
        }

        private LinqDataSource GetUsersDataSource()
        {
            var dataSource = new LinqDataSource
            {
                ID = "UsersDataSource",
                EnableDelete = false,
                EnableInsert = false,
                EnableObjectTracking = false,
                EnableUpdate = false
            };
            dataSource.Selecting += (o, e) =>
            {
                e.Result = Membership.GetAllUsers().Cast<MembershipUser>().Select(u => new
                {
                    Name = u.UserName,
                    Id = u.ProviderUserKey
                });
            };
            return dataSource;
        }
    }
}