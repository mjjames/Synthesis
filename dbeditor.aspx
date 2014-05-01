<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="True"
    Inherits="mjjames.AdminSystem.DBEditor" Title="DBEditor" ValidateRequest="false"
    CodeBehind="dbeditor.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SiteMapDataSource ID="navigationSiteMap" runat="server" ShowStartingNode="True"
        OnLoad="LoadListing" />
    <div class="container">
        <div class="page-header">
            <h1 class="listingTitle">
                <asp:Literal runat="server" ID="dbeditorLabel" />
                Editor <small>Edit View</small>
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

            <div id="rightCol" class="span9">
                <h4>Status:
				<asp:Label ID="labelStatus" Text="Editing" runat="server" CssClass="status"></asp:Label>
                </h4>
                <div class="listingTable">
                    <span>
                        <asp:HyperLink ID="linkbuttonBack" runat="server" Style="position: relative" ToolTip="Back to Listing"
                            Visible="true" CssClass="backListing btn btn-link">
					 &lt; &lt; Back to Listing
                        </asp:HyperLink>
                        |
                        <asp:HyperLink ID="linkButtonAddSibling" runat="server" Style="position: relative" ToolTip="Add Sibling"
                            Visible="false" CssClass="backListing btn btn-link">
					        Add Sibling Ite,
                        </asp:HyperLink>
                    </span>
                    <span class="pull-right">
                        <asp:HyperLink ID="linkbuttonSubPages" runat="server" Style="position: relative"
                            ToolTip="Show SubItems" Visible="false" CssClass="subPages btn btn-link">
					Sub Items &gt; &gt;
                        </asp:HyperLink>
                        |
                        <asp:HyperLink ID="linkButtonAddSubPage" runat="server" Style="position: relative"
                            ToolTip="Add SubItem" Visible="false" CssClass="subPages btn btn-link">
					Add Sub Item &gt; &gt;
                        </asp:HyperLink>
                    </span>
                </div>
                <div id="pageEditor" class="listingTable">
                    <asp:PlaceHolder ID="placeholderTabs" runat="server" />
                    <asp:HiddenField ID="pkey" runat="server" />
                </div>
            </div>
            <hr class="clear" />
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("img[rel=popover]").popover({
                trigger: 'hover'
            });
        });
    </script>
</asp:Content>
