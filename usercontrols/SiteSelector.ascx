<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteSelector.ascx.cs"
	Inherits="mjjames.AdminSystem.usercontrols.SiteSelector" %>
<div class="siteSelector" runat="server" id="SiteSelectorWrapper" visible="true">
	<asp:Label runat="server" AssociatedControlID="ddlSites" ID="lblSites">Manage Site:</asp:Label>
	<asp:DropDownList runat="server" ID="ddlSites" OnSelectedIndexChanged="UpdateSiteSession" AutoPostBack="true" OnInit="LoadSites">
	</asp:DropDownList>
</div>
