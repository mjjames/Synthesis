<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeBehind="RecycleBin.aspx.cs" Inherits="mjjames.AdminSystem.RecycleBin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<h1 class="listingTitle">
		<asp:Literal runat="server" ID="dbEditorLabel" />
		Recylcle Bin: Listing View
	</h1>
	<div id="colContainer">
		<div id="leftCol">
			<p class="description">
				Please choose an item on the right and either click "RESTORE" to restore your item back into your system, or to permenantly remove it click "DELETE"
			</p>
		</div>
		<div id="rightCol">
			<asp:GridView runat="server" ID="recycledItems" CssClass="listingTable" AutoGenerateColumns="false" OnSelectedIndexChanged="RestoreItem" OnRowDeleting="OnRowDeleting">
				<Columns>
					<asp:CommandField  ShowSelectButton="true" SelectText="Restore" />
					<asp:BoundField ShowHeader="true" HeaderText="Title" DataField="ItemTitle" />
					<asp:BoundField ShowHeader="true" HeaderText="Type" DataField="ItemType" />
					<asp:CommandField ShowDeleteButton="true" DeleteText="Delete" ControlStyle-CssClass="buttonDelete" />
				</Columns>
				<EmptyDataTemplate>
					<h3>Recycle Bin Is Empty</h3>
				</EmptyDataTemplate>	
			</asp:GridView>
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