using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;




public partial class editpage : System.Web.UI.Page
{
    private string pagefkey;

    protected void Page_Load(Object s, EventArgs e){

		HtmlHead head = (HtmlHead)Page.Header;
		head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Edit Page";
		
        if (Request.QueryString["fkey"] != null && Request.QueryString["fkey"] != ""){
           pagefkey = Request.QueryString["fkey"].Split((char) '?').GetValue(0).ToString();
        }

    }


    protected void formview_Mode(Object sender, EventArgs e)
    {
        
        LinkButton UpdateCancelButton = (LinkButton)this.edit_addPageForm.FindControl("UpdateCancelButton");
        LinkButton InsertCancelButton = (LinkButton)this.edit_addPageForm.FindControl("InsertCancelButton");
        if (Request.QueryString["id"]!=null && Request.QueryString["id"]!="")
        {
            labelStatus.Text = "Edit";
            edit_addPageForm.ChangeMode(FormViewMode.Edit);
            buttonSubPages.PostBackUrl = "~/listpage.aspx?fkey=" + Request.QueryString["id"];
            buttonSubPages.Visible= true; //we only want sub pages on edited pages
            HiddenField page_fkey = (HiddenField)edit_addPageForm.FindControl("page_fkeyHiddenField"); // get our hidden field for its page_fkey
            linkbuttonBack.PostBackUrl = "~/listpage.aspx?fkey=" + page_fkey.Value;
            
            UpdateCancelButton.PostBackUrl = "~/listpage.aspx?fkey=" + page_fkey.Value;
        }
        else
        {
            labelStatus.Text = "Insert";
            edit_addPageForm.ChangeMode(FormViewMode.Insert);
            linkbuttonBack.PostBackUrl = "~/listpage.aspx";// in insert mode by default we need to go back to start
        }

        if (Request.QueryString["fkey"] != null && Request.QueryString["fkey"] != "")
        {
            linkbuttonBack.PostBackUrl = "~/listpage.aspx?fkey=" + pagefkey; //however if we have an fkey then we are inserting down a level or to so we need to go back to our fkey view
            //InsertCancelButton.PostBackUrl = "~/listpage.aspx?fkey=" + Request.QueryString["fkey"]; //however if we have an fkey then we are inserting down a level or to so we need to go back to our fkey view
        }

    }

    protected void edit_addPage_ItemInserted(Object sender, FormViewInsertedEventArgs e)
    {
        
        if (e.Exception == null) //catch exception
        {
            
            if (e.AffectedRows == 1) //see if a row was inserted
            {
                labelStatus.Text = "Record inserted successfully.";
                e.KeepInInsertMode = false;
            }
            else
            {
                labelStatus.Text = "An error occurred during the insert operation.";
                e.KeepInInsertMode = true;  // when an error occurs stay in insert.
            }
        }
        else
        {
            
            labelStatus.Text = e.Exception.Message; // exception handler
            e.ExceptionHandled = false; //exception handled
            e.KeepInInsertMode = true; //when an error occurs stay in insert.
        }
    }

    protected void edit_addPage_ItemUpdated(Object sender, FormViewUpdatedEventArgs e)
    {

        if (e.Exception == null) //catch exception
        {

            if (e.AffectedRows == 1) //see if a row was inserted
            {
                labelStatus.Text = "Record updated successfully.";
            }
            else
            {
                labelStatus.Text = "An error occurred during the update operation.";
            }
        }
        else
        {

            labelStatus.Text = e.Exception.Message; // exception handler
            e.ExceptionHandled = false; //exception handled
        }
    }

    protected void edit_addPage_ItemDeleted(Object sender, FormViewDeletedEventArgs e)
    {

        if (e.Exception == null) //catch exception
        {

            if (e.AffectedRows == 1) //see if a row was inserted
            {
                labelStatus.Text = "Record deleted successfully.";
            }
            else
            {
                labelStatus.Text = "An error occurred during the delete operation.";
            }
        }
        else
        {

            labelStatus.Text = e.Exception.Message; // exception handler
            e.ExceptionHandled = false; //exception handled
        }
    }

    protected void showSubPages(Object sender, EventArgs e)
    {
        Response.Redirect("~/listpage.aspx?fkey=" + Request.QueryString["id"]);
    }
    protected void showPageList(Object sender, EventArgs e)
    {
        Response.Redirect("~/listpage.aspx?fkey=" + pagefkey);
    }

    protected void getPageFKey(Object sender, EventArgs e)
    {
        if (Request.QueryString["fkey"] != null && Request.QueryString["fkey"] != "")
        {
            HiddenField page_fkey = (HiddenField) this.edit_addPageForm.FindControl("page_fkeyHiddenField");
            page_fkey.Value = pagefkey;

        }
    }

    protected void fileUpload(Object sender, EventArgs e)
    {
        FileUpload thumbnailUpload = (FileUpload)this.edit_addPageForm.FindControl("thumbnailUpload");
        if (thumbnailUpload.HasFile)
        {
            try
            {
                if ("image/jpeg image/png image/gif".Contains(thumbnailUpload.PostedFile.ContentType.ToLower().ToString()))
                {
                    thumbnailUpload.SaveAs(Server.MapPath("/images/uploaded/thumbnails/") + thumbnailUpload.FileName);
                    Image thumbnailPreview = (Image)this.edit_addPageForm.FindControl("thumbnailPreview");
                    thumbnailPreview.ImageUrl = "/images/uploaded/thumbnails/" + thumbnailUpload.FileName;
                }
                else {
                        labelStatus.Text = "An Error Occured While Uploading Your Thumbnail, It is not a valid Image Type: JPEG, GIF or PNG ONLY";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "An Error Occured While Uploading Your Thumbnail";
                labelStatus.Text += ex.Message.ToString();
            }
        }
        else
        {
            
        }
    }
}

