using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.dataControls;
using mjjames.AdminSystem.DataEntities;
using com.flajaxian;
using System.IO;

namespace mjjames.AdminSystem.tests
{
	public partial class MediaGallery : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ph.Controls.Add(new MediaControl
			{
				PKey = 1,
				SiteKey = 1
			}.GenerateControl(new dataentities.AdminField
			{
				ID = "mediaTest",
				Label = "mediaTest",
				Type = "media",
				Attributes = new Dictionary<string, string>()
			}, new page
			{
				
			}));


			//var serverAdapter = new FileSaverAdapter();

			//serverAdapter.FileNameDetermining += new EventHandler<FileNameDeterminingEventArgs>(serverAdapter_FileNameDetermining);

			////now create our file uploader 
			//var uploader = new FileUploader()
			//{
			//    CssClass = "field fileuploader",
			//    ID = "fileuploader",
			//    IsSingleFileMode = true,
			//    //UseInsideUpdatePanel = true,
			//    //JsFunc_FileStateChanged = "mjjames.StorageServices.FileStateChanged",
			//    PageUrl = Request.Url.PathAndQuery,
			//    IsDebug = HttpContext.Current.IsDebuggingEnabled,
			//    AllowedFileTypes = "Web Images (Jpeg, Gig, Png):*.jpg;*.jpeg;*.png;*.gif",
			//    MaxFileSize = "5MB"
			//};
			////give it the local server adapter
			//uploader.Adapters.Add(serverAdapter);

			//ph.Controls.Add(uploader);
		}

		void serverAdapter_FileNameDetermining(object sender, FileNameDeterminingEventArgs e)
		{
			e.FileName = Path.Combine(Server.MapPath("/uploads"), DateTime.Now.Ticks.ToString() + e.File.FileName);
		}
	}
}