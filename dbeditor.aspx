<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="True"
    Inherits="mjjames.AdminSystem.DBEditor" Title="DBEditor" ValidateRequest="false"
    CodeBehind="dbeditor.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <AjaxControlToolkit:ToolkitScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
        AsyncPostBackTimeout="60" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
        <CompositeScript>
            <%--<Scripts>
				<asp:ScriptReference Name="WebForms.js" Assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
				<asp:ScriptReference Name="MicrosoftAjax.js" />
				<asp:ScriptReference Name="MicrosoftAjaxWebForms.js" />
				<asp:ScriptReference Name="Common.Common.js" Assembly="AjaxControlToolkit" />
				<asp:ScriptReference Name="ExtenderBase.BaseScripts.js" Assembly="AjaxControlToolkit" />
				<asp:ScriptReference Name="Tabs.Tabs.js" Assembly="AjaxControlToolkit" />
				<asp:ScriptReference Name="DynamicPopulate.DynamicPopulateBehavior.js"
					Assembly="AjaxControlToolkit" />
			</Scripts>--%>
        </CompositeScript>
    </AjaxControlToolkit:ToolkitScriptManager>
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
                    <asp:HyperLink ID="linkbuttonBack" runat="server" Style="position: relative" ToolTip="Back to Listing"
                        Visible="true" CssClass="backListing btn btn-link">
					 &lt; &lt; Back to Listing
                    </asp:HyperLink>
                    <asp:HyperLink ID="linkbuttonSubPages" runat="server" Style="position: relative"
                        ToolTip="Show SubItems" Visible="false" CssClass="subPages pull-right btn btn-link">
					Sub Items &gt; &gt;
                    </asp:HyperLink>
                </div>
                <div id="pageEditor" class="listingTable">
                    <asp:PlaceHolder ID="placeholderTabs" runat="server" />
                    <asp:HiddenField ID="pkey" runat="server" />
                </div>
            </div>
            <hr class="clear" />
        </div>
    </div>
</asp:Content>
