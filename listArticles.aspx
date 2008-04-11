<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"  ValidateRequest="false" CodeFile="listArticles.aspx.cs" Inherits="listPage" 
Title="Page Listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        SelectCommand="SELECT * FROM [articles]" 
        ConflictDetection="CompareAllValues" 
        DeleteCommand="DELETE FROM [articles] WHERE [article_key] = @article_key" 
        >
        <DeleteParameters>
            <asp:Parameter Name="article_key" Type="Int32" />
        </DeleteParameters>
    </asp:SqlDataSource>
 
    <span class="listingTitle">
        Current Articles
    </span>
    <asp:LinkButton ID="buttonAddArticle" runat="server"  CssClass="addPage"
        ToolTip="Add a New Article" PostBackUrl="~/editarticle.aspx">Add an Article</asp:LinkButton><br />
    <asp:GridView ID="articleListing" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataKeyNames="article_key" DataSourceID="SqlDataSource1"
        Style="position: relative" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" EnableViewState="False" DataMember="DefaultView" CssClass="pageListing">
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
            <asp:BoundField DataField="title" HeaderText="Page Title" SortExpression="title" />
            <asp:BoundField DataField="startdate" HeaderText="Start Date" SortExpression="startdate" />
            <asp:BoundField DataField="enddate" HeaderText="End Date" SortExpression="enddate" />
            <asp:BoundField DataField="sortorder" HeaderText="Page Sort Order" SortExpression="sortorder" />
            <asp:BoundField DataField="active" HeaderText="active" SortExpression="active" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <EmptyDataTemplate>
            Currently there are no articles within the system
        </EmptyDataTemplate>
        <FooterStyle CssClass="pageListFooter" />
        <RowStyle CssClass="pageListRow" />
        <HeaderStyle CssClass="pageListHeader" />
        <AlternatingRowStyle CssClass="pageListRowAlternate" />
    </asp:GridView>
</asp:Content>

