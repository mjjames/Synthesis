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
    
    protected void Page_Load(Object s, EventArgs e){
		HtmlHead head = (HtmlHead)Page.Header;
		head.Title = ConfigurationManager.AppSettings["SiteName"].ToString() + ": Admin - Edit Article";
    }



    protected void formview_Mode(Object sender, EventArgs e)
    {

        if (Request.QueryString["id"]!=null && Request.QueryString["id"]!="")
        {
            labelStatus.Text = "Edit";
            edit_addArticleForm.ChangeMode(FormViewMode.Edit);          
        }
        else
        {
            labelStatus.Text = "Insert";
            edit_addArticleForm.ChangeMode(FormViewMode.Insert);
        }
    }

    protected void edit_addArticle_ItemInserted(Object sender, FormViewInsertedEventArgs e)
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
            e.ExceptionHandled = true; //exception handled
            e.KeepInInsertMode = true; //when an error occurs stay in insert.
        }
    }

    protected void showPageList(Object sender, EventArgs e)
    {
        Response.Redirect("~/listarticles.aspx");
    }


    protected void dateConvert(Object sender, EventArgs e)
    {
        TextBox date = (TextBox)sender;
        string strDate = date.Text.ToString();
        DateTime dt = DateTime.Parse(strDate);
        date.Text = dt.ToShortDateString();
    }
}

