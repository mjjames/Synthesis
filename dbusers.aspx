<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
    Inherits="mjjames.AdminSystem.DBUsers" Title="User Administration" CodeBehind="DBUsers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
        <CompositeScript>
            <Scripts>
                <asp:ScriptReference Name="MicrosoftAjax.js" />
                <asp:ScriptReference Name="MicrosoftAjaxWebForms.js" />
            </Scripts>
        </CompositeScript>
    </asp:ScriptManager>
    <div class="container">
        <div class="page-header">
            <h1 class="listingTitle">User Administration
            </h1>
        </div>
        <div class="row">

            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <div id="leftCol" runat="server" class="span3">
                        <h2>Available Options
                        </h2>
                        <asp:Label ID="labelDBUser" runat="server" AssociatedControlID="dbuserDropDownList">Edit mode:</asp:Label>
                        <asp:DropDownList ID="dbuserDropDownList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ChangeView">
                            <asp:ListItem Value="createUser">Create User</asp:ListItem>
                            <asp:ListItem Selected="True" Value="changePassword">Change Password</asp:ListItem>
                            <asp:ListItem Value="manageUser">Manage Users</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div id="rightCol" class="span9 form-horizontal">
                        <h2>
                            <asp:Literal runat="server" ID="settingLabel" Text="Change Password" />
                        </h2>
                        <asp:CreateUserWizard ID="CreateUserWizard" runat="server" Visible="false" OnCreatedUser="SetDefaultRole"
                            LoginCreatedUser="false" CreateUserButtonStyle-CssClass="btn btn-primary btn-large">
                            <WizardSteps>
                                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                                    <ContentTemplate>
                                        <div class="control-group">
                                            <asp:Label runat="server" AssociatedControlID="UserName" CssClass="control-label">
												Username:
                                            </asp:Label>
                                            <div class="controls">
                                                <asp:TextBox runat="server" ID="UserName" CssClass="field" />
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="UserName"
                                                    ErrorMessage="Username is required." CssClass="validation" Display="dynamic">*
                                                </asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <asp:Label runat="server" CssClass="control-label" AssociatedControlID="Password">
												Password:
                                            </asp:Label>
                                            <div class="controls">
                                                <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator10" ControlToValidate="Password"
                                                    ErrorMessage="Password is required." CssClass="validation" Display="dynamic">*
                                                </asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <asp:Label runat="server" CssClass="control-label" AssociatedControlID="ConfirmPassword">
												Confirm Password:
                                            </asp:Label>
                                            <div class="controls">
                                                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator13" ControlToValidate="ConfirmPassword"
                                                    ErrorMessage="Confirm Password is required." CssClass="validation" Display="Static">*
                                                </asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <asp:Label runat="server" CssClass="control-label" AssociatedControlID="Email">
												Email:
                                            </asp:Label>
                                            <div class="controls">
                                                <asp:TextBox runat="server" ID="Email" CssClass="wideField" />
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator11" ControlToValidate="Email"
                                                    ErrorMessage="Email is required." CssClass="validation" Display="dynamic">*
                                                </asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator runat="server" ID="emailValidation" ControlToValidate="Email"
                                                    ErrorMessage="Please Provide a Valid Email"
                                                    ValidationExpression="[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?" />
                                            </div>
                                        </div>
                                        <span class="row hidden">
                                            <asp:Label runat="server" CssClass="label" AssociatedControlID="Question">
												Question:
                                            </asp:Label>
                                            <asp:TextBox runat="server" ID="Question" />
                                        </span>
                                        <span class="row hidden">
                                            <asp:Label runat="server" CssClass="label" AssociatedControlID="Answer">
												Answer:
                                            </asp:Label>
                                            <asp:TextBox runat="server" ID="Answer" />
                                        </span>
                                        <div class="control-group">
                                            <div class="controls">
                                                <asp:RegularExpressionValidator runat="server" ID="_regexPassword" ControlToValidate="Password"
                                                    SetFocusOnError="true" ValidationExpression='^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$'
                                                    ErrorMessage="Error: Your password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character, Valid special characters." />
                                                <br />
                                                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                                    ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."></asp:CompareValidator>

                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <div class="controls">
                                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                                            </div>
                                        </div>
                                        </table>
                                    </ContentTemplate>
                                </asp:CreateUserWizardStep>
                                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                                </asp:CompleteWizardStep>
                            </WizardSteps>
                        </asp:CreateUserWizard>
                        <asp:ChangePassword ID="ChangePassword" runat="server">
                            <ChangePasswordTemplate>
                                <div class="control-group">

                                    <asp:Label runat="server" ID="_currentPassword" CssClass="control-label" AssociatedControlID="CurrentPassword">
									Current Password
                                    </asp:Label>

                                    <div class="controls">
                                        <asp:TextBox runat="server" ID="CurrentPassword" CssClass="field" TextMode="Password" />
                                    </div>
                                </div>
                                <div class="control-group">

                                    <asp:Label runat="server" ID="_lblPassword" CssClass="control-label" AssociatedControlID="NewPassword">New Password:</asp:Label>
                                    <div class="controls">
                                        <asp:TextBox runat="server" ID="NewPassword" CssClass="field" AutoCompleteType="Disabled"
                                            TextMode="Password" />
                                        <asp:RequiredFieldValidator ID="_valRequired" runat="server" ControlToValidate="NewPassword"
                                            ErrorMessage="* You must supply a new password" Display="dynamic">*
                                        </asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <asp:Label runat="server" ID="_lblConfirmPassword" CssClass="control-label" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                                    <div class="controls">
                                        <asp:TextBox runat="server" ID="ConfirmNewPassword" CssClass="field" AutoCompleteType="Disabled"
                                            TextMode="Password" />
                                        <asp:RequiredFieldValidator ID="_RequiredFieldValidator1" runat="server" ControlToValidate="ConfirmNewPassword"
                                            ErrorMessage="* You must confirm your new password" Display="dynamic">*
                                        </asp:RequiredFieldValidator>
                                        <asp:CompareValidator runat="server" ID="_comparePassword" ControlToCompare="NewPassword"
                                            Display="dynamic" ControlToValidate="ConfirmNewPassword" EnableClientScript="true"
                                            Operator="Equal" Text="Please ensure both passwords are the same" />
                                    </div>
                                </div>
                                <div class="control-group">
                                    <div class="controls">
                                        <asp:Button runat="server" CssClass="btn btn-primary btn-large" ID="ChangePassword" CommandName="ChangePassword" Text="Change Password" />
                                    </div>
                                </div>
                                <div class="control-group">
                                    <div class="controls">
                                        <asp:RegularExpressionValidator runat="server" ID="_regexPassword" ControlToValidate="NewPassword"
                                            Display="Dynamic"
                                            SetFocusOnError="true" ValidationExpression='^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$'
                                            ErrorMessage="Error: Your password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character, Valid special characters." />
                                        <br />

                                        <asp:ValidationSummary runat="server" ID="_Summary" DisplayMode="BulletList" />
                                        <asp:Literal runat="server" ID="FailureText" Text="" />
                                    </div>
                                </div>
                            </ChangePasswordTemplate>
                            <SuccessTemplate>
                                <p>Your Password Has Successfully Been Changed</p>
                            </SuccessTemplate>
                        </asp:ChangePassword>
                        <asp:ListView runat="server" ID="_UserLising" Visible="false" OnItemEditing="EditDataBind"
                            OnItemUpdating="UpdateUsers" OnItemDeleting="DeleteUser" OnItemCanceling="CancelEdit" OnItemCommand="CustomCommands">
                            <LayoutTemplate>
                                <table class="listingTable table table-striped table-bordered table-hover">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>UserName
                                            </th>
                                            <th>Role
                                            </th>
                                            <th>Locked Out?
                                            </th>
                                            <th>Last Login Date
                                            </th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr runat="server" id="itemPlaceHolder" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr class="pageListRow">
                                    <td>
                                        <asp:LinkButton runat="server" ID="_edit" CommandName="Edit" Text="Edit" CssClass="btn btn-small btn-primary" />
                                    </td>
                                    <td>
                                        <asp:Literal runat="server" ID="_username" Text='<%# Eval("UserName") %>' />
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="_roles" OnInit="LoadRoleData" OnDataBound="SetRole"
                                            Enabled="false" />
                                    </td>
                                    <td>
                                        <asp:CheckBox runat="server" ID="_lockedOut" Checked='<%# Eval("LockedOut") %>' Enabled="false" />
                                    </td>
                                    <td>
                                        <asp:Literal runat="server" ID="_lastLogin" Text='<%# Eval("LastLogin")%>' />
                                    </td>
                                    <td>
                                        <asp:LinkButton runat="server" ID="_resetPassword" CommandName="ResetPassword" Text="Reset Password" CssClass="btn btn-small  btn-warning" />
                                        <asp:LinkButton runat="server" ID="_deleteUser" CommandName="Delete" Text="Delete" CssClass="btn btn-small  btn-danger" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:TextBox runat="server" ID="_username" Text='<%# Bind("UserName") %>' />
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="_roles" OnInit="LoadRoleData" OnDataBound="SetRole" />
                                    </td>
                                    <td>
                                        <asp:CheckBox runat="server" ID="_lockedOut" Checked='<%# Eval("LockedOut") %>' />
                                    </td>
                                    <td>
                                        <asp:Literal runat="server" ID="_lastLogin" Text='<%# Eval("LastLogin")%>' />
                                    </td>
                                    <td>
                                        <asp:LinkButton runat="server" ID="_save" CssClass="btn btn-small btn-success" Text="Save" CommandName="Update" />
                                        <asp:LinkButton runat="server" ID="_cancel" CssClass="btn btn-small" Text="Cancel" CommandName="Cancel" />
                                    </td>
                                </tr>
                            </EditItemTemplate>
                        </asp:ListView>
                        <p class="error">
                            <asp:Literal runat="server" ID="errorMessage" Text="" />
                        </p>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="_upProgress" DisplayAfter="1">
                <ProgressTemplate>
                    loading...
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </div>
</asp:Content>
