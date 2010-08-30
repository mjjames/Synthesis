using System;
using System.Web;
using System.Web.UI;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.DataControls;
using mjjames.ControlLibrary.AdminWebControls;
using System.Web.UI.WebControls;
using System.Configuration;
using mjjames.core;
using mjjames.core.dataentities;
using mjjames.AdminSystem.dataentities;
using mjjames.Imaging;

namespace mjjames.AdminSystem.dataControls
{
	/// <summary>
	/// Although very similar to the photogallery control, the media gallery control allows the use of photo's and online videos
	/// </summary>
	public class MediaControl : IDataControl
	{
		private readonly ILogger _logger = new Logger("MediaControl");

		public int PKey { get; set; }
		/// <summary>
		/// Key of site this photogallery is being used in
		/// </summary>
		public int SiteKey { get; set; }

		private int _maxMedia = 0;

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			throw new NotImplementedException();
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			UpdatePanel panelGallery = new UpdatePanel
			{
				ID = "panelMediaGallery-" + field.ID,
				UpdateMode = UpdatePanelUpdateMode.Always
			};




			_logger.LogInformation("Primary Key: " + PKey + " Total Galery Items:");

			//Hack: Basically if we try to load and use a mediagallery before it's even saved we have no primary key
			//rather than try and save and then save this I'm going to bodge in a "save message" and look at this at a later date
			if (PKey <= 0)
			{
				panelGallery.ContentTemplateContainer.Controls.Add(
					new LiteralControl
					{
						Text = "<h2 class=\"information\">Please save your changes before proceeding</h2>",
						ID = "control" + field.ID
					});
				return panelGallery;
			}

			var gallery = new AdminMediaGallery { ID = "control" + field.ID };
			gallery.Attributes.Add("cssclass", "mediagalleryContainer");

			string sLookupID = field.Attributes.ContainsKey("lookupid") ? field.Attributes["lookupid"] : "media_item";

			ObjectDataSource ods = new ObjectDataSource("mjjames.AdminSystem.MediaInfoData", "GetMedia") { ID = "ods" + field.ID };
			ods.SelectParameters.Add("linkkey", PKey.ToString());
			ods.SelectParameters.Add("lookupid", sLookupID);

			ods.InsertParameters.Add("lookupid", sLookupID);
			ods.InsertParameters.Add("siteKey", SiteKey.ToString());

			ods.InsertMethod = "SaveMedia";
			ods.UpdateMethod = "UpdateMedia";

			ods.DeleteMethod = "DeleteMedia";
			ods.DeleteParameters.Add("linkkey", PKey.ToString());
			ods.DeleteParameters.Add("lookupid", sLookupID);

			string sAction = field.Attributes.ContainsKey("thumbaction") ? field.Attributes["thumbaction"] : "resize";
			int iWidth = 90;
			int iHeight = 90;

			if (field.Attributes.ContainsKey("thumbwidth"))
			{
				int.TryParse(field.Attributes["thumbwidth"], out iWidth);
			}

			if (field.Attributes.ContainsKey("thumbheight"))
			{
				int.TryParse(field.Attributes["thumbheight"], out iHeight);
			}

			if (field.Attributes.ContainsKey("maxitems"))
			{
				int.TryParse(field.Attributes["maxitems"], out _maxMedia);
			}

			gallery.DataKeyNames = new[] { "key" };
			gallery.InsertItemPosition = InsertItemPosition.LastItem;
			gallery.ItemInserting += GalleryItemInserting;
			gallery.ItemUpdating += GalleryItemUpdating;
			gallery.ItemDeleted += UpdateTotalMedia;
			gallery.ItemInserted += UpdateTotalMedia;
			gallery.DataBound += UpdateTotalMedia;

			gallery.DataSourceID = "ods" + field.ID;
			gallery.ThumbResizeProperties = new ResizerImage { Action = (ResizerImage.ResizerAction)Enum.Parse(typeof(ResizerImage.ResizerAction), sAction, true), Height = iHeight, Width = iWidth };
			gallery.FileUploadPath = ConfigurationManager.AppSettings["uploaddir"];


			sAction = field.Attributes.ContainsKey("previewaction") ? field.Attributes["previewaction"] : "resize";
			iWidth = 90;
			iHeight = 90;

			if (field.Attributes.ContainsKey("previewwidth"))
			{
				int.TryParse(field.Attributes["previewwidth"], out iWidth);
			}

			if (field.Attributes.ContainsKey("previewheight"))
			{
				int.TryParse(field.Attributes["previewheight"], out iHeight);
			}

			gallery.PreviewResizeProperties = new ResizerImage { Action = (ResizerImage.ResizerAction)Enum.Parse(typeof(ResizerImage.ResizerAction), sAction, true), Height = iHeight, Width = iWidth };

			//the only time we can guarantee our controls are setup properly and can register the insertItem control is on prerender
			//databind only occurs on a full page load
			gallery.PreRender += new EventHandler(GalleryLoad);
			panelGallery.ContentTemplateContainer.Controls.Add(gallery);
			panelGallery.ContentTemplateContainer.Controls.Add(ods);
			return panelGallery;

			//var placeholder = new PlaceHolder();
			//placeholder.Controls.Add(gallery);
			//placeholder.Controls.Add(ods);
			//return placeholder;
		}

		////registers the insert button as a full postback control
		////this is because at present we don't have an async file uploader
		////once we do we can look at making the entire photogallery propery async
		static void GalleryLoad(object sender, EventArgs e)
		{
			var gallery = sender as AdminMediaGallery;

			var page = (Page)HttpContext.Current.Handler;
			page.MaintainScrollPositionOnPostBack = true;
			//during a postback we find that the control has not always been databound - counter this by databinding here if we are a postback`
			if (page.IsPostBack)
			{
				gallery.DataBind();
			}

			ScriptManager ourSM = ScriptManager.GetCurrent(page);
			//check to see that we have came here once everything is ready
			if (ourSM != null && gallery != null && gallery.InsertItem != null)
			{
				ourSM.RegisterPostBackControl(gallery.InsertItem);
			}
		}

		static void GalleryItemUpdating(object sender, ListViewUpdateEventArgs e)
		{
			var gallery = sender as AdminMediaGallery;
			if (gallery == null) return;
			
			FileUpload fu = helpers.FindControlRecursive(gallery.EditItem, "fileupload") as FileUpload;
			TextBox title = helpers.FindControlRecursive(gallery.EditItem, "txtTitle") as TextBox;
			TextBox desc = helpers.FindControlRecursive(gallery.EditItem, "txtDescription") as TextBox;
			HiddenField file = helpers.FindControlRecursive(gallery.EditItem, "hiddenFilePath") as HiddenField;
			TextBox videoURL = helpers.FindControlRecursive(gallery.EditItem, "videoURL") as TextBox;

			var filePath = file.Value;

			//if we have a null field something is wrong
			if (title == null || desc == null || file == null)
			{
				e.Cancel = true;
				return;
			}
			
			if (fu.HasFile)
			{
				FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
				if (fud.error)
				{
					//something's gone wrong so STOP
					LiteralControl labelStatus = new LiteralControl { Text = "Invalid File: " + fud.errormessage };
					gallery.Parent.Controls.Add(labelStatus);
					e.Cancel = true;
					return;
					//throw new Exception("File Upload Error: " + fud.errormessage);
				}
				filePath = fud.filepath;
			}
			else if(!String.IsNullOrEmpty(videoURL.Text))
			{
				filePath = helpers.MakeFriendlyVideoURL(videoURL.Text);
			}
			MediaInfo pi = new MediaInfo { Title = title.Text, Description = desc.Text, FileName = filePath };
			//if we aren't cancelling add the image
			if (!e.Cancel)
			{
				e.NewValues.Add("MediaInfo", pi);
			}
		}

		void GalleryItemInserting(object sender, ListViewInsertEventArgs e)
		{
			var gallery = sender as AdminMediaGallery;
			if (gallery == null)
			{
				e.Cancel = true;
				return;
			}

			FileUpload fu = helpers.FindControlRecursive(gallery.InsertItem, "fileupload") as FileUpload;
			TextBox title = helpers.FindControlRecursive(gallery.InsertItem, "txtTitle") as TextBox;
			TextBox desc = helpers.FindControlRecursive(gallery.InsertItem, "txtDescription") as TextBox;
			TextBox videoURL = helpers.FindControlRecursive(gallery.InsertItem, "videoURL") as TextBox;

			var filePath = "";

			if (title == null || desc == null)
			{
				//something's gone wrong so STOP
				LiteralControl labelStatus = new LiteralControl { Text = "Please ensure you provide a title and description" };
				gallery.Parent.Controls.Add(labelStatus);
				e.Cancel = true;
				return;
			}
			if (fu.HasFile)
			{
				FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
				if (fud.error)
				{
					//something's gone wrong so STOP
					LiteralControl labelStatus = new LiteralControl { Text = "Invalid File: " + fud.errormessage };
					gallery.Parent.Controls.Add(labelStatus);
					e.Cancel = true;
					return;
					//throw new Exception("File Upload Error: " + fud.errormessage);
				}
				filePath = fud.filepath;
			}
			else
			{
				filePath = helpers.MakeFriendlyVideoURL(videoURL.Text);
			}
			//all is well if we get here so do your stuff
			MediaInfo mi = new MediaInfo { Title = title.Text, Description = desc.Text, FileName = filePath };
			//pi.AltTag = alttag.Text;
			e.Values.Add("MediaInfo", mi);
			e.Values.Add("LinkKey", PKey);
		}

		void UpdateTotalMedia(object sender, EventArgs e)
		{
			//0 indicates unlimited
			if (_maxMedia == 0) return;
			var gallery = sender as AdminMediaGallery;
			//if we have more or equal images to our max images hide the insert item
			if (gallery.Items.Count >= _maxMedia)
			{
				gallery.InsertItemPosition = InsertItemPosition.None;
			}
			else
			{
				gallery.InsertItemPosition = InsertItemPosition.LastItem;
			}
		}
	}
}
