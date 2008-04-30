<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeFile="editBlog.aspx.cs" Inherits="editBlog" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<h1 class="listingTitle">
		Blog Editor
	</h1>
	<div id="colContainer">
		<iframe src="/blog/admin/pages/add_entry.aspx" id="blogEditor" width="873px" height="1000px"  frameborder="0" style="width: 100%;">
		</iframe>
	</div>
</asp:Content>