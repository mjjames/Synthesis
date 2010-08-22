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
	public class PhotogalleryControl : IDataControl
	{
		private readonly ILogger _logger = new Logger("PhotogalleryControl");
	
		public int PKey { get; set; }
		/// <summary>
		/// Key of site this photogallery is being used in
		/// </summary>
		public int SiteKey { get; set; }

		private int _maxImages = 0;

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			throw new NotImplementedException();
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			UpdatePanel panelGallery = new UpdatePanel
										{
											ID = "panelGallery-"+field.ID,
											UpdateMode = UpdatePanelUpdateMode.Conditional,
											ChildrenAsTriggers = true
										};


			_logger.LogInformation("Primary Key: " + PKey + " Total Galery Items:");
			
			//Hack: Basically if we try to load and use a photogallery before it's even saved we have no primary key
			//rather than try and save and then save this I'm going to bodge in a "save message" and look at this at a later date
			if(PKey <= 0)
			{
				panelGallery.ContentTemplateContainer.Controls.Add(
					new LiteralControl
						{
							Text = "<h2 class=\"information\">Please save your changes before adding any images to your gallery</h2>",
							ID = "control" + field.ID
						});
				return panelGallery;
			}

			AdminPhotoGallery gallery = new AdminPhotoGallery { ID = "control" + field.ID };
			gallery.Attributes.Add("cssclass", "photogalleryContainer");

			string sLookupID = field.Attributes.ContainsKey("lookupid") ? field.Attributes["lookupid"] : "galleryimage";

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

			if (field.Attributes.ContainsKey("maximages"))
			{
				int.TryParse(field.Attributes["maximages"], out _maxImages);
			}

			gallery.DataKeyNames = new[] { "key" };
			gallery.InsertItemPosition = InsertItemPosition.LastItem;
			gallery.ItemInserting += GalleryItemInserting;
			gallery.ItemUpdating += GalleryItemUpdating;
			gallery.ItemDeleted += UpdateTotalImages;
			gallery.ItemInserted += UpdateTotalImages;
			gallery.DataBound += UpdateTotalImages;

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
		}

		//registers the insert button as a full postback control
		//this is because at present we don't have an async file uploader
		//once we do we can look at making the entire photogallery propery async
		static void GalleryLoad(object sender, EventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;

			var page = (Page)HttpContext.Current.Handler;
			page.MaintainScrollPositionOnPostBack = true;
			//during a postback we find that the control has not always been databound - counter this by databinding here if we are a postback`
			if(page.IsPostBack){
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
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			if (gallery == null) return;
			FileUpload fu = helpers.FindControlRecursive(gallery.EditItem, "fileupload") as FileUpload;
			TextBox title = helpers.FindControlRecursive(gallery.EditItem, "txtTitle") as TextBox;
			TextBox desc = helpers.FindControlRecursive(gallery.EditItem, "txtDescription") as TextBox;
			//TextBox alttag = helpers.FindControlRecursive(gallery.EditItem, "txtAltTag") as TextBox;
			if (title == null || desc == null) return;
			MediaInfo pi = new MediaInfo { Title = title.Text, Description = desc.Text };
			//pi.AltTag = alttag.Text;

			//if we don't have a file something is wrong so cancel
			if (fu == null)
			{
				e.Cancel = true;
			}

			if (fu.HasFile)
			{
				FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
				if (fud.error)
				{
					LiteralControl labelStatus = new LiteralControl { Text = "Invalid File: " + fud.errormessage };
					gallery.Parent.Controls.Add(labelStatus);
					e.Cancel = true;
				}
				pi.FileName = fud.filepath;
			}
			//if we aren't cancelling add the image
			if (!e.Cancel)
			{
				e.NewValues.Add("MediaInfo", pi);
			}
		}

		void GalleryItemInserting(object sender, ListViewInsertEventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			if (gallery == null)
			{
				e.Cancel = true;
				return;
			}

			FileUpload fu = helpers.FindControlRecursive(gallery.InsertItem, "fileupload") as FileUpload;
			TextBox title = helpers.FindControlRecursive(gallery.InsertItem, "txtTitle") as TextBox;
			TextBox desc = helpers.FindControlRecursive(gallery.InsertItem, "txtDescription") as TextBox;

			if (title == null || desc == null)
			{
				//something's gone wrong so STOP
				LiteralControl labelStatus = new LiteralControl { Text = "Please ensure you provide a title and description"};
				gallery.Parent.Controls.Add(labelStatus);
				e.Cancel = true;
				return;
			}

			FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
			if (fud.error)
			{
				//something's gone wrong so STOP
				LiteralControl labelStatus = new LiteralControl {Text = "Invalid File: " + fud.errormessage};
				gallery.Parent.Controls.Add(labelStatus);
				e.Cancel = true;
				return;
				//throw new Exception("File Upload Error: " + fud.errormessage);
			}
			//all is well if we get here so do your stuff
			MediaInfo pi = new MediaInfo { Title = title.Text, Description = desc.Text, FileName = fud.filepath };
			//pi.AltTag = alttag.Text;
			e.Values.Add("MediaInfo", pi);
			e.Values.Add("LinkKey", PKey);
		}

		void UpdateTotalImages(object sender, EventArgs e)
		{
			//0 indicates unlimited
			if(_maxImages == 0 ) return;
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			//if we have more or equal images to our max images hide the insert item
			if (gallery.Items.Count >= _maxImages)
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
