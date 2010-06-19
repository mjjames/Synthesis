<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeBehind="Donations.aspx.cs" Inherits="mjjames.AdminSystem.Donations" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	   <asp:ScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
        AsyncPostBackTimeout="60" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
        <CompositeScript>
            <Scripts>
                <asp:scriptreference name="WebForms.js" assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
                <asp:ScriptReference Name="MicrosoftAjax.js" />
                <asp:ScriptReference Name="MicrosoftAjaxWebForms.js" />
            </Scripts>
        </CompositeScript>
    </asp:ScriptManager>
	
	<h1 class="listingTitle">
		Donations Listing View
	</h1>
	<div id="colContainer">
		<div id="leftCol" runat="server" class="leftCol">
			<h2>
				Quick Links</h2>
			<div id="treeView">
				<ul>
					<li>
						<asp:HyperLink runat="server" ID="hlListing" Text="Donations" NavigateUrl="~/Donations.aspx" />
					</li>
					<li>
						<asp:HyperLink runat="server" ID="hlReports" Text="Reports" NavigateUrl="~/DonationReports.aspx" />
					</li>
				</ul>
			</div>
		</div>
		<div id="rightCol">
			<h2 runat="server" id="levelLabel">
				Donations</h2>
			<div class="listingTable">
				<span class="startDate">
					<asp:Label runat="server" ID="labelStartDate" Text="Start Date:" AssociatedControlID="txtStartDate" CssClass="labelStartDate" />
					<asp:TextBox runat="server" ID="txtStartDate" TextMode="SingleLine" CssClass="txtStartDate" AutoPostBack="true" OnTextChanged="UpdateStartDate" ToolTip="Start Date For Donations" />
				</span>
				<span class="endDate">
					<asp:Label runat="server" ID="labelEndDate" Text="End Date:" AssociatedControlID="txtEndDate" CssClass="labelEndDate" />
					<asp:TextBox runat="server" ID="txtEndDate" TextMode="SingleLine" CssClass="txtEndDate" AutoPostBack="true" OnTextChanged="UpdateEndDate" ToolTip="End Date for Donations" />
				</span>
			</div>
			<asp:UpdatePanel ID="upListing" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"  >
				<ContentTemplate>
					<asp:ListView runat="server" ID="lvDonations" DataKeyNames="donation_key" DataSource="loadDonations" >
						<LayoutTemplate>
							<table class="listingTable">
								<thead>
									<tr>
										<th>Date</th>
										<th>Donatee</th>
										<th>Amount</th>
										<th>Gift Aid?</th>
									</tr>
								</thead>
								<tbody>
									<tr runat="server" id="itemPlaceholder" />
								</tbody>
							</table>
						</LayoutTemplate>
						<ItemTemplate>
							<tr class="pageListRow>
								<td></td>
								<td></td>
								<td></td>
								<td></td>
							</tr>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<tr class="pageListRowAlternate">
								<td></td>
								<td></td>
								<td></td>
								<td></td>
							</tr>
						</AlternatingItemTemplate>
					</asp:ListView>
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
</asp:Content>