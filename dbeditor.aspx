﻿<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="dbeditor" Title="DBEditor" ValidateRequest="false" Codebehind="dbeditor.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:ScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
		AsyncPostBackTimeout="60" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
	</asp:ScriptManager>
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" ShowStartingNode="True"
		OnLoad="loadListing" />
		
	<h1 class="listingTitle">
		<asp:Literal runat="server" ID="dbeditorLabel" />
		Editor : Editing
	</h1>
	<div id="colContainer">
		<div id="leftCol" runat="server" class="leftCol">
			<h2>
				Quick Edit</h2>
			<div id="treeView">
				<asp:UpdatePanel ID="treePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"  >
					<ContentTemplate>
					
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
		</div>
		<div id="rightCol">
			<h2>
				Status:
				<asp:Label ID="labelStatus" Text="Editing" runat="server" CssClass="status"></asp:Label>
			</h2>
			<div class="listingTable">
				<asp:LinkButton ID="linkbuttonBack" runat="server" Style="position: relative" ToolTip="Back to Listing"
					PostBackUrl="#" Visible="true" EnableViewState="False" CausesValidation="false"
					CssClass="backListing">
					 &lt; &lt; Back to Listing
				</asp:LinkButton>
				<asp:LinkButton ID="linkbuttonSubPages" runat="server" Style="position: relative"
					CausesValidation="false" ToolTip="Show SubItems" PostBackUrl="#" Visible="false"
					EnableViewState="False" CssClass="subPages">
					Sub Items &gt; &gt;
				</asp:LinkButton>
			</div>
			<div id="pageEditor" class="listingTable">
				<asp:PlaceHolder ID="placeholderTabs" runat="server" />
				<asp:HiddenField ID="pkey" runat="server" />
			</div>
		</div>
		<hr class="clear" />
	</div>
</asp:Content>