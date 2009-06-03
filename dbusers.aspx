<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="dbusers" Title="User Administration" Codebehind="dbusers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
		<ContentTemplate>
			<h1 class="listingTitle">
				User Administration
			</h1>
			<div id="colContainer">
					<asp:Label ID="labelDBUser" runat="server" for="dbuserDropDownList">Edit mode:</asp:Label>
					<asp:DropDownList ID="dbuserDropDownList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="changeView">
						<asp:ListItem Value="createUser">Create User</asp:ListItem>
						<asp:ListItem Selected="True" Value="changePassword">Change Password</asp:ListItem>
					</asp:DropDownList>
					<asp:CreateUserWizard ID="CreateUserWizard" runat="server" Visible="false">
						<WizardSteps>
							<asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
							</asp:CreateUserWizardStep>
							<asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
							</asp:CompleteWizardStep>
						</WizardSteps>
					</asp:CreateUserWizard>
					<asp:ScriptManager ID="ScriptManager1" runat="server">
					</asp:ScriptManager>
					<asp:ChangePassword ID="ChangePassword" runat="server">
					</asp:ChangePassword>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
