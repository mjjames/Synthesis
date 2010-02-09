using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Reflection;
using System.Configuration;
using mjjames.core.dataentities;
using mjjames.core;
using System.Xml;
using System.Web.UI.HtmlControls;

namespace mjjames.AdminSystem.dataControls
{
	public class FileControl : IDataControl
	{
		public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			Control ourFileControl = ourControl.Parent.FindControl(ourControl.ID.Replace("control", "hidden"));
			HiddenField ourHiddenFile = (HiddenField)ourFileControl;
			return Convert.ChangeType(ourHiddenFile.Value, ourType);
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{

			ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);

			UpdatePanel fileUpload = new UpdatePanel();
			FileUpload ourUploader = new FileUpload();
			Button uploadButton = new Button();
			HiddenField fileHidden = new HiddenField();
            Button clearFile = new Button();
			HtmlControl script = new HtmlGenericControl("script");

			fileUpload.ID = "panel" + field.ID;

			script.Attributes.Add("type", "text/javascript");
			script.Controls.Add(new LiteralControl("$('.uploadSubmit"+field.ID+"').click(function(){ if($('.uploaderFile" + field.ID + "')[0].value === ''){ alert('Please Ensure a File Is Provided Before Uploading'); return false; }else{ return true;}});"));
			

			uploadButton.ID = "button" + field.ID;
			uploadButton.Text = "Upload";
			uploadButton.Click += FileUploader;
			uploadButton.CommandName = "submit";
			uploadButton.CssClass = "uploadSubmit" + field.ID;
			ourUploader.ID = "control" + field.ID;
			ourUploader.CssClass = "uploaderFile" + field.ID;
			fileHidden.ID = "hidden" + field.ID;
            clearFile.ID = "buttonClear" + field.ID;
            clearFile.Click += ClearFileValue;
            clearFile.Visible = false;

			fileUpload.UpdateMode = UpdatePanelUpdateMode.Conditional;
			if (ourSM != null) ourSM.RegisterPostBackControl(uploadButton);

			fileUpload.ContentTemplateContainer.Controls.Add(ourUploader);
			fileUpload.ContentTemplateContainer.Controls.Add(uploadButton);
			fileUpload.ContentTemplateContainer.Controls.Add(fileHidden);
			fileUpload.ContentTemplateContainer.Controls.Add(script);
            fileUpload.ContentTemplateContainer.Controls.Add(clearFile);
			
			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
			string ourFileValue = "";

			if (PKey > 0 && ourProperty != null)
			{
				ourFileValue = (string)ourProperty.GetValue(ourPage, null);
				fileHidden.Value = ourFileValue;
                if (!String.IsNullOrEmpty(fileHidden.Value))
                {
                    clearFile.Text = "Clear";
                    clearFile.Visible = true;
                }
			}

			if (field.Attributes.ContainsKey("preview"))
			{
				if (XmlConvert.ToBoolean(field.Attributes["preview"]))
				{
					Image imagePreview = new Image
					                     	{
					                     		AlternateText = "No Preview Available",
					                     		CssClass = "previewImg",
					                     		ID = "image" + field.ID,
					                     		Width = 200,
					                     		ImageUrl = null,
					                     		BorderColor = System.Drawing.Color.Black,
					                     		BorderStyle = BorderStyle.Ridge,
					                     		BorderWidth = Unit.Pixel(2)
					                     	};

					string strDir = ConfigurationManager.AppSettings["uploaddir"];
					imagePreview.ImageUrl = strDir + ourFileValue;

					fileUpload.ContentTemplateContainer.Controls.Add(imagePreview);
				}
			}
			return fileUpload;
		}

		/// <summary>
		/// Uploads the provided fileupload file
		/// </summary>
		/// <param name="sender">Button Calling Upload</param>
		/// <param name="e"></param>
		protected void FileUploader(Object sender, EventArgs e)
		{
			///TODO swap this out for mjjames.core edition
			Button ourSender = (Button)sender;

			FileUpload ourFile = (FileUpload)ourSender.Parent.FindControl(ourSender.ID.Replace("button", "control"));
			string strDir = ConfigurationManager.AppSettings["uploaddir"];

			if (ourFile == null || ourFile.PostedFile.ContentLength <= 0) return;
			FileUploadDetails fud = helpers.fileUploader(ourFile, strDir);
			if (fud.error)
			{
				LiteralControl labelStatus = new LiteralControl {Text = "Invalid File: " + fud.errormessage};
				ourSender.Parent.Controls.Add(labelStatus);
				throw new Exception("File Upload Error: " + fud.errormessage);
			}
			Image ourImage = (Image)ourSender.Parent.FindControl(ourSender.ID.Replace("button", "image"));
			HiddenField ourHiddenFile = (HiddenField)ourSender.Parent.FindControl(ourSender.ID.Replace("button", "hidden"));
			if (ourHiddenFile != null)
			{
				ourHiddenFile.Value = fud.filepath;
			}
			if (ourImage == null) return;
			ourImage.ImageUrl = strDir +  fud.filepath;
			ourImage.AlternateText = "Preview";
		}

        /// <summary>
        /// Clears the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ClearFileValue(Object sender, EventArgs e)
        {
           	Button ourSender = (Button)sender;
            HiddenField ourHiddenFile = (HiddenField)ourSender.Parent.FindControl(ourSender.ID.Replace("buttonClear", "hidden"));
            Image ourImage = ourSender.Parent.FindControl(ourSender.ID.Replace("buttonClear", "image")) as Image;
            if (ourImage != null)
            {
                ourImage.Visible = false;
            }
					
            ourHiddenFile.Value = "";
            ourSender.Visible = false;
        }

	}
}
