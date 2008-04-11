<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeFile="editBanner.aspx.cs" Inherits="editBanner" Title="Admin Tool - Edit / Insert Banners" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server"><asp:SqlDataSource ID="sqlBanners" runat="server" ConflictDetection="CompareAllValues"
        ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        DeleteCommand="DELETE FROM [banners] WHERE [bannerdid] = @original_bannerdid AND [name] = @original_name AND [image] = @original_image AND [alttext] = @original_alttext AND [url] = @original_url AND [category] = @original_category AND [randomness] = @original_randomness"
        InsertCommand="INSERT INTO [banners] ([name], [image], [alttext], [url], [category], [randomness]) VALUES (@name, @image, @alttext, @url, @category, 0 + @randomness)"
        OldValuesParameterFormatString="original_{0}" 
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM [banners] WHERE [bannerdid] = @bannerid"
        UpdateCommand="UPDATE [banners] SET [name] = @name, [image] = @image, [alttext] = @alttext, [url] = @url, [category] = @category, [randomness] = 0 + @randomness WHERE [bannerdid] = @original_bannerdid">
        <DeleteParameters>
            <asp:Parameter Name="original_bannerdid" Type="Int32" />
            <asp:Parameter Name="original_name" Type="String" />
            <asp:Parameter Name="original_image" Type="String" />
            <asp:Parameter Name="original_alttext" Type="String" />
            <asp:Parameter Name="original_url" Type="String" />
            <asp:Parameter Name="original_category" Type="String" />
            <asp:Parameter Name="original_randomness" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="name" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="image" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="alttext" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="url" Type="String" ConvertEmptyStringToNull="false" />
            <asp:Parameter Name="category" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="randomness" Type="Int32" />
            <asp:QueryStringParameter Name="original_bannerdid" Type="Int32" ConvertEmptyStringToNull="false"  QueryStringField="id"/>
            <asp:Parameter Name="original_name" Type="String" />
            <asp:Parameter Name="original_image" Type="String" />
            <asp:Parameter Name="original_alttext" Type="String" />
            <asp:Parameter Name="original_url" Type="String" />
            <asp:Parameter Name="original_category" Type="String" />
            <asp:Parameter Name="original_randomness" Type="Int32" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="name" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="image" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="alttext" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="url" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="category" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:Parameter Name="randomness" Type="Int32"  DefaultValue="1"/>
        </InsertParameters>
        <SelectParameters>
            <asp:QueryStringParameter Name="bannerid" Type="Int32" ConvertEmptyStringToNull="true"  QueryStringField="id"/>
        </SelectParameters>
    </asp:SqlDataSource> 
    <div id="pageStatus">
        <p class="labelStatus">
            <asp:Label ID="labelStatus" Text="Loaded" runat="server" CssClass="status"></asp:Label>
        </p>
    </div>
<asp:FormView ID="edit_addBannerForm" runat="server" DataKeyNames="bannerdid" DataSourceID="sqlBanners"
OnItemInserted="edit_addBanner_ItemInserted" OnLoad="formview_Mode" OnItemUpdated="edit_addBanner_ItemUpdated" OnItemDeleted="edit_addBanner_ItemDeleted">
<EditItemTemplate>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelName" AssociatedControlID="nameTextBox" RunAt="server" Text="Banner Name: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    
    <div class="row">
        <span class="label">
            <asp:Label ID="labelAltText" AssociatedControlID="alttextTextBox" RunAt="server" Text="Alt Text: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="alttextTextBox" runat="server" Text='<%# Bind("alttext") %>'> </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelURL" AssociatedControlID="urlTextBox" RunAt="server" Text="URL: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="urlTextBox" runat="server" Text='<%# Bind("url") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
     <div class="row">
        <span class="label">
            <asp:Label ID="labelCategory" AssociatedControlID="categoryTextBox" RunAt="server" Text="Category: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="categoryTextBox" runat="server" Text='<%# Bind("category") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
   
    <div class="row">
        <span class="label">
            <asp:Label ID="labelRandomness" AssociatedControlID="randomnessTextBox" RunAt="server" Text="Randomness: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="randomnessTextBox" runat="server" Text='<%# Bind("randomness") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    
    <div class="row">
        <span class="label">
            <asp:Label ID="labelImage" AssociatedControlID="bannerUpload" RunAt="server" Text="Image: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:FileUpload ID="bannerUpload" runat="server" ToolTip="A banner image suitable for this page" />
            <asp:Button ID="bannerButton" runat="server" Text="Upload banner" OnClick="fileUpload" />
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="previewbanner">
        <h4 class="title">Preview</h4>
        <asp:Image ID="bannerPreview" runat="server"  ImageUrl='<%# Bind("image") %>' AlternateText="Banner Preview" BackColor="#FFFFFF" CssClass="previewbannerIMG" Width="600px" Height="112px"/>
    </div>
    <div class="row buttons">
        <asp:LinkButton ID="UpdateButton" runat="server" 
            CausesValidation="True" CommandName="Update" Text="Update">
        </asp:LinkButton> 
        <asp:LinkButton ID="UpdateCancelButton" runat="server" 
            CausesValidation="False" CommandName="Cancel" Text="Cancel" PostBackUrl="~/listBanners.aspx">
        </asp:LinkButton> 
    </div>
</EditItemTemplate>
<InsertItemTemplate>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelName" AssociatedControlID="nameTextBox" RunAt="server" Text="Banner Name: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelAltText" AssociatedControlID="alttextTextBox" RunAt="server" Text="Alt Text: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="alttextTextBox" runat="server" Text='<%# Bind("alttext") %>'> </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelURL" AssociatedControlID="urlTextBox" RunAt="server" Text="URL: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="urlTextBox" runat="server" Text='<%# Bind("url") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
     <div class="row">
        <span class="label">
            <asp:Label ID="labelCategory" AssociatedControlID="categoryTextBox" RunAt="server" Text="Category: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="categoryTextBox" runat="server" Text='<%# Bind("category") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
   
    <div class="row">
        <span class="label">
            <asp:Label ID="labelRandomness" AssociatedControlID="randomnessTextBox" RunAt="server" Text="Randomness: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="randomnessTextBox" runat="server" Text='<%# Bind("randomness") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelImage" AssociatedControlID="bannerUpload" RunAt="server" Text="Image: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:FileUpload ID="bannerUpload" runat="server" ToolTip="A banner image suitable for this page" />
            <asp:Button ID="bannerButton" runat="server" Text="Upload banner" OnClick="fileUpload" />
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="previewbanner">
            <h4 class="title">Preview</h4>
            <asp:Image ID="bannerPreview" runat="server"  ImageUrl='<%# Bind("image") %>' AlternateText="Banner Preview" BackColor="#FFFFFF" CssClass="previewbannerIMG" Width="600px" Height="112px"/>
    </div>

    <div class="row buttons">
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                Text="Insert">
            </asp:LinkButton>
            <asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                Text="Cancel">
            </asp:LinkButton>
    </div>
</InsertItemTemplate>
<ItemTemplate>
        <div class="row">
        <span class="label">
            <asp:Label ID="labelName" AssociatedControlID="nameTextBox" RunAt="server" Text="Banner Name: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    
    <div class="row">
        <span class="label">
            <asp:Label ID="labelAltText" AssociatedControlID="alttextTextBox" RunAt="server" Text="Alt Text: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="alttextTextBox" runat="server" Text='<%# Bind("alttext") %>'> </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelURL" AssociatedControlID="urlTextBox" RunAt="server" Text="URL: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="urlTextBox" runat="server" Text='<%# Bind("url") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="row">
        <span class="label">
            <asp:Label ID="labelCategory" AssociatedControlID="categoryTextBox" RunAt="server" Text="Category: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="categoryTextBox" runat="server" Text='<%# Bind("category") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
   
    <div class="row">
        <span class="label">
            <asp:Label ID="labelRandomness" AssociatedControlID="randomnessTextBox" RunAt="server" Text="Randomness: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:TextBox ID="randomnessTextBox" runat="server" Text='<%# Bind("randomness") %>'>
            </asp:TextBox>
        </span>
        <div class="clear"> </div> 
    </div>
    
    <div class="row">
        <span class="label">
            <asp:Label ID="labelImage" AssociatedControlID="bannerUpload" RunAt="server" Text="Image: " CssClass="editLabel" />
        </span>
        <span class="field">
            <asp:FileUpload ID="bannerUpload" runat="server" ToolTip="A banner image suitable for this page" />
            <asp:Button ID="bannerButton" runat="server" Text="Upload banner" OnClick="fileUpload" />
        </span>
        <div class="clear"> </div> 
    </div>
    <div class="previewbanner">
        <h4 class="title">Preview</h4>
        <asp:Image ID="bannerPreview" runat="server"  ImageUrl='<%# Bind("image") %>' AlternateText="Banner Preview" BackColor="#FFFFFF" CssClass="previewbannerIMG" Width="600px" Height="112px"/>
    </div>
    
    <div class="row buttons">
        <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit"
            Text="Edit">
        </asp:LinkButton>
        <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete"
            Text="Delete">
        </asp:LinkButton>
        <asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" CommandName="New"
            Text="New">
        </asp:LinkButton>
    </div>    
</ItemTemplate>
</asp:FormView>
</asp:Content>

