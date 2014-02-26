using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Reflection;
using System.Configuration;
using System.Xml;
using System.Web.UI.HtmlControls;
using com.flajaxian;
using mjjames.core;
using mjjames.core.dataentities;
using AjaxControlToolkit;

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
            HiddenField fileHidden = new HiddenField();
            Button clearFile = new Button();

            fileUploadPanel.CssClass = "field fileuploadWrapper";
            fileHidden.ID = "control" + field.ID;
            clearFile.ID = "buttonClear" + field.ID;
            clearFile.Click += ClearFileValue;
            clearFile.Visible = false;
            clearFile.CssClass = "btn btn-danger removeImage";

            //if we have a storageservice attribute we need to set up this control to use that not the built in one
            if (field.Attributes.ContainsKey("storageservice"))
            {
                GenerateStorageServiceControl(field, page, csm, fileUploadPanel);
            }
            else
            {
                GenerateLocalStorageControl(field, page, csm, fileUploadPanel);
            }

            fileUploadPanel.Controls.Add(fileHidden);
            fileUploadPanel.Controls.Add(clearFile);


            PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
            string ourFileValue = "";

            if (PKey > 0 && ourProperty != null)
            {
                ourFileValue = (string)ourProperty.GetValue(ourPage, null);
                fileHidden.Value = ourFileValue;
                if (!String.IsNullOrEmpty(fileHidden.Value))
                {
                    clearFile.Text = "Remove Item";
                    clearFile.Visible = true;
                }
            }

            if (field.Attributes.ContainsKey("preview") && XmlConvert.ToBoolean(field.Attributes["preview"]))
            {

                var previewClass = "previewImg pull-right thumbnail";
                //if our field stores files to an external provider our preview requires an extra classname
                if (field.Attributes.ContainsKey("storageservice"))
                {
                    previewClass += " storageservicefile";
                }
                Image imagePreview = new Image
                {
                    AlternateText = "No Preview Available",
                    CssClass = previewClass,
                    ID = "image" + field.ID,
                    ImageUrl = null,
                    BorderColor = System.Drawing.Color.Black,
                    BorderStyle = BorderStyle.Ridge,
                    BorderWidth = Unit.Pixel(2),
                };
                SetImagePath(ourFileValue, imagePreview);
                

                imagePreview.Style.Add("max-height", "50px");
                imagePreview.Attributes.Add("data-original-title", "Preview: " + field.Label);
                imagePreview.Attributes.Add("rel", "popover");


                fileUploadPanel.Controls.Add(imagePreview);
            }

            return fileUploadPanel;
        }

        private static void UpdateImageSource(Image imagePreview, string imageUrl)
        {
            
            var contentPath = imageUrl.StartsWith("http") ? imageUrl : VirtualPathUtility.ToAbsolute(imageUrl);
            if (contentPath.EndsWith(".pdf"))
            {
                contentPath = VirtualPathUtility.ToAbsolute("~/images/pdfpreview.png");
            }
            imagePreview.ImageUrl = contentPath;
            imagePreview.Attributes.Add("data-content", "<img src='" + contentPath  + "' />");
        }

        private void GenerateLocalStorageControl(AdminField field,Page page, ClientScriptManager csm, Panel fileUpload)
        {
            ////pull in our storage service js
            if (!csm.IsClientScriptIncludeRegistered("localstorage"))
            {
                csm.RegisterClientScriptInclude("localservice", page.ResolveClientUrl("~/javascript/localstorageservice.js"));
            }
            if (!csm.IsClientScriptIncludeRegistered("jquery.form"))
            {
                csm.RegisterClientScriptInclude("jquery.form", page.ResolveClientUrl("~/javascript/jquery.form.js"));
            }


            var isWithinStorageLimit = IsWithinStorageLimit();

            var divAppendWrapper = new HtmlGenericControl("div"){
                ID = "filecontrol-" + field.ID,
                ClientIDMode = System.Web.UI.ClientIDMode.Predictable,
                InnerHtml = "<input type=\"file\" class=\"input-medium\" id=\"uploaderFile" + field.ID + "\" name=\"file\" /> <input type=\"button\" class=\"btn\" id=\"uploadSubmit" + field.ID +"\" value=\"Upload\"/>" +
                            "   <div class=\"progress progress-striped active\">" +
                            "   <div class=\"bar\" ></div> " +
                            "   </div>"
            };
            divAppendWrapper.Attributes["class"] = "uploader";

            var divNoStorage = new HtmlGenericControl("div")
            {
                InnerHtml = "<p> No Storage Space Available</p>"
            };

            var fileType = field.Attributes.ContainsKey("mediaType") ? field.Attributes["mediaType"] : "image";

            //init
            csm.RegisterStartupScript(this.GetType(), "storageservice-" + field.ID, String.Format("mjjames.LocalStorageService.Init(\"input[id$='uploaderFile{0}']\", \"input[id$='uploadSubmit{0}']\",\"" + fileType + "\");", field.ID), true);
    

            fileUpload.Controls.Add(isWithinStorageLimit ? divAppendWrapper : divNoStorage);
        }

     
        private bool IsWithinStorageLimit()
        {
            double maxStorage = 0;
            if (!double.TryParse(ConfigurationManager.AppSettings["StorageLimitInGigabytes"], out maxStorage))
            {
                maxStorage = 2;
            }
            maxStorage = maxStorage * 1073741824; //1 gb in bytes
            var totalSize = System.IO.Directory.GetFiles(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["uploaddir"]), "*", System.IO.SearchOption.AllDirectories).Sum(x => (double)(new System.IO.FileInfo(x).Length));
            return totalSize < maxStorage;
        }

        private void GenerateStorageServiceControl(AdminField field, Page page, ClientScriptManager csm, Panel fileUpload)
        {
            ////pull in our storage service js
            if (!csm.IsClientScriptIncludeRegistered("storageservice"))
            {
                csm.RegisterClientScriptInclude("storageservice", page.ResolveClientUrl("~/javascript/storageservices.js"));
            }

            //setup our vars
            var accessKeyID = "";
            var acl = FileAccess.PublicRead;
            var bucket = field.Attributes.ContainsKey("storagebucket") ? field.Attributes["storagebucket"] : "";
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
                serviceAlias = String.Format("http://{0}.{1}/{2}/", bucket, serviceHost, path);
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
                FileAccess = acl
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
            fileUpload.Controls.Add(uploader);

        }

        private void SetImagePath(string filePath, Image imagePreview){
            string imageUrl;
            if (String.IsNullOrWhiteSpace(filePath))
            {
                imageUrl = "~/images/noimage.png";                    
            }
            else
            {
                if (filePath.StartsWith("http"))
                {
                    imageUrl = filePath;
                }
                else
                {
                    string strDir = ConfigurationManager.AppSettings["uploaddir"];
                    imageUrl = strDir + filePath;
                }
            }
            UpdateImageSource(imagePreview, imageUrl);
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
                SetImagePath("", ourImage);
            }

            ourHiddenFile.Value = "";
            ourSender.Visible = false;
        }

    }
}
