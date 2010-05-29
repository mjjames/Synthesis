<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="mjjames.AdminSystem.NewslettersSubscriptionmanagement" Codebehind="subscriptionmanagement.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" SiteMapProvider="newsletterNavigation"
		ShowStartingNode="False" />
	<asp:ScriptManager runat="server" ID="sm" OnAsyncPostBackError="errorStatus"  CompositeScript-ScriptMode="Release" ScriptMode="Release" />
	<h1 class="listingTitle">
		Newsletter: Subscription Management
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
			<asp:LinqDataSource ID="ldsReciprients" runat="server" ContextTypeName="mjjames.AdminSystem.DataContexts.AdminDataContext"
				EnableInsert="True" EnableUpdate="True" OrderBy="name" TableName="NewsletterReciprients"
				AutoPage="true" EnableDelete="True" AutoSort="true" >
			</asp:LinqDataSource>

			<script type="text/javascript" language="javascript">
				var messageElem = 'span.status';
				Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

				function ClearErrorState() {
					$(messageElem).text("");
				}
				function EndRequestHandler(sender, args) {
					if (args.get_error() != undefined) {
						var errorMessage;
						if (Sys.Debug.isDebug) {
							errorMessage = args.get_error().message;
						}
						else {
							errorMessage = "Sorry an Error has Occurred";
						}
						args.set_errorHandled(true);
						$(messageElem).text(errorMessage);
					}
				}
			</script>

			<asp:UpdatePanel runat="server" ID="updateReciprients" ChildrenAsTriggers="true">
				<ContentTemplate>
					<h2>
						Status:
						<asp:UpdateProgress runat="server" ID="upLoading" AssociatedUpdatePanelID="updateReciprients">
							<ProgressTemplate>
								Updating...
							</ProgressTemplate>
						</asp:UpdateProgress>
						<asp:Label ID="labelStatus" runat="server" CssClass="status" Text="Listing Mode" />
					</h2>
					<asp:ListView ID="lvReciprients" runat="server" DataSourceID="ldsReciprients" InsertItemPosition="LastItem"
						OnItemCanceling="resetStatus" DataKeyNames="reciprient_key" OnItemDeleted="removedStatus"
						OnItemEditing="editStatus" OnItemInserted="insertedStatus" OnItemUpdated="updatedStatus">
						<ItemTemplate>
							<tr class="pageListRow">
								<td>
									<asp:ImageButton ID="EditButton" runat="server" AlternateText="Edit" CommandName="Edit"
										ImageUrl="~/adminimages/icons/edit.png" ToolTip="Edit Reciprient" />
								</td>
								<td>
									<asp:Label ID="nameLabel" runat="server" Text='<%# Eval("name") %>' />
								</td>
								<td>
									<asp:Label ID="emailLabel" runat="server" Text='<%# Eval("email") %>' />
								</td>
								<td>
									<asp:CheckBox ID="activeCheckBox" runat="server" Checked='<%# Eval("active") %>'
										Enabled="false" />
								</td>
								<td>
									<asp:CheckBox ID="confirmedCheckBox" runat="server" Checked='<%# Eval("confirmed") %>'
										Enabled="false" />
								</td>
								<td>
									<asp:LinkButton ID="sendConfirmation" runat="server" OnClick="ResendConfirmation"
										Text="Resend" ToolTip="Resend Confirmation Email" />
									<asp:ImageButton ID="removeReciprient" runat="server" AlternateText="Remove" CommandName="Delete"
										CssClass="buttonDelete" ImageUrl="~/adminimages/icons/remove.png" ToolTip="Remove Reciprient" />
								</td>
							</tr>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<tr class="pageListRowAlternate">
								<td>
									<asp:ImageButton ID="EditButton" runat="server" AlternateText="Edit" CommandName="Edit"
										ImageUrl="~/adminimages/icons/edit.png" ToolTip="Edit Reciprient" />
								</td>
								<td>
									<asp:Label ID="nameLabel" runat="server" Text='<%# Eval("name") %>' />
								</td>
								<td>
									<asp:Label ID="emailLabel" runat="server" Text='<%# Eval("email") %>' />
								</td>
								<td>
									<asp:CheckBox ID="activeCheckBox" runat="server" Checked='<%# Eval("active") %>'
										Enabled="false" />
								</td>
								<td>
									<asp:CheckBox ID="confirmedCheckBox" runat="server" Checked='<%# Eval("confirmed") %>'
										Enabled="false" />
								</td>
								<td>
									<asp:LinkButton ID="sendConfirmation" runat="server" OnClick="ResendConfirmation"
										Text="Resend" ToolTip="Resend Confirmation Email" />
									<asp:ImageButton ID="removeReciprient" runat="server" AlternateText="Remove" CommandName="Delete"
										CssClass="buttonDelete" ImageUrl="~/adminimages/icons/remove.png" ToolTip="Remove Reciprient" />
								</td>
							</tr>
						</AlternatingItemTemplate>
						<EmptyDataTemplate>
							<table runat="server" style="">
								<tr>
									<td>
										No data was returned.
									</td>
								</tr>
							</table>
						</EmptyDataTemplate>
						<InsertItemTemplate>
							<tr style="">
								<td>
								</td>
								<td>
									<asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("name") %>' />
								</td>
								<td>
									<asp:TextBox ID="emailTextBox" runat="server" Text='<%# Bind("email") %>' />
								</td>
								<td>
									<asp:CheckBox ID="activeCheckBox" runat="server" Checked='<%# Bind("active") %>' />
								</td>
								<td>
									<asp:CheckBox ID="confirmedCheckBox" runat="server" Checked='<%# Bind("confirmed") %>' />
								</td>
								<td>
									<asp:ImageButton ID="InsertButton" runat="server" AlternateText="Insert" CommandName="Insert"
										ImageUrl="~/adminimages/icons/Verify.png" />
									<asp:ImageButton ID="CancelButton" runat="server" AlternateText="Clear" CommandName="Cancel"
										ImageUrl="~/adminimages/icons/cancel.png" />
								</td>
							</tr>
						</InsertItemTemplate>
						<LayoutTemplate>
							<table id="itemPlaceholderContainer" runat="server" border="1" cellspacing="0" class="listingTable"
								rules="all" style="border-collapse: collapse;">
								<tr runat="server" class="pageListHeader">
									<th runat="server">
									</th>
									<th runat="server">
										name
									</th>
									<th runat="server">
										email
									</th>
									<th runat="server">
										active
									</th>
									<th runat="server">
										confirmed
									</th>
									<th runat="server">
									</th>
								</tr>
								<tr id="itemPlaceholder" runat="server">
								</tr>
							</table>
							<asp:DataPager ID="DataPager1" runat="server">
								<Fields>
									<asp:NextPreviousPagerField ButtonType="Button" FirstPageImageUrl="~/adminimages/icons/move_first.png"
										FirstPageText="|&lt;" PreviousPageImageUrl="~/adminimages/icons/move_prev.png"
										PreviousPageText="&lt;" ShowFirstPageButton="True" ShowNextPageButton="False"
										ShowPreviousPageButton="True" />
									<asp:NumericPagerField />
									<asp:NextPreviousPagerField ButtonType="Button" LastPageImageUrl="~/adminimages/icons/move_last.png"
										LastPageText="&gt;|" NextPageImageUrl="~/adminimages/icons/move_next.png" NextPageText="&gt;"
										ShowLastPageButton="True" ShowNextPageButton="True" ShowPreviousPageButton="False" />
								</Fields>
							</asp:DataPager>
						</LayoutTemplate>
						<EditItemTemplate>
							<tr style="">
								<td>
								</td>
								<td>
									<asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("name") %>' />
								</td>
								<td>
									<asp:TextBox ID="emailTextBox" runat="server" Text='<%# Bind("email") %>' />
								</td>
								<td>
									<asp:CheckBox ID="activeCheckBox" runat="server" Checked='<%# Bind("active") %>' />
								</td>
								<td>
									<asp:CheckBox ID="confirmedCheckBox" runat="server" Checked='<%# Bind("confirmed") %>' />
								</td>
								<td>
									<asp:ImageButton ID="UpdateButton" runat="server" AlternateText="Save" CommandName="Update"
										ImageUrl="~/adminimages/icons/verify.png" />
									<asp:ImageButton ID="CancelButton" runat="server" AlternateText="Cancel" CommandName="Cancel"
										ImageUrl="~/adminimages/icons/cancel.png" />
								</td>
							</tr>
						</EditItemTemplate>
					</asp:ListView>

					<script type="text/javascript">

						$(".buttonDelete").click(function() {
							var bDelete = confirm("Are You Sure You Want To Delete This Page?");
							return bDelete;
						});
					</script>

					</h2>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</div>
</asp:Content>
