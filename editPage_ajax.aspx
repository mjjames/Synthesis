<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	CodeFile="editPage_ajax.aspx.cs" Inherits="editPage_ajax" Title="Edit Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:ScriptManager ID="scriptmanagerAdmin" runat="server">
	</asp:ScriptManager>
	<asp:LinkButton ID="buttonSubPages" runat="server" Style="position: relative" CausesValidation="false"
		OnClick="showSubPages" ToolTip="Show SubPages" PostBackUrl="#" Visible="false"
		EnableViewState="False" CssClass="subPages">Sub Pages
 &gt; &gt;</asp:LinkButton>
	<asp:LinkButton ID="linkbuttonBack" runat="server" Style="position: relative" ToolTip="Back to Page Listing"
		PostBackUrl="#" Visible="true" EnableViewState="False" CausesValidation="false" OnClick="showPageList"
		CssClass="backListing">
         &lt; &lt; Back to Page Listing
	</asp:LinkButton>
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" SiteMapProvider="adminNavigation"
		ShowStartingNode="True" />
	<div id="pageStatus">
		<p class="labelStatus">
			Status:
			<asp:Label ID="labelStatus" Text="Editing" runat="server" CssClass="status"></asp:Label>
		</p>
	</div>
	<div id="colContainer">
		<div id="leftCol">
			<asp:TreeView ID="treeListing" runat="server" DataSourceID="navigationSiteMap" PopulateNodesFromClient="true"
				ShowExpandCollapse="true" ShowLines="true" EnableClientScript="true" ExpandDepth="1"
				CssClass="treeView" />
		</div>
		<div id="rightCol">
			<asp:PlaceHolder ID="placeholderTabs" runat="server" />
			<asp:HiddenField ID="page_key" runat="server"/>
		</div>
	</div>
</asp:Content>
