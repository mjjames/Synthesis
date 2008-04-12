<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="listPage.aspx.cs" Inherits="listPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
		SelectCommand="SELECT * FROM [page]" ConflictDetection="CompareAllValues" DeleteCommand="DELETE FROM [page] WHERE [page_key] = @page_key"
		OnLoad="getFKey">
		<DeleteParameters>
			<asp:Parameter Name="page_key" Type="Int32" />
		</DeleteParameters>
	</asp:SqlDataSource>
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" SiteMapProvider="adminNavigation"
		ShowStartingNode="True" />
	<h1 class="listingTitle">
		Page Editor: Listing View
	</h1>
	<div id="colContainer">
		<div id="leftCol">
			<h2>Quick Edit</h2>
			<div id="treeView">
				<asp:TreeView ID="treeListing" runat="server" DataSourceID="navigationSiteMap" PopulateNodesFromClient="true"
				ShowExpandCollapse="true" ShowLines="true" EnableClientScript="true" ExpandDepth="1"
				CssClass="treeView" />
			</div>
		</div>
		<div id="rightCol">
			<h2>Pages at this Level</h2>
			<div class="listingTable">
				<asp:LinkButton ID="linkbuttonBack" runat="server" CssClass="backUp" ToolTip="Up a Level" PostBackUrl="~/editPage_ajax.aspx" Visible="true" EnableViewState="False">
					&lt; &lt; Up a Level
				</asp:LinkButton>
				<asp:LinkButton ID="buttonAddPage" runat="server" CssClass="addPage" ToolTip="Add a New Page" PostBackUrl="~/editpage_ajax.aspx">
					Add Page
				</asp:LinkButton>
			</div>
			<asp:GridView ID="pageListing" runat="server" AllowPaging="True" AllowSorting="True"
				AutoGenerateColumns="False" DataKeyNames="page_key,page_fkey" DataSourceID="SqlDataSource1"
				OnSelectedIndexChanged="GridView1_SelectedIndexChanged" EnableViewState="False"
				DataMember="DefaultView" CssClass="listingTable">
				<Columns>
					<asp:CommandField ShowSelectButton="True" SelectText="Edit" />
					<asp:BoundField DataField="navtitle" HeaderText="Nav Title" SortExpression="navtitle" />
					<asp:BoundField DataField="title" HeaderText="Page Title" SortExpression="title" />
					<asp:BoundField DataField="sortorder" HeaderText="Page Sort Order" SortExpression="sortorder" />
					<asp:BoundField DataField="active" HeaderText="Active" SortExpression="active" />
					<asp:CommandField ShowDeleteButton="True" />
				</Columns>
				<EmptyDataTemplate>
					Currently there are no pages within this section
				</EmptyDataTemplate>
				<FooterStyle CssClass="pageListFooter" />
				<RowStyle CssClass="pageListRow" />
				<HeaderStyle CssClass="pageListHeader" />
				<AlternatingRowStyle CssClass="pageListRowAlternate" />
			</asp:GridView>
		</div>
	</div>
</asp:Content>
