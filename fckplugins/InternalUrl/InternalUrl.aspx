<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InternalUrl.aspx.cs" Inherits="mjjames.AdminSystem.fckplugins.InternalUrl.InternalUrl" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Internal Page URL</title>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<meta content="noindex, nofollow" name="robots" />

	<script src="/admin/fckeditor/editor/dialog/common/fck_dialog_common.js" type="text/javascript"></script>
	<script type="text/javascript" src="http://jsresources.mjjames.co.uk/jquery/jquery.1.3.2.min.js"></script>
	<script src="internalurl.js" type="text/javascript"></script>

	<link href="/admin/fckeditor/editor/dialog/common/fck_dialog_common.css" rel="stylesheet"
		type="text/css" />

	<script type="text/javascript">
		document.write(FCKTools.GetStyleHtml(GetCommonDialogCss()));
	</script>

</head>
<body style="overflow: hidden">
	<form id="form1" runat="server">
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
	<asp:SiteMapDataSource ID="navigationSiteMap" runat="server" ShowStartingNode="True"
		OnLoad="LoadListing" />
		<div id="divInfo">
			<table cellspacing="1" cellpadding="1" width="100%" border="0">
				<tr>
					<td>
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td width="100%">
									<span fcklang="DlgSelectedURL">Selected URL</span>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<input id="txtUrl" style="width: 100%" type="text" value="" readonly="readonly" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td>
						<asp:UpdatePanel ID="treePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
							<ContentTemplate>
							</ContentTemplate>
						</asp:UpdatePanel>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
