<%@ Page Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true" CodeFile="editarticle.aspx.cs" Inherits="editpage" Title="Add/Edit Page"  ValidateRequest="false"%>

<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
       
<asp:Content ID="editPageContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />

    <asp:SqlDataSource ID="dsPage" runat="server" ConflictDetection="CompareAllValues"
        ConnectionString="<%$ ConnectionStrings:ourDatabase %>"
        InsertCommand="INSERT INTO [articles] ( [title], [body], [showonhome], [shortdescription], [active], [sortorder],[startdate],[enddate]) VALUES (@title, @body, @showonhome, @shortdescription, @active, @sortorder, @startdate, @enddate)"
        OldValuesParameterFormatString="original_{0}" 
        SelectCommand="SELECT * FROM [articles] WHERE [article_key] = @article_key"
        UpdateCommand="UPDATE [articles] SET [title] = @title, [body] = @body, [showonhome] = @showonhome, [shortdescription] = @shortdescription, [active] = @active, [sortorder] = @sortorder,  [startdate] = @startdate, [enddate] = @enddate WHERE [article_key] = @original_article_key ">
        <UpdateParameters>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="body" Type="String" />
            <asp:Parameter Name="showonhome" />
            <asp:Parameter Name="shortdescription" />
            <asp:Parameter Name="active" Type="Byte" />
            <asp:Parameter Name="sortorder" Type="Int32" />
            <asp:Parameter Name="startdate" />
            <asp:Parameter Name="enddate" />
            <asp:Parameter Name="original_article_key" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="title" Type="String"/>
            <asp:Parameter Name="body" Type="String" />
            <asp:Parameter Name="showonhome" />
            <asp:Parameter Name="shortdescription" />
            <asp:Parameter Name="active" Type="Boolean"  DefaultValue="True"/>
            <asp:Parameter Name="sortorder" Type="Int32" DefaultValue="0"/>
            <asp:Parameter Name="startdate" />
            <asp:Parameter Name="enddate" />
        </InsertParameters>
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="article_key" QueryStringField="id" />
        </SelectParameters>   
    </asp:SqlDataSource>
    
    <asp:LinkButton ID="linkbuttonBack" runat="server" Style="position: relative"  ToolTip="Back to Article Listing" PostBackUrl="~/listarticles.aspx" Visible="true"  EnableViewState="False" CausesValidation="false">
         &lt; &lt; Back to Article Listing
    </asp:LinkButton>
    <br />
    <div id="pageStatus">
        <p>Status: 
            <asp:Label ID="labelStatus" Text="Loaded" runat="server"></asp:Label>
        </p>
    </div>
    <asp:FormView ID="edit_addArticleForm" runat="server" DataKeyNames="article_key" DataSourceID="dsPage"
        Style="position: relative" DefaultMode="Edit" Height="600px" Width="75%" 
         OnItemInserted="edit_addArticle_ItemInserted" OnLoad="formview_Mode">
        <EditItemTemplate>
             <div id="insertContainer">
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelTitle" AssociatedControlID="titleTextBox" RunAt="server" Text="title:" />        
                    </span>
                    <span class="field">
                        <asp:TextBox ID="titleTextBox" runat="server" Text='<%# Bind("title") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="validatorTitle" runat="server" ControlToValidate="titleTextBox" ErrorMessage="Required" Style="position: relative"></asp:RequiredFieldValidator>
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelShortDescription" AssociatedControlID="shortdescriptionTextBox" RunAt="server" Text="short description:" />        
                    </span>
                    <span class="field">
                        <asp:TextBox ID="shortdescriptionTextBox" runat="server" Text='<%# Bind("shortdescription") %>'  TextMode="MultiLine"></asp:TextBox>
                    </span>
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
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelStartDate"  AssociatedControlID="textboxStartDate" runat="server" Text="Start Date" />
                    </span>
                    <span class="field">
                        <asp:TextBox ID="textboxStartDate" runat="server" visible="true" Text='<%# Bind("startdate")%>'  OnDataBinding="dateConvert" />&nbsp;
                        </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelEndDate" runat="server" Text="End Date" />
                    </span>
                    <span class="field">
                        <asp:TextBox ID="textboxEndDate" runat="server" visible="true" Text='<%# Bind("enddate") %>' />
                        
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelShowOnHome" AssociatedControlID="checkboxShowOnHome" RunAt="server" Text="show on home:" />        
                    </span>
                    <span class="field">
                        <asp:CheckBox ID="checkboxShowOnHome" runat="server" Checked='<%# Bind("showonhome") %>' Style="position: relative" />        
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelActive" AssociatedControlID="checkboxActive" RunAt="server" Text="active:" />        
                    </span>
                    <span class="field">
                        <asp:CheckBox ID="checkboxActive" runat="server" Checked='<%# Bind("active") %>'
                            Style="position: relative" />
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelSortOrder" AssociatedControlID="sortorderTextBox" RunAt="server" Text="sort order:" />        
                    </span>
                    <span class="field">
                        <asp:TextBox ID="sortorderTextBox" runat="server" Text='<%# Bind("sortorder") %>'></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="field">
                        <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                            Text="Update"></asp:LinkButton>
                        <asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="Cancel" PostBackUrl="~/listArticles.aspx"></asp:LinkButton>
                    </span>
                </div>
            </div>           
        </EditItemTemplate>
        <InsertItemTemplate>
            <div id="insertContainer">
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelTitle" AssociatedControlID="titleTextBox" RunAt="server" Text="title:" />        
                    </span>
                    <span class="field">
                        <asp:TextBox ID="titleTextBox" runat="server" Text='<%# Bind("title") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="validatorTitle" runat="server" ControlToValidate="titleTextBox" ErrorMessage="Required" Style="position: relative"></asp:RequiredFieldValidator>
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelShortDescription" AssociatedControlID="shortdescriptionTextBox" RunAt="server" Text="short description:" />        
                    </span>
                    <span class="field">
                        <asp:TextBox ID="shortdescriptionTextBox" runat="server" TextMode="MultiLine" Text='<%# Bind("shortdescription") %>' ></asp:TextBox>
                    </span>
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
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelShowOnHome" AssociatedControlID="checkboxShowOnHome" RunAt="server" Text="show on home:" />        
                    </span>
                    <span class="field">
                        <asp:CheckBox ID="checkboxShowOnHome" runat="server" Checked='<%# Bind("showonhome") %>' Style="position: relative" />        
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelActive" AssociatedControlID="checkboxActive" RunAt="server" Text="active:" />        
                    </span>
                    <span class="field">
                        <asp:CheckBox ID="checkboxActive" runat="server" Checked='<%# Bind("active") %>'
                            Style="position: relative" />
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="labelSortOrder" AssociatedControlID="sortorderTextBox" RunAt="server" Text="sort order:" />        
                    </span>
                    <span class="field">
                        <asp:TextBox ID="sortorderTextBox" runat="server" Text='<%# Bind("sortorder") %>'></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="field">
                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                            Text="Insert" ></asp:LinkButton>
                        <asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="Cancel"></asp:LinkButton>
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
    
 
</asp:Content>

