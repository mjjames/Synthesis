<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="_Default" Title="Untitled Page" CodeBehind="Default.aspx.cs" %>


<asp:Content ID="homePage" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container">
        <header class="hero-unit">
            <h1>Welcome to your Admin Tool! <small>Version <%= GetVersionNumber() %></small></h1>
            <p class="lead">
                Please choose an area to administer using the navigation bar.<br />
                For more help and information refer to the <a href="help/admintool.pdf" title="MJJames Admin Tool User Guide">user guide</a>.
            </p>

        </header>
    </div>
    <div class="container">
        <div class="row">
            <div class="span12">
                <p>For all Bug Requests and for support please email <a href="mailto:cases@mjjames.co.uk">cases@mjjames.co.uk</a> and submit a report.</p>
            </div>
        </div>
    </div>
</asp:Content>

