<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeFile="listBanners.aspx.cs" Inherits="listBanners" Title="Admin Tool - Banner Manager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConflictDetection="CompareAllValues"
        ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        DeleteCommand="DELETE FROM [banners] WHERE [bannerdid] = @original_bannerdid"
        InsertCommand="INSERT INTO [banners] ([name], [image], [alttext], [url], [category], [randomness]) VALUES (@name, @image, @alttext, @url, @category, @randomness)"
        OldValuesParameterFormatString="original_{0}" ProviderName="System.Data.SqlClient"
        SelectCommand="SELECT * FROM [banners]" UpdateCommand="UPDATE [banners] SET [name] = @name, [image] = @image, [alttext] = @alttext, [url] = @url, [category] = @category, [randomness] = @randomness WHERE [bannerdid] = @original_bannerdid AND [name] = @original_name AND [image] = @original_image AND [alttext] = @original_alttext AND [url] = @original_url AND [category] = @original_category AND [randomness] = @original_randomness">
        <DeleteParameters>
            <asp:Parameter Name="original_bannerdid" Type="Int32" />
            <asp:Parameter Name="original_name" Type="String" />
            <asp:Parameter Name="original_image" Type="String" />
            <asp:Parameter Name="original_alttext" Type="String" />
            <asp:Parameter Name="original_url" Type="String" />
            <asp:Parameter Name="original_category" Type="String" />
            <asp:Parameter Name="original_randomness" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="image" Type="String" />
            <asp:Parameter Name="alttext" Type="String" />
            <asp:Parameter Name="url" Type="String" />
            <asp:Parameter Name="category" Type="String" />
            <asp:Parameter Name="randomness" Type="Int32" />
            <asp:Parameter Name="original_bannerdid" Type="Int32" />
            <asp:Parameter Name="original_name" Type="String" />
            <asp:Parameter Name="original_image" Type="String" />
            <asp:Parameter Name="original_alttext" Type="String" />
            <asp:Parameter Name="original_url" Type="String" />
            <asp:Parameter Name="original_category" Type="String" />
            <asp:Parameter Name="original_randomness" Type="Int32" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="image" Type="String" />
            <asp:Parameter Name="alttext" Type="String" />
            <asp:Parameter Name="url" Type="String" />
            <asp:Parameter Name="category" Type="String" />
            <asp:Parameter Name="randomness" Type="Int32" />
        </InsertParameters>
    </asp:SqlDataSource>
    
    <h1 class="listingTitle">
       Banner Manager: Banner Listing
    </h1>
    
    <asp:LinkButton ID="buttonAddBanner" runat="server"  CssClass="addBanner"
        ToolTip="Add a New Banner" PostBackUrl="~/editbanner.aspx">Add Banner</asp:LinkButton>
    
    <asp:GridView ID="bannerListing" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataKeyNames="bannerdid" DataSourceID="SqlDataSource1" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" CssClass="listingTable">
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
            <asp:BoundField AccessibleHeaderText="Name of Banner" DataField="name" HeaderText="Banner Name"
                SortExpression="name" />
            <asp:BoundField AccessibleHeaderText="Alternative Text" DataField="alttext" HeaderText="Alt Text"
                SortExpression="alttext" />
            <asp:BoundField DataField="url" HeaderText="Linked to URL" SortExpression="url" />
            <asp:BoundField DataField="category" HeaderText="Banner Category" SortExpression="category" />
            <asp:BoundField DataField="randomness" HeaderText="Randomness" SortExpression="randomness" />
            <asp:CommandField ShowDeleteButton="True"  ControlStyle-CssClass="buttonDelete"  />
        </Columns>
    </asp:GridView>
    <mjjames:jsLoader id="jquery" runat="server" JSLibrary="jquery" />
	<script type="text/javascript">
		$(".buttonDelete").click(function(){
			var bDelete = confirm("Are You Sure You Want To Delete This Banner?");
			return bDelete;
		});
	</script>

</asp:Content>

