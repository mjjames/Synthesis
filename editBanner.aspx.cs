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

public partial class editBanner : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (ConfigurationManager.AppSettings["SiteName"] != null)
		{
			HtmlHead head = (HtmlHead)Page.Header;
			head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Edit Page";
		}
    }


	/// <summary>
	/// Checks File Size
	/// </summary>
	/// <param name="iFSize">File Size</param>
	/// <returns>Boolean indicating valid or not</returns>
	private bool checkFileSize(long iFSize)
	{
		bool bCheck = false;

		long iMaxSize = long.Parse(ConfigurationManager.AppSettings["maxFileSize"]) * 1024 * 1024;
		if (iFSize <= iMaxSize)
		{
			bCheck = true;
		}
		return bCheck;
	}

    protected void fileUpload(Object sender, EventArgs e)
    {
        FileUpload bannerUpload = (FileUpload)this.edit_addBannerForm.FindControl("bannerUpload");
        if (bannerUpload.HasFile)
        {
			if (bannerUpload.PostedFile.ContentLength > 0)
			{
				bool bValidFileType = false;
				bool bValidFileSize = false;
				try
				{
					bValidFileType = ConfigurationManager.AppSettings["fileTypes"].Contains(bannerUpload.PostedFile.ContentType.ToLower());
					bValidFileSize = checkFileSize(bannerUpload.PostedFile.ContentLength);
					if (bValidFileSize && bValidFileType)
					{
						string strDir = ConfigurationManager.AppSettings["uploaddir"] + "banners/";
						bannerUpload.SaveAs(Server.MapPath(strDir) + bannerUpload.FileName);
						Image bannerPreview = (Image)this.edit_addBannerForm.FindControl("bannerPreview");
						bannerPreview.ImageUrl = strDir + bannerUpload.FileName;
					}
					else
					{
						if (!bValidFileType)
						{
							throw new Exception("Invalid File Type");
						}
						if (!bValidFileSize)
						{
							throw new Exception("File Too Large");
						}
					}
				}
				catch (Exception ex)
				{
					labelStatus.Text = "An Error Occured While Uploading Your Banner";
					labelStatus.Text += "<br />" + ex.Message.ToString();
				}
			}
        }
    }

    protected void formview_Mode(Object sender, EventArgs e)
    {

        LinkButton UpdateCancelButton = (LinkButton)this.edit_addBannerForm.FindControl("UpdateCancelButton");
        LinkButton InsertCancelButton = (LinkButton)this.edit_addBannerForm.FindControl("InsertCancelButton");
        if (Request.QueryString["id"] != null && Request.QueryString["id"] != "")
        {
            labelStatus.Text = "Edit";
            edit_addBannerForm.ChangeMode(FormViewMode.Edit);
        }
        else
        {
            labelStatus.Text = "Insert";
            edit_addBannerForm.ChangeMode(FormViewMode.Insert);
        }
    }
    protected void edit_addBanner_ItemInserted(Object sender, FormViewInsertedEventArgs e)
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

    protected void edit_addBanner_ItemUpdated(Object sender, FormViewUpdatedEventArgs e)
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

    protected void edit_addBanner_ItemDeleted(Object sender, FormViewDeletedEventArgs e)
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

}
