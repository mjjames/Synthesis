<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	CodeFile="NewsletterBuilder.aspx.cs" Inherits="newsletters_NewsletterBuilder" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" SiteMapProvider="newsletterNavigation"
		ShowStartingNode="false" />
	<asp:LinqDataSource ID="ldsNewsletters" runat="server" ContextTypeName="adminDataClassesDataContext"
		EnableDelete="True" EnableInsert="True" EnableUpdate="True" TableName="Newsletters">
	</asp:LinqDataSource>
	<asp:ScriptManager ID="sm" runat="server" />
	<h1 class="listingTitle">
		Newsletters: Newsletter Builder
	</h1>
	<div id="colContainer">
		<div id="leftCol">
			<h2>
				Quick Edit</h2>
			<div id="treeView">
				<asp:TreeView ID="treeListing" runat="server" DataSourceID="navigationSiteMap" PopulateNodesFromClient="true"
					ShowExpandCollapse="true" ShowLines="false" EnableClientScript="true" ExpandDepth="1"
					CssClass="treeView" />
			</div>
		</div>
		<div id="rightCol">
			<h2>
				Status:
				<asp:Label ID="labelStatus" Text="Building" runat="server" CssClass="status"></asp:Label>
			</h2>
			<div id="pageEditor" class="listingTable">
				<asp:PlaceHolder ID="placeholderTabs" runat="server" />
				<asp:HiddenField ID="newsletter_key" runat="server"/>
			</div>
		</div>
		<hr class="clear" />
	</div>
</asp:Content>
