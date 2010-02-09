<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	Inherits="mjjames.AdminSystem.DBUsers" Title="User Administration" CodeBehind="DBUsers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<h1 class="listingTitle">
		User Administration
	</h1>
	<asp:ScriptManager ID="ScriptManager1" runat="server">
	</asp:ScriptManager>
	<div id="colContainer">
		<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
			<ContentTemplate>
				<div id="leftCol" runat="server" class="leftCol">
					<h2>
						Available Options
					</h2>
					<asp:Label ID="labelDBUser" runat="server" AssociatedControlID="dbuserDropDownList">Edit mode:</asp:Label>
					<asp:DropDownList ID="dbuserDropDownList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ChangeView">
						<asp:ListItem Value="createUser">Create User</asp:ListItem>
						<asp:ListItem Selected="True" Value="changePassword">Change Password</asp:ListItem>
						<asp:ListItem Value="manageUser">Manage Users</asp:ListItem>
					</asp:DropDownList>
				</div>
				<div id="rightCol">
					<h2>
						<asp:Literal runat="server" ID="settingLabel" Text="Change Password" />
					</h2>
					<asp:CreateUserWizard ID="CreateUserWizard" runat="server" Visible="false" OnCreatedUser="SetDefaultRole"
						LoginCreatedUser="false" >
						<WizardSteps>
							<asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
								<ContentTemplate>
									<h3>User Information</h3>
										<span class="row">
											<asp:Label runat="server" AssociatedControlID="UserName" CssClass="label">
												Username:
											</asp:Label>
											<asp:TextBox runat="server" ID="UserName" CssClass="field" />
											<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="UserName"
													ErrorMessage="Username is required." CssClass="validation"  Display="dynamic">*
											</asp:RequiredFieldValidator>
										</span>
										<span class="row">
											<asp:Label runat="server" CssClass="label" AssociatedControlID="Password">
												Password:
											</asp:Label>
											
											<asp:TextBox runat="server" ID="Password" TextMode="Password" />
											<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator10" ControlToValidate="Password"
												ErrorMessage="Password is required." CssClass="validation" Display="dynamic">*
											</asp:RequiredFieldValidator>
										</span>
										<span class="row">
											<asp:Label runat="server" CssClass="label" AssociatedControlID="ConfirmPassword">
												Confirm Password:
											</asp:Label>
											<asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
											<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator13" ControlToValidate="ConfirmPassword"
												ErrorMessage="Confirm Password is required." CssClass="validation"  Display="Static"	>*
											</asp:RequiredFieldValidator>
											
										</span>
										<span class="row">
											<asp:Label runat="server" CssClass="label" AssociatedControlID="Email">
												Email:
											</asp:Label>
											<asp:TextBox runat="server" ID="Email" CssClass="wideField" />
											<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator11" ControlToValidate="Email"
												ErrorMessage="Email is required." CssClass="validation" Display="dynamic">*
											</asp:RequiredFieldValidator>
											<asp:RegularExpressionValidator runat="server" ID="emailValidation" ControlToValidate="Email"
												ErrorMessage="Please Provide a Valid Email" 
												ValidationExpression="[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?" />
										</span>
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
										<span class="row">
											<asp:RegularExpressionValidator runat="server" ID="_regexPassword" ControlToValidate="Password"
												SetFocusOnError="true" ValidationExpression='^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$'
												ErrorMessage="Error: Your password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character, Valid special characters." />				
											<br />
											<asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
													ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."></asp:CompareValidator>

										</span>
										<span class="row">
											<asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
										</span>
									</table>
								</ContentTemplate>
							</asp:CreateUserWizardStep>
							<asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
							</asp:CompleteWizardStep>
						</WizardSteps>
					</asp:CreateUserWizard>
					<asp:ChangePassword ID="ChangePassword" runat="server">
						<ChangePasswordTemplate>
							<span class="row">
								<asp:Label runat="server" ID="_currentPassword" CssClass="label" AssociatedControlID="CurrentPassword">
									Current Password
								</asp:Label>
								<asp:TextBox runat="server" ID="CurrentPassword" CssClass="field" TextMode="Password" />
							</span>
							<span class="row">
								<asp:Label runat="server" ID="_lblPassword" CssClass="label" AssociatedControlID="NewPassword">New Password:</asp:Label>
								<asp:TextBox runat="server" ID="NewPassword" CssClass="field" AutoCompleteType="Disabled"
									TextMode="Password" />
								<asp:RequiredFieldValidator ID="_valRequired" runat="server" ControlToValidate="NewPassword"
									ErrorMessage="* You must supply a new password" Display="dynamic">*
								</asp:RequiredFieldValidator>
							</span>
							<span class="row">
								<asp:Label runat="server" ID="_lblConfirmPassword" CssClass="label" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
								<asp:TextBox runat="server" ID="ConfirmNewPassword" CssClass="field" AutoCompleteType="Disabled"
									TextMode="Password" />
								<asp:RequiredFieldValidator ID="_RequiredFieldValidator1" runat="server" ControlToValidate="ConfirmNewPassword"
									ErrorMessage="* You must confirm your new password" Display="dynamic">*
								</asp:RequiredFieldValidator>
								<asp:CompareValidator runat="server" ID="_comparePassword" ControlToCompare="NewPassword"
									Display="dynamic" ControlToValidate="ConfirmNewPassword" EnableClientScript="true"
									Operator="Equal" Text="Please ensure both passwords are the same" />
							</span>
							<span class="row">
								<asp:Button runat="server" ID="ChangePassword" CommandName="ChangePassword" Text="Change Password" />
							</span>
							<span class="row">
								<asp:RegularExpressionValidator runat="server" ID="_regexPassword" ControlToValidate="NewPassword"
									Display="Dynamic"
									SetFocusOnError="true" ValidationExpression='^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$'
									ErrorMessage="Error: Your password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character, Valid special characters." />
								<br  />
								
								<asp:ValidationSummary runat="server" ID="_Summary" DisplayMode="BulletList" />
								<asp:Literal runat="server" ID="FailureText" Text="" />
							</span>
						</ChangePasswordTemplate>						
						<SuccessTemplate>
							<p>Your Password Has Successfully Been Changed</p>
						</SuccessTemplate>
					</asp:ChangePassword>
					<asp:ListView runat="server" ID="_UserLising" Visible="false" OnItemEditing="EditDataBind"
						OnItemUpdating="UpdateUsers" OnItemDeleting="DeleteUser" OnItemCanceling="CancelEdit" OnItemCommand="CustomCommands">
						<LayoutTemplate>
							<table class="listingTable">
								<thead>
									<tr>
										<th>
										</th>
										<th>
											UserName
										</th>
										<th>
											Role
										</th>
										<th>
											Locked Out?
										</th>
										<th>
											Last Login Date
										</th>
										<th>
										</th>
										<th>
										</th>
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
									<asp:LinkButton runat="server" ID="_edit" CommandName="Edit" Text="Edit" />
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
									<asp:LinkButton runat="server" ID="_resetPassword" CommandName="ResetPassword" Text="Reset Password" />
								</td>
								<td>
									<asp:LinkButton runat="server" ID="_deleteUser" CommandName="Delete" Text="Delete" />
								</td>
							</tr>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<tr class="pageListRowAlternate">
								<td>
									<asp:LinkButton runat="server" ID="_edit" CommandName="Edit" Text="Edit" />
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
									<asp:LinkButton runat="server" ID="_resetPassword" CommandName="ResetPassword" Text="Reset Password" />
								</td>
								<td>
									<asp:LinkButton runat="server" ID="_deleteUser" CommandName="Delete" Text="Delete" />
								</td>
							</tr>
						</AlternatingItemTemplate>
						<EditItemTemplate>
							<tr>
								<td>
								</td>
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
									<asp:LinkButton runat="server" ID="_save" Text="Save" CommandName="Update" />
								</td>
								<td>
									<asp:LinkButton runat="server" ID="_cancel" Text="Cancel" CommandName="Cancel" />
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
</asp:Content>
