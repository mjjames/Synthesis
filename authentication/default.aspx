<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="mjjames.AdminSystem.authentication.Login" Title="Untitled Page"  EnableEventValidation="true" Codebehind="default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<h1 class="listingTitle">
		<asp:Label runat="server" ID="lblMsg" Text="System Login: " />
	</h1>
	<div id="colContainer">
		<div id="loginWrapper">
			<div id="loginForm">
				<div class="row">
					<label runat="server" id="labelUserName" for="inputUserName" class="label">
						Username:</label>
					<input runat="server" id="inputUserName" type="text" class="field" />
					<asp:RequiredFieldValidator ControlToValidate="inputUserName" Display="Static" ErrorMessage="*"
						runat="server" ID="vUserName" CssClass="validate" />
				</div>
				<div class="row">
					<label runat="server" id="labelPassword" for="inputPassword" class="label">
						Password:</label>
					<input runat="server" id="inputPassword" type="password" class="field" />
					<asp:RequiredFieldValidator ControlToValidate="inputPassword" Display="Static" ErrorMessage="*"
						runat="server" ID="vPassword" CssClass="validate" />
				</div>
				<div class="row">
					<input runat="server" id="btnLogon" type="submit" value="Logon" onserverclick="btnLogin_Click" class="submit" />
				</div>
			</div>
			<div id="centerBG">
				&nbsp;
			</div>
		</div>
	</div>
</asp:Content>
