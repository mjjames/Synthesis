<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" Inherits="_Default" Title="Untitled Page" Codebehind="Default.aspx.cs" %>


<asp:Content ID="homePage" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<h1 class="listingTitle">
		Welcome to your Websites Admin Area - Version <%= GetVersionNumber() %></h1>
	<div id="adminContent">
		<p>        
			Please choose an area to administer using the navigation bar.<br />
			for more help and information refer to the <a href="help/admintool.pdf" title="MJJames Admin Tool User Guide">user guide </a>.        
		</p>
		<p> For all Bug Requests and for support please email <a href="mailto:cases@mjjames.co.uk">cases@mjjames.co.uk</a> and submit a report.</p>
	</div>
</asp:Content>

	