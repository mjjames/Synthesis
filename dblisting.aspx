<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	ValidateRequest="false" Inherits="mjjames.AdminSystem.ListPage" CodeBehind="dblisting.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:ScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
		AsyncPostBackTimeout="60" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
	</asp:ScriptManager>
	<asp:SqlDataSource ID="sdsData" runat="server" OnLoad="SetupTable" ConnectionString="<%$ ConnectionStrings:ourDatabase %>">
	</asp:SqlDataSource>
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" ShowStartingNode="True"
		OnLoad="LoadListing" />
	<h1 class="listingTitle">
		<asp:Literal runat="server" ID="dbEditorLabel" />
		Editor: Listing View
	</h1>
	<div id="colContainer">
		<div id="leftCol" runat="server" class="leftCol">
			<h2>
				Quick Edit</h2>
			<div id="treeView">
				<asp:UpdatePanel ID="treePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
					<ContentTemplate>
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
		</div>
		<div id="rightCol">
			<h2 runat="server" id="levelLabel">
				at this Level</h2>
			<div class="listingTable">
				<asp:Hyperlink ID="linkbuttonBack" runat="server" CssClass="backUp" ToolTip="Up a Level"
					NavigateUrl="~/DBEditor.aspx?type=" Visible="true" >
					&lt; &lt; Up a Level
				</asp:Hyperlink>
				<asp:HyperLink ID="buttonAddPage" runat="server" CssClass="addPage" ToolTip="Add a New "
					NavigateUrl="~/DBEditor.aspx?type=" Visible="false">
					Add Page
				</asp:HyperLink>
			</div>
			<asp:UpdatePanel ID="upListing" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"  >
				<ContentTemplate>
					<asp:GridView ID="pageListing" runat="server" AllowPaging="True" AllowSorting="True"
						AutoGenerateColumns="False" DataSourceID="sdsData" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
						EnableViewState="False" DataMember="DefaultView" CssClass="listingTable">
						<Columns>
						</Columns>
						<EmptyDataTemplate>
							Currently there are no items to edit within this section
						</EmptyDataTemplate>
						<FooterStyle CssClass="pageListFooter" />
						<RowStyle CssClass="pageListRow" />
						<HeaderStyle CssClass="pageListHeader" />
						<AlternatingRowStyle CssClass="pageListRowAlternate" />
					</asp:GridView>
				</ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upListing"
				DisplayAfter="500">
				<ProgressTemplate>
					<p class="loading">
						Loading ...</p>
				</ProgressTemplate>
			</asp:UpdateProgress>
		</div>
	</div>
	<mjjames:jsLoader ID="jquery" runat="server" JSLibrary="jquery" />

	<script type="text/javascript">
		var deleteCheck = function() {
			$(".buttonDelete").unbind("click.deletecheck").bind("click.deletecheck", function() {
				var bDelete = confirm("Are You Sure You Want To Delete This Item?");
				return bDelete;
			});
		};
		$(function() {
			deleteCheck(); //setup delete confirmations
			var prm = Sys.WebForms.PageRequestManager.getInstance();
			prm.add_endRequest(deleteCheck); //on async finish we need to redo our deleteCheck
		});
	</script>

</asp:Content>
