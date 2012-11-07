<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="install.aspx.cs" Inherits="mjjames.AdminSystem.install.install" MasterPageFile="~/AdminSystem.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container">
        <div class="page-header">
            <h1 class="listingTitle">Initial Configuration
            </h1>
        </div>
        <div class="row">
            <div class="span12 form-horizontal">
                <asp:Panel runat="server" ID="installAlreadyComplete" Visible="false">
                    <h2>Install Already Complete</h2>
                    <p>Your database has already been configured for use. <asp:Hyperlink runat="server" NavigateUrl="~/">Continue to login</asp:Hyperlink></p>
                </asp:Panel>
                <asp:Panel runat="server" ID="installComplete" Visible="false">
                    <h2>Install Complete</h2>
                    <h3>Initial Logon Details</h3>
                    <p class="well">Username: <asp:Literal runat="server" ID="logonUsername"></asp:Literal> <br />
                        Password: <asp:Literal runat="server" ID="logonPassword"></asp:Literal> <br />
                        <asp:Hyperlink runat="server" NavigateUrl="~/">Continue to login</asp:Hyperlink></p>
                </asp:Panel>

                <asp:Panel runat="server" ID="configureDB" Visible="true">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                    <fieldset>
                        <legend>Site Details</legend>

                        <div class="control-group">
                            <asp:Label runat="server" CssClass="control-label" AssociatedControlID="siteName">Site Name</asp:Label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="siteName" CssClass="input-xlarge" />
                                <p class="help-inline">The name of the site to use in seo and identification</p>
                            </div>
                        </div>
                        <div class="control-group">
                            <asp:Label runat="server" CssClass="control-label" AssociatedControlID="siteURL">Site Url</asp:Label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="siteURL" CssClass="input-xlarge" />
                                <p class="help-inline">The url of the site, used to load the correct content etc</p>
                            </div>
                        </div>
                    </fieldset>
                    <fieldset>
                        <legend>System Admin Details</legend>
                        <div class="control-group">
                            <asp:Label runat="server" CssClass="control-label" AssociatedControlID="email">Email</asp:Label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="email" CssClass="input-xlarge" />
                                <p class="help-inline">The email address for the system admin</p>
                            </div>
                        </div>
                    </fieldset>
                    <div class="form-actions">
                        <asp:Button runat="server" UseSubmitBehavior="true" CssClass="btn-primary btn" Text="Configure" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
