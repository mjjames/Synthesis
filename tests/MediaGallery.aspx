<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MediaGallery.aspx.cs" Inherits="mjjames.AdminSystem.tests.MediaGallery" %>
<%@ Register TagPrefix="mjjames" Namespace="mjjames" Assembly="mjjames.AdminSystem" %>
<%@ Register
Assembly="AjaxControlToolkit" 
Namespace="AjaxControlToolkit"
TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Media Gallery Tests</title>
		<link rel="stylesheet" type="text/css" href="~/styles/web.css" />
		<script type="text/javascript" src="http://jsresources.mjjames.co.uk/jquery/jquery.1.3.2.min.js"></script>
		
</head>
<body>	
	<form id="form1" runat="server">
	<div>
		<ajaxToolkit:ToolkitScriptManager runat="server" />
		<asp:PlaceHolder runat="server" ID="ph"/>
	</div>
	</form>
</body>
</html>
