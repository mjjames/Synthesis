<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	CodeBehind="ResetPassword.aspx.cs" Inherits="mjjames.AdminSystem.Authentication.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1 class="listingTitle">
		User Administration
	</h1>
	<asp:ScriptManager ID="ScriptManager1" runat="server">
	</asp:ScriptManager>
	<div id="colContainer">
		<div id="leftCol" runat="server" class="leftCol">
		</div>
		<div id="rightCol">
			<h2>
				<asp:Literal runat="server" ID="settingLabel" Text="Reset Password" />
			</h2>
			<asp:Panel ID="_ChangePassword" runat="server" Visible="false">
				<span class="row">
					<asp:Label runat="server" ID="_lblPassword" CssClass="label" AssociatedControlID="_txtPassword">New Password:</asp:Label>
					<asp:TextBox runat="server" ID="_txtPassword" CssClass="field" AutoCompleteType="Disabled"
						TextMode="Password" />
					<asp:RequiredFieldValidator ID="_valRequired" runat="server" ControlToValidate="_txtPassword"
						ErrorMessage="* You must supply a new password" Display="dynamic">*
					</asp:RequiredFieldValidator>
				</span>
				<span class="row">
					<asp:Label runat="server" ID="_lblConfirmPassword" CssClass="label" AssociatedControlID="_txtConfirmPassword">Confirm New Password:</asp:Label>
					<asp:TextBox runat="server" ID="_txtConfirmPassword" CssClass="field" AutoCompleteType="Disabled"
						TextMode="Password" />
					<asp:RequiredFieldValidator ID="_RequiredFieldValidator1" runat="server" ControlToValidate="_txtConfirmPassword"
						ErrorMessage="* You must confirm your new password" Display="dynamic">*
					</asp:RequiredFieldValidator>
				</span>
				<span class="row">
					<asp:Button runat="server" ID="_savePassword" Text="Save New Password" OnClick="SavePassword" />
				</span>
				<asp:RegularExpressionValidator runat="server" ID="_regexPassword" ControlToValidate="_txtPassword"
						SetFocusOnError="true" ValidationExpression='^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$'
						ErrorMessage="Error: Your password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character, Valid special characters." />
				<asp:CompareValidator runat="server" ID="_comparePassword" ControlToCompare="_txtPassword"
					Display="dynamic" ControlToValidate="_txtConfirmPassword" EnableClientScript="true"
					Operator="Equal" Text="Please ensure both passwords are the same" />
				<asp:ValidationSummary runat="server" ID="_Summary" DisplayMode="BulletList" />
			</asp:Panel>
			<asp:Literal runat="server" ID="_Status" />
		</div>
	</div>
</asp:Content>
