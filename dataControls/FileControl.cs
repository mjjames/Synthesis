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
using com.flajaxian;

namespace mjjames.AdminSystem.dataControls
{
    public class FileControl : IDataControl
    {
        public int PKey { get; set; }

        public static object GetDataValue(Control ourControl, Type ourType)
        {
            //Control ourFileControl = ourControl.Parent.FindControl(ourControl.ID.Replace("control", "hidden"));
            //HiddenField ourHiddenFile = (HiddenField)ourFileControl;
            var ourHiddenFile = (HiddenField)ourControl;
            return Convert.ChangeType(ourHiddenFile.Value, ourType);
        }

        public Control GenerateControl(AdminField field, object ourPage)
        {
            var page = (Page)HttpContext.Current.Handler;
            ScriptManager ourSM = ScriptManager.GetCurrent(page);
            ClientScriptManager csm = page.ClientScript;

			Panel fileUploadPanel = new Panel();
            UpdatePanel fileUpload = new UpdatePanel();
            HiddenField fileHidden = new HiddenField();
            Button clearFile = new Button();

			fileUploadPanel.CssClass = "field fileuploadWrapper";
            fileUpload.ID = "panel" + field.ID;
            fileHidden.ID = "control" + field.ID;
            clearFile.ID = "buttonClear" + field.ID;
            clearFile.Click += ClearFileValue;
            clearFile.Visible = false;
            
            //if we have a storageservice attribute we need to set up this control to use that not the built in one
            if (field.Attributes.ContainsKey("storageservice"))
            {
                GenerateStorageServiceControl(field, page, csm, fileUpload);
            }
            else
            {
                GenerateLocalStorageControl(field, ourSM, csm, fileUpload);
            }

            fileUpload.ContentTemplateContainer.Controls.Add(fileHidden);
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
			fileUploadPanel.Controls.Add(fileUpload);
            return fileUploadPanel;
        }

        private void GenerateLocalStorageControl(AdminField field, ScriptManager ourSM, ClientScriptManager csm, UpdatePanel fileUpload)
        {
            FileUpload ourUploader = new FileUpload();
            Button uploadButton = new Button();
			
            uploadButton.Click += FileUploader;
            uploadButton.CommandName = "submit";

            csm.RegisterStartupScript(this.GetType(), "filecontrol-" + field.ID, "$('.uploadSubmit" + field.ID + "').click(function(){ if($('.uploaderFile" + field.ID +
                                                            "')[0].value === ''){ alert('Please Ensure a File Is Provided Before Uploading'); return false; }else{ return true;}});");

            uploadButton.ID = "button" + field.ID;
            uploadButton.Text = "Upload";

            uploadButton.CssClass = "uploadSubmit" + field.ID;
            ourUploader.ID = "file" + field.ID;
            ourUploader.CssClass = "uploaderFile" + field.ID;

            fileUpload.ContentTemplateContainer.Controls.Add(ourUploader);
            fileUpload.ContentTemplateContainer.Controls.Add(uploadButton);
            fileUpload.UpdateMode = UpdatePanelUpdateMode.Conditional;
            if (ourSM != null) ourSM.RegisterPostBackControl(uploadButton);
        }

        private void GenerateStorageServiceControl(AdminField field, Page page, ClientScriptManager csm, UpdatePanel fileUpload)
        {
            ////pull in our storage service js
            if (!csm.IsClientScriptIncludeRegistered("storageservice"))
            {
                csm.RegisterClientScriptInclude("storageservice", "javascript/storageservices.js");
            }

            //setup our vars
            var accessKeyID = "";
            var acl = FileAccess.PublicRead;
            var bucket = field.Attributes.ContainsKey("storagebucket") ? field.Attributes["storagebucket"] : "";
            var contentTypeStartsWith = "";
            var path = field.Attributes.ContainsKey("storagepath") ? field.Attributes["storagepath"] : "";
            var serviceHost = "";
            var secretKey = "";
            var serviceAlias = field.Attributes.ContainsKey("storagealias") ? field.Attributes["storagealias"] : "";

            switch (field.Attributes["storageservice"].ToLower())
            {
                case "amazons3":
                    serviceHost = "s3.amazonaws.com";
                    accessKeyID = ConfigurationManager.AppSettings["AmazonS3APIKEY"];
                    secretKey = ConfigurationManager.AppSettings["AmazonS3SecretKey"];
                    break;
                case "google":
                    serviceHost = "commondatastorage.googleapis.com";
                    accessKeyID = ConfigurationManager.AppSettings["GoogleStorageAPIKEY"];
                    secretKey = ConfigurationManager.AppSettings["GoogleStorageSecretKey"];
                    break;
            }

            if (String.IsNullOrEmpty(serviceAlias))
            {
                serviceAlias = String.Format("http://{0}.{1}/", bucket, serviceHost);
            }

            

            ////init the script
            csm.RegisterStartupScript(this.GetType(), "storageservice-" + field.ID, String.Format("mjjames.StorageServices.Init(\"input[id$='control{0}']\", \"{1}\");",
                field.ID, serviceAlias), true);

            //create a directamazonupload and set its properties
            var amazonAdapter = new DirectAmazonUploader()
            {
                ConfirmUploadJsFunc = "mjjames.StorageServices.SuccessfullUpload",
                SecretKey = secretKey,
                BucketName = bucket,
                AccessKey = accessKeyID,
                ServiceHost = serviceHost,
                Path = path,
                FileAccess= acl
            };
            //now create our file uploader and give it the amazon adapter
            var uploader = new FileUploader()
            {
                CssClass = "field fileuploader" + field.ID,
                ID = "fileuploader" + field.ID,
                IsSingleFileMode = true,
                UseInsideUpdatePanel = true,
                JsFunc_FileStateChanged = "mjjames.StorageServices.FileStateChanged",
                PageUrl = page.Request.Url.PathAndQuery,
                IsDebug = HttpContext.Current.IsDebuggingEnabled
            };

            uploader.Adapters.Add(amazonAdapter);
            fileUpload.ContentTemplateContainer.Controls.Add(uploader);
            
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
                LiteralControl labelStatus = new LiteralControl { Text = "Invalid File: " + fud.errormessage };
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
            ourImage.ImageUrl = strDir + fud.filepath;
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
            HiddenField ourHiddenFile = (HiddenField)ourSender.Parent.FindControl(ourSender.ID.Replace("buttonClear", "control"));
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
