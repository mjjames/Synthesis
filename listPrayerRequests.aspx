<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
    CodeFile="listPrayerRequests.aspx.cs" Inherits="PrayerRequest" Title="Admin Tool - Prayer Request Manager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="sqldatasourcePrayerRequest" runat="server" ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        SelectCommand="SELECT * FROM [prayer_request] ORDER BY [date], [active]" InsertCommand="INSERT INTO [prayer_request] ([title], [request], [name], [includeinnewsletter], [showonsite], [date], [active]) VALUES (@title, @request, @name, @includeinnewsletter, @showonsite, @date, @active)"
        DeleteCommand="DELETE FROM [prayer_request] WHERE [prayerrequest_key] = @original_prayerrequest_key AND [title] = @original_title AND [request] = @original_request AND [name] = @original_name AND [includeinnewsletter] = @original_includeinnewsletter AND [showonsite] = @original_showonsite AND [date] = @original_date AND [active] = @original_active"
        UpdateCommand="UPDATE [prayer_request] SET [title] = @title, [request] = @request, [name] = @name, [includeinnewsletter] = @includeinnewsletter, [showonsite] = @showonsite, [date] = @date, [active] = @active WHERE [prayerrequest_key] = @original_prayerrequest_key AND [title] = @original_title AND [request] = @original_request AND [name] = @original_name AND [includeinnewsletter] = @original_includeinnewsletter AND [showonsite] = @original_showonsite AND [date] = @original_date AND [active] = @original_active"
        ConflictDetection="CompareAllValues" OldValuesParameterFormatString="original_{0}">
        <DeleteParameters>
            <asp:Parameter Name="original_prayerrequest_key" Type="Int32" />
            <asp:Parameter Name="original_title" Type="String" />
            <asp:Parameter Name="original_request" Type="String" />
            <asp:Parameter Name="original_name" Type="String" />
            <asp:Parameter Name="original_includeinnewsletter" Type="Boolean" />
            <asp:Parameter Name="original_showonsite" Type="Boolean" />
            <asp:Parameter Name="original_date" Type="DateTime" />
            <asp:Parameter Name="original_active" Type="Boolean" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="request" Type="String" />
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="includeinnewsletter" Type="Boolean" />
            <asp:Parameter Name="showonsite" Type="Boolean" />
            <asp:Parameter Name="date" Type="DateTime" />
            <asp:Parameter Name="active" Type="Boolean" />
            <asp:Parameter Name="original_prayerrequest_key" Type="Int32" />
            <asp:Parameter Name="original_title" Type="String" />
            <asp:Parameter Name="original_request" Type="String" />
            <asp:Parameter Name="original_name" Type="String" />
            <asp:Parameter Name="original_includeinnewsletter" Type="Boolean" />
            <asp:Parameter Name="original_showonsite" Type="Boolean" />
            <asp:Parameter Name="original_date" Type="DateTime" />
            <asp:Parameter Name="original_active" Type="Boolean" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="request" Type="String" />
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="includeinnewsletter" Type="Boolean" />
            <asp:Parameter Name="showonsite" Type="Boolean" />
            <asp:Parameter Name="date" Type="DateTime" />
            <asp:Parameter Name="active" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
    <h1 class="listingTitle">
        Prayer Management: Prayer Request Listing
    </h1>
    <asp:ScriptManager ID="scriptManager" runat="server" AllowCustomErrorsRedirect="true" ></asp:ScriptManager>
    <asp:UpdatePanel ID="prayerListing" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <asp:GridView ID="gvListing" runat="server" AllowPaging="True" AllowSorting="True"
                DataSourceID="sqldatasourcePrayerRequest" AutoGenerateColumns="False" CssClass="listingTable">
                <Columns>
                    <asp:CommandField ShowEditButton="True" />
                    <asp:BoundField DataField="title" HeaderText="title" SortExpression="title" />
                    <asp:BoundField DataField="name" HeaderText="name" SortExpression="name" />
                    <asp:BoundField DataField="request" HeaderText="request" />
                    <asp:CheckBoxField DataField="includeinnewsletter" HeaderText="includeinnewsletter"
                        SortExpression="includeinnewsletter" />
                    <asp:CheckBoxField DataField="showonsite" HeaderText="showonsite" SortExpression="showonsite" />
                    <asp:CheckBoxField DataField="active" HeaderText="active" SortExpression="active" />
                    <asp:CommandField ShowDeleteButton="True" />
                </Columns>
                <EmptyDataTemplate>
                    No Current Prayer Requests
                </EmptyDataTemplate>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <mjjames:jsLoader ID="jquery" runat="server" JSLibrary="jquery" />

    <script type="text/javascript">
		$(".buttonDelete").click(function(){
			var bDelete = confirm("Are You Sure You Want To Delete This Banner?");
			return bDelete;
		});
    </script>

</asp:Content>
