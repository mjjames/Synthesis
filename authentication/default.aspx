<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="mjjames.AdminSystem.authentication.Login" Title="Untitled Page" EnableEventValidation="true" CodeBehind="default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container">
        <div class="page-header">
            <h1 class="listingTitle">
                <asp:Label runat="server" ID="lblMsg" Text="System Login: " />
            </h1>
        </div>
        <div class="row">


            <div class="span6 offset3">
                <div id="loginForm" class="form-horizontal">
                    <div class="control-group">
                        <label runat="server" id="labelUserName" for="inputUserName" class="control-label">
                            Username:</label>
                        <div class="controls">
                            <input runat="server" id="inputUserName" type="text" class="field" placeholder="name@email.com" />
                            <asp:RequiredFieldValidator ControlToValidate="inputUserName" Display="Static" ErrorMessage="*"
                                runat="server" ID="vUserName" CssClass="validate" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label runat="server" id="labelPassword" for="inputPassword" class="control-label">
                            Password:</label>
                        <div class="controls">
                            <input runat="server" id="inputPassword" type="password" class="field" placeholder="your password" />
                            <asp:RequiredFieldValidator ControlToValidate="inputPassword" Display="Static" ErrorMessage="*"
                                runat="server" ID="vPassword" CssClass="validate" />
                        </div>
                    </div>
                    <div class="control-group">
                        <div class="controls">
                            <input runat="server" id="btnLogon" type="submit" value="Logon" onserverclick="btnLogin_Click" class="submit btn btn-primary btn-large" />
                            <asp:HiddenField runat="server" ID="returnUrl"/>
                        </div>
                    </div>
                </div>
                <div id="centerBG">
                    &nbsp;
                </div>
            </div>
        </div>
    </div>
</asp:Content>
