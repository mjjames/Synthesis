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

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			throw new NotImplementedException();
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			UpdatePanel panelGallery = new UpdatePanel
										{
											ID = "panelGallery",
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

			ObjectDataSource ods = new ObjectDataSource("mjjames.AdminSystem.PhotoInfoData", "GetImages") { ID = "ods" };
			ods.SelectParameters.Add("linkkey", PKey.ToString());
			ods.SelectParameters.Add("lookupid", sLookupID);

			ods.InsertParameters.Add("lookupid", sLookupID);

			ods.InsertMethod = "SaveImages";
			ods.UpdateMethod = "UpdateImages";

			ods.DeleteMethod = "DeleteImage";
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

			gallery.DataKeyNames = new[] { "key" };
			gallery.InsertItemPosition = InsertItemPosition.LastItem;
			gallery.ItemInserting += GalleryItemInserting;
			gallery.ItemUpdating += GalleryItemUpdating;

			gallery.DataSourceID = "ods";
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

			gallery.DataBound += GalleryLoad;

			panelGallery.ContentTemplateContainer.Controls.Add(gallery);
			panelGallery.ContentTemplateContainer.Controls.Add(ods);

			return panelGallery;
		}

		static void GalleryLoad(object sender, EventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);

			if (ourSM != null && gallery != null) ourSM.RegisterPostBackControl(gallery.InsertItem);
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
			PhotoInfo pi = new PhotoInfo { Title = title.Text, Description = desc.Text };
			//pi.AltTag = alttag.Text;

			if (fu != null && fu.HasFile)
			{
				FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
				if (fud.error)
				{
					LiteralControl labelStatus = new LiteralControl { Text = "Invalid File: " + fud.errormessage };
					gallery.Parent.Controls.Add(labelStatus);
					throw new Exception("File Upload Error: " + fud.errormessage);
				}
				pi.FileName = fud.filepath;

			}
			e.NewValues.Add("PhotoInfo", pi);
		}

		void GalleryItemInserting(object sender, ListViewInsertEventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			if (gallery != null)
			{
				FileUpload fu = helpers.FindControlRecursive(gallery.InsertItem, "fileupload") as FileUpload;
				TextBox title = helpers.FindControlRecursive(gallery.InsertItem, "txtTitle") as TextBox;
				TextBox desc = helpers.FindControlRecursive(gallery.InsertItem, "txtDescription") as TextBox;
				FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
				if (fud.error)
				{
					LiteralControl labelStatus = new LiteralControl {Text = "Invalid File: " + fud.errormessage};
					gallery.Parent.Controls.Add(labelStatus);
					throw new Exception("File Upload Error: " + fud.errormessage);
				}
				if (title != null && desc != null)
				{
					PhotoInfo pi = new PhotoInfo { Title = title.Text, Description = desc.Text, FileName = fud.filepath };
					//pi.AltTag = alttag.Text;
					e.Values.Add("PhotoInfo", pi);
				}
			}

			e.Values.Add("LinkKey", PKey);
		}
	}
}
