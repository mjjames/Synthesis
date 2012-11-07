<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="mjjames.AdminSystem.ListPage" CodeBehind="dblisting.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
        AsyncPostBackTimeout="60" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
        <CompositeScript>
            <Scripts>
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
                <asp:ScriptReference Name="MicrosoftAjax.js" />
                <asp:ScriptReference Name="MicrosoftAjaxWebForms.js" />
            </Scripts>
        </CompositeScript>
    </asp:ScriptManager>
    <asp:SqlDataSource ID="sdsData" runat="server" OnLoad="SetupTable" ConnectionString="<%$ ConnectionStrings:ourDatabase %>"></asp:SqlDataSource>
    <asp:SiteMapDataSource ID="navigationSiteMap" runat="server" ShowStartingNode="True"
        OnLoad="LoadListing" />
    <div class="container">
       
        <div class="page-header">
            <h1 class="listingTitle">
                <asp:Literal runat="server" ID="dbEditorLabel" />
                Editor <small>Listing View</small>
            </h1>
        </div> 
        <div class="row">
            <div id="leftCol" runat="server" class="span3">
                <div class="well sidebar-nav">
                    <ul class="nav nav-list">
                        <li class="nav-header">Quick Edit</li>
                        <li id="treeView">
                            <asp:UpdatePanel ID="treePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                <ContentTemplate>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="span9" runat="server" id="rightCol">
                <h4 runat="server" id="levelLabel">at this Level</h4>
                <div class="listingTable">
                    <asp:HyperLink ID="linkbuttonBack" runat="server" CssClass="backUp btn btn-link" ToolTip="View parent"
                        NavigateUrl="~/DBEditor.aspx?type=" Visible="true">
					View parent
                    </asp:HyperLink>
                    <asp:HyperLink ID="buttonAddPage" runat="server" CssClass="addPage btn btn-link pull-right" ToolTip="Add a New "
                        NavigateUrl="~/DBEditor.aspx?type=" Visible="false">
                        Add Page
                    </asp:HyperLink>
                </div>
                <asp:GridView ID="pageListing" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="sdsData" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                    EnableViewState="False" DataMember="DefaultView" CssClass="listingTable table table-bordered table-hover table-striped">
                    <Columns>
                    </Columns>
                    <EmptyDataTemplate>
                        Currently there are no items to edit within this section
                    </EmptyDataTemplate>
                  
                </asp:GridView>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        var deleteCheck = function () {
            $(".    ").unbind("click.deletecheck").bind("click.deletecheck", function () {
                var bDelete = confirm("Are You Sure You Want To Delete This Item?");
                return bDelete;
            });
        };
        $(function () {
            deleteCheck(); //setup delete confirmations
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(deleteCheck); //on async finish we need to redo our deleteCheck
        });
    </script>

</asp:Content>
