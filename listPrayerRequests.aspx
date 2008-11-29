<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
    CodeFile="listPrayerRequests.aspx.cs" Inherits="PrayerRequest" Title="Admin Tool - Prayer Request Manager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="sqldatasourcePrayerRequest" runat="server" ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        SelectCommand="SELECT * FROM [prayer_request] ORDER BY [date] desc, [active]" 
        DeleteCommand="DELETE FROM [prayer_request] WHERE [prayerrequest_key] = @original_prayerrequest_key"
        UpdateCommand="UPDATE [prayer_request] SET [title] = @title, [request] = @request, [name] = @name, [includeinnewsletter] = @includeinnewsletter, [showonsite] = @showonsite, [active] = @active WHERE [prayerrequest_key] = @original_prayerrequest_key"
        ConflictDetection="CompareAllValues"
        OldValuesParameterFormatString="original_{0}">
        <DeleteParameters>
            <asp:Parameter Name="original_prayerrequest_key" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="request" Type="String" />
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="includeinnewsletter" Type="Boolean" />
            <asp:Parameter Name="showonsite" Type="Boolean" />
            <asp:Parameter Name="active" Type="Boolean" />
            <asp:Parameter Name="original_prayerrequest_key" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <h1 class="listingTitle">
        Prayer Management: Prayer Request Listing
    </h1>
    <asp:ScriptManager ID="scriptManager" runat="server" AllowCustomErrorsRedirect="true" ></asp:ScriptManager>
    <asp:UpdateProgress ID="updateProgress" runat="server"  AssociatedUpdatePanelID="prayerListing">
		<ProgressTemplate>
			Updating ...
		</ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="prayerListing" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <asp:GridView ID="gvListing" runat="server" AllowPaging="True" AllowSorting="True" DataKeyNames="prayerrequest_key"
                DataSourceID="sqldatasourcePrayerRequest" AutoGenerateColumns="False" CssClass="listingTable" >
                <Columns>
                    <asp:BoundField DataField="title" HeaderText="title" SortExpression="title" />
                    <asp:BoundField DataField="name" HeaderText="name" SortExpression="name" />
                    <asp:TemplateField HeaderText="request">
						<EditItemTemplate>
							<asp:TextBox runat="server" id="txtRequest" TextMode="MultiLine" Columns="40" Text='<%# Bind("request") %>' />
						</EditItemTemplate>
						<ItemTemplate>
							<asp:Label runat="server" ID="lblRequest" Text='<%# Eval("request") %>' />
						</ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="includeinnewsletter" HeaderText="includeinnewsletter"
                        SortExpression="includeinnewsletter" />
                    <asp:CheckBoxField DataField="showonsite" HeaderText="showonsite" SortExpression="showonsite" />
                    <asp:CheckBoxField DataField="active" HeaderText="active" SortExpression="active" />
                    <asp:CommandField ShowEditButton="True" />
                    <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="buttonDelete" />
                </Columns>
                <EmptyDataTemplate>
                    No Current Prayer Requests
                </EmptyDataTemplate>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>


    <script type="text/javascript">
		$(".buttonDelete").click(function(){
			var bDelete = confirm("Are You Sure You Want To Delete This Banner?");
			return bDelete;
		});
    </script>

</asp:Content>
