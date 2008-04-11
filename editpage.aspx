<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeFile="editpage.aspx.cs" Inherits="editpage" Title="Admin Tool - Add/Edit Page"  ValidateRequest="false"%>
<%@ Import Namespace="mjjames" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<asp:Content ID="editPageContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:LinkButton ID="buttonSubPages" runat="server" Style="position: relative"  CausesValidation="false" OnClick="showSubPages" ToolTip="Show SubPages" PostBackUrl="#" Visible="false"  EnableViewState="False" CssClass="subPages">Sub Pages
 &gt; &gt;</asp:LinkButton>
    <asp:LinkButton ID="linkbuttonBack" runat="server" Style="position: relative"  ToolTip="Back to Page Listing" PostBackUrl="#" Visible="true"  EnableViewState="False" CausesValidation="false" CssClass="backListing">
         &lt; &lt; Back to Page Listing
    </asp:LinkButton>
    <br />
    <asp:SqlDataSource ID="dsPage" runat="server" ConflictDetection="CompareAllValues"
        ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        InsertCommand="INSERT INTO [page]  ([page_fkey], [navtitle], [metadescription],[metakeywords], [title], [thumbnailimage],[body], [showinnav], [showinfooter], [showonhome], [showinfeaturednav], [active], [sortorder], [accesskey]) 
                       VALUES              (@page_fkey, @navtitle, @metadescription, @metakeywords, @title, @thumbnailimage, @body, @showinnav, @showinfooter, @showonhome, @showinfeaturednav, @active, @sortorder, @accesskey)"
        OldValuesParameterFormatString="original_{0}" 
        SelectCommand="SELECT [page_key], [page_fkey], [navtitle], [metadescription],[metakeywords], [title], [body], [showinnav], [showinfooter], [showonhome], [showinfeaturednav], [active], [sortorder], [accesskey], [thumbnailimage]  FROM [page] WHERE [page_key] = @page_key"
        UpdateCommand="UPDATE [page] SET [navtitle] = @navtitle, [metadescription] = @metadescription, [metakeywords] = @metakeywords, [title] = @title, [thumbnailimage] = @thumbnailimage, [body] = @body, [showinnav] = @showinnav, [showinfooter] = @showinfooter, [showonhome] = @showonhome, [showinfeaturednav] = @showinfeaturednav, [active] = @active, [sortorder] = @sortorder, [accesskey] = @accesskey WHERE [page_key] = @original_page_key ">
        <DeleteParameters>
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="navtitle" Type="String" />
            <asp:Parameter Name="metadescription" Type="String" />
            <asp:Parameter Name="metakeywords" Type="String" />
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="thumbnailimage" Type="String" />
            <asp:Parameter Name="body" Type="String" />
            <asp:Parameter Name="showinnav" Type="Byte" />
            <asp:Parameter Name="showinfooter" Type="Byte" />
            <asp:Parameter Name="showonhome" Type="Byte" />
            <asp:Parameter Name="showinfeaturednav" Type="Byte" />
            <asp:Parameter Name="active" Type="Byte" />
            <asp:Parameter Name="sortorder" Type="Int32" />
            <asp:Parameter Name="accesskey" Type="char"/>
        
            <asp:QueryStringParameter Name="original_page_key" Type="Int32" ConvertEmptyStringToNull="false"  QueryStringField="id"/>
            <asp:Parameter Name="original_page_fkey" Type="Int32"/>
            <asp:Parameter Name="original_navtitle" Type="String" />
            <asp:Parameter Name="original_metadescription" Type="String" />
            <asp:Parameter Name="original_metakeywords" Type="String" />
            <asp:Parameter Name="original_title" Type="String" />
            <asp:Parameter Name="original_thumbnailimage" Type="String" />
            <asp:Parameter Name="original_body" Type="String" />
            <asp:Parameter Name="original_showinnav" Type="Byte" />
            <asp:Parameter Name="original_showinfooter" Type="Byte" />
            <asp:Parameter Name="original_showonhome" Type="Byte" />
            <asp:Parameter Name="original_showinfeaturednav" Type="Byte" />
            <asp:Parameter Name="original_active" Type="Byte" />
            <asp:Parameter Name="original_sortorder" Type="Int32" />
            <asp:Parameter Name="original_accesskey" Type="char"/>
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="page_fkey" Type="Int32" />
            <asp:Parameter Name="navtitle" Type="String" />
            <asp:Parameter Name="metadescription" Type="String" ConvertEmptyStringToNull="true"/>
            <asp:Parameter Name="metakeywords" Type="String" ConvertEmptyStringToNull="true"/>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="thumbailimage" Type="string" />
            <asp:Parameter Name="body" Type="String" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="showinnav" Type="byte" />
            <asp:Parameter Name="showinfooter" Type="Byte" />
            <asp:Parameter Name="showonhome" Type="Byte" />
            <asp:Parameter Name="showinfeaturednav" Type="Byte" />
            <asp:Parameter Name="active" Type="Byte"/>
            <asp:Parameter Name="sortorder" Type="Int32" DefaultValue="0"/>
            <asp:Parameter Name="accesskey" Type="Char"/> 
        </InsertParameters>
        <SelectParameters>
            <asp:QueryStringParameter Name="page_key" Type="Int32" ConvertEmptyStringToNull="true"  QueryStringField="id"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SiteMapDataSource ID="navigationSiteMap" runat="server"  SiteMapProvider="adminNavigation" ShowStartingNode="True"  />
    <div id="pageStatus">
        <p class="labelStatus">Mode: 
            <asp:Label ID="labelStatus" Text="Loaded" runat="server" CssClass="status"></asp:Label>
        </p>
    </div>
     <div id="colContainer">
		<div id="leftCol">
			<asp:TreeView ID="treeListing" runat="server" DataSourceID="navigationSiteMap" PopulateNodesFromClient="true" ShowExpandCollapse="true" ShowLines="true" EnableClientScript="true" ExpandDepth="1" CssClass="treeView" />       
		</div>
		<div id="rightCol">
			<asp:FormView ID="edit_addPageForm" runat="server" DataKeyNames="page_key" DataSourceID="dsPage"
				Style="position: relative" DefaultMode="Edit" Height="600px" Width="75%" 
				 OnItemInserted="edit_addPage_ItemInserted" OnLoad="formview_Mode" OnItemUpdated="edit_addPage_ItemUpdated" OnItemDeleted="edit_addPage_ItemDeleted">
				<EditItemTemplate>
						<div id="insertContainer">
							<asp:HiddenField ID="page_fkeyHiddenField" runat="server" Value='<%# Bind("page_fkey", "{0}") %>' />
						<div class="row">
							<span class="label">
								<asp:Label ID="labelNavTitle" AssociatedControlID="navtitleTextBox" RunAt="server" Text="nav title:" CssClass="editLabel" />
							</span>
							<span class="field">
								<asp:TextBox ID="navtitleTextBox" runat="server" Text='<%# Bind("navtitle") %>' Columns="50" CssClass="editTextbox"></asp:TextBox>
								<asp:RequiredFieldValidator ID="validatorNavTitle" runat="server" ControlToValidate="navtitleTextBox" ErrorMessage="Required" ></asp:RequiredFieldValidator>
							</span>
							<div class="clear"> </div> 
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelMetaDescription" AssociatedControlID="metadescriptionTextBox" RunAt="server" Text="meta description:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:TextBox ID="metadescriptionTextBox" runat="server" Text='<%# Bind("metadescription") %>' TextMode="MultiLine" Columns="45" CssClass="editTextArea" Rows="2"></asp:TextBox>            
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelMetaKeywords" AssociatedControlID="metakeywordsTextBox" RunAt="server" Text="meta keywords:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:TextBox ID="metakeywordsTextBox" runat="server" Text='<%# Bind("metakeywords") %>' TextMode="MultiLine" Columns="45" CssClass="editTextArea" Rows="2"></asp:TextBox>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelTitle" AssociatedControlID="titleTextBox" RunAt="server" Text="title:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:TextBox ID="titleTextBox" runat="server" Text='<%# Bind("title") %>' Columns="50" CssClass="editTextbox"></asp:TextBox>
								<asp:RequiredFieldValidator ID="validatorTitle" runat="server" ControlToValidate="titleTextBox" ErrorMessage="Required" ></asp:RequiredFieldValidator>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelThumbnail" AssociatedControlID="thumbnailUpload" runat="server" Text="Thumbnail:" CssClass="editLabel" />
							</span>
							<span class="field">
								<asp:FileUpload ID="thumbnailUpload" runat="server" ToolTip="A thumbnail image suitable for this page" />
								<asp:Button ID="thumbnailButton" runat="server" Text="Upload Thumbnail" OnClick="fileUpload" />
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelPreview" AssociatedControlID="thumbnailPreview" runat="server" Text="Tumbnail Preview:" CssClass="editLabel" />
							</span>
							<span class="field thumbnail">
								<asp:Image ID="thumbnailPreview" runat="server"  ImageUrl='<%# Bind("thumbnailimage") %>' AlternateText="Thumbnail Preview" CssClass="previewThumbnailIMG" />
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelBody" AssociatedControlID="pageBody" RunAt="server" Text="body content:" CssClass="editLabel" />        
							</span>
							<span class="field">
								
								<FCKeditorV2:FCKeditor ID="pageBody" runat="server" Height="480px" BasePath="~/FCKeditor/"
								Width="600px" Value='<%# Bind("body") %>' ToolbarSet="mjjames" EnableXHTML="true"  EnableSourceXHTML="true"   >
								</FCKeditorV2:FCKeditor>     
								
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowInNav" AssociatedControlID="checkboxShowinNav" RunAt="server" Text="show in nav:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowinNav" runat="server" Checked='<%#  Bind("showinnav") %>' Style="position: relative" />        
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowInFeaturedNav" AssociatedControlID="checkboxShowinFeaturedNav" RunAt="server" Text="show in featured nav:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowInFeaturedNav" runat="server" Checked='<%# Bind("showinfeaturednav") %>' Style="position: relative" />        
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowOnHome" AssociatedControlID="checkboxShowonHome" RunAt="server" Text="show on home:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowOnHome" runat="server" Checked='<%# Bind("showonhome") %>' Style="position: relative" />        
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowInFooter" AssociatedControlID="checkboxShowinFooter" RunAt="server" Text="show in footer:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowinFooter" runat="server" Checked='<%# Bind("showinfooter") %>'
									Style="position: relative" />
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelActive" AssociatedControlID="checkboxActive" RunAt="server" Text="active:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxActive" runat="server" Checked='<%# Bind("active") %>'
									Style="position: relative" />
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelSortOrder" AssociatedControlID="sortorderTextBox" RunAt="server" Text="sort order:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:TextBox ID="sortorderTextBox" runat="server" Text='<%# Bind("sortorder") %>' ></asp:TextBox>
							</span>
							<div class="clear"> </div>
						</div>
					  <div class="row">
							<span class="label">
								<asp:Label ID="labelAccessKey" AssociatedControlID="accesskeyTextBox" RunAt="server" Text="access key:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:TextBox ID="accesskeyTextBox" runat="server" Text='<%# Bind("accesskey") %>' MaxLength="1"></asp:TextBox>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="field">
								<asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
									Text="Update" ></asp:LinkButton>
								<asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
									Text="Cancel" PostBackUrl="~/listPage.aspx"></asp:LinkButton>
							</span>
							<div class="clear"> </div>
						</div>
					</div>           
				</EditItemTemplate>
				<InsertItemTemplate>
					<div id="insertContainer">
						<asp:HiddenField ID="page_fkeyHiddenField" runat="server"  OnLoad="getPageFKey" Value='<%# Bind("page_fkey") %>'/>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelNavTitle" AssociatedControlID="navtitleTextBox" RunAt="server" Text="navtitle:" />
							</span>
							<span class="field">
								<asp:TextBox ID="navtitleTextBox" runat="server" Text='<%# Bind("navtitle") %>'></asp:TextBox>
								<asp:RequiredFieldValidator ID="validatorNavTitle" runat="server" ControlToValidate="navtitleTextBox" ErrorMessage="Required" Style="left: -3px; position: relative; top: -2px"></asp:RequiredFieldValidator>
							</span> 
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelMetaDescription" AssociatedControlID="metadescriptionTextBox" RunAt="server" Text="metadescription:" />        
							</span>
							<span class="field">
								<asp:TextBox ID="metadescriptionTextBox" runat="server" Text='<%# Bind("metadescription") %>'></asp:TextBox>            
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelMetaKeywords" AssociatedControlID="metakeywordsTextBox" RunAt="server" Text="metakeywords:" />        
							</span>
							<span class="field">
								<asp:TextBox ID="metakeywordsTextBox" runat="server" Text='<%# Bind("metakeywords") %>'></asp:TextBox>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelTitle" AssociatedControlID="titleTextBox" RunAt="server" Text="title:" />        
							</span>
							<span class="field">
								<asp:TextBox ID="titleTextBox" runat="server" Text='<%# Bind("title") %>'></asp:TextBox>
								<asp:RequiredFieldValidator ID="validatorTitle" runat="server" ControlToValidate="titleTextBox" ErrorMessage="Required" Style="position: relative"></asp:RequiredFieldValidator>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelThumbnail" AssociatedControlID="thumbnailUpload" runat="server" Text="Thumbnail:" CssClass="editLabel" />
							</span>
							<span class="field">
								<asp:FileUpload ID="thumbnailUpload" runat="server" ToolTip="A thumbnail image suitable for this page" />
								<asp:Button ID="thumbnailButton" runat="server" Text="Upload Thumbnail" OnClick="fileUpload" />
							</span>
							 <div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelPreview" AssociatedControlID="thumbnailPreview" runat="server" Text="Tumbnail Preview:" CssClass="editLabel" />
							</span>
							<span class="field">
								<asp:Image ID="thumbnailPreview" runat="server"  ImageUrl='<%# Bind("thumbnailimage") %>' AlternateText="Thumbnail Preview" CssClass="previewThumbnailIMG"/>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelBody" AssociatedControlID="pageBody" RunAt="server" Text="body content:" />        
							</span>
							<span class="field">
								<FCKeditorV2:FCKeditor ID="pageBody" runat="server" Height="480px" BasePath="~/FCKeditor/"
									Width="680px" Value='<%# Bind("body") %>' ToolbarSet="mjjames">
								</FCKeditorV2:FCKeditor>        
							</span>
							<div class="clear"> </div>
						</div>
		                
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowInNav" AssociatedControlID="checkboxShowinNav" RunAt="server" Text="show in nav:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowinNav" runat="server" Checked='<%#  Bind("showinnav") %>' Style="position: relative" />        
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowInFeaturedNav" AssociatedControlID="checkboxShowinFeaturedNav" RunAt="server" Text="show in featured nav:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowInFeaturedNav" runat="server" Checked='<%# Bind("showinfeaturednav") %>' Style="position: relative" />        
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowOnHome" AssociatedControlID="checkboxShowonHome" RunAt="server" Text="show on home:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowOnHome" runat="server" Checked='<%# Bind("showonhome") %>' Style="position: relative" />        
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelShowInFooter" AssociatedControlID="checkboxShowinFooter" RunAt="server" Text="show in footer:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxShowinFooter" runat="server" Checked='<%# Bind("showinfooter") %>'
									Style="position: relative" />
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelActive" AssociatedControlID="checkboxActive" RunAt="server" Text="active:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:CheckBox ID="checkboxActive" runat="server" Checked='<%# Bind("active") %>'
									Style="position: relative" />
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelSortOrder" AssociatedControlID="sortorderTextBox" RunAt="server" Text="sort order:" />        
							</span>
							<span class="field">
								<asp:TextBox ID="sortorderTextBox" runat="server" Text='<%# Bind("sortorder") %>'></asp:TextBox>
							</span>
							<div class="clear"> </div>
						</div>
						<div class="row">
							<span class="label">
								<asp:Label ID="labelAccessKey" AssociatedControlID="accesskeyTextBox" RunAt="server" Text="access key:" CssClass="editLabel" />        
							</span>
							<span class="field">
								<asp:TextBox ID="accesskeyTextBox" runat="server" Text='<%# Bind("accesskey") %>' MaxLength="1"></asp:TextBox>
							</span>
							<div class="clear"> </div>
						</div>

						<div class="row">
							<span class="field">
								<asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
									Text="Insert" ></asp:LinkButton>
								<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
									Text="Cancel" PostBackUrl="~/listPage.aspx"></asp:LinkButton>
							</span>
						</div>
					</div>           
		            
				</InsertItemTemplate>
				<ItemTemplate>

				</ItemTemplate>
				<EmptyDataTemplate>
					<p>No Content Loaded</p>
				</EmptyDataTemplate>
			</asp:FormView>
		</div>
	</div>
</asp:Content>

