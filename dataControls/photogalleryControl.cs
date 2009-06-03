using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using mjjames.ControlLibrary.AdminWebControls;
using System.Web.UI.WebControls;
using System.Configuration;
using mjjames.Imaging;
using mjjames.core;
using mjjames.core.dataentities;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class photogalleryControl
	{
		private int _iPKey;

		public int iPKey{
			get
			{
				return _iPKey;
			}
			set
			{
				_iPKey = value;
			}
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			UpdatePanel panelGallery = new UpdatePanel();
			panelGallery.ID = "panelGallery";
			panelGallery.UpdateMode = UpdatePanelUpdateMode.Conditional;
			panelGallery.ChildrenAsTriggers = true;

			AdminPhotoGallery gallery = new AdminPhotoGallery();
			gallery.ID = "control" + field.ID;
			gallery.Attributes.Add("cssclass", "photogalleryContainer");

			string sLookupID = field.Attributes.ContainsKey("lookupid") ? field.Attributes["lookupid"] : "galleryimage";

			ObjectDataSource ods = new ObjectDataSource("mjjames.AdminSystem.PhotoInfoData", "GetImages");
			ods.ID = "ods";
			ods.SelectParameters.Add("linkkey", iPKey.ToString());
			ods.SelectParameters.Add("lookupid", sLookupID);

			ods.InsertParameters.Add("lookupid", sLookupID);

			ods.InsertMethod = "SaveImages";
			ods.UpdateMethod = "UpdateImages";

			ods.DeleteMethod = "DeleteImage";
			ods.DeleteParameters.Add("linkkey", iPKey.ToString());
			ods.DeleteParameters.Add("lookupid", sLookupID);


			HttpContext.Current.Trace.Write("Primary Key: " + iPKey + " Total Galery Items:");

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
			gallery.ItemInserting += new EventHandler<ListViewInsertEventArgs>(gallery_ItemInserting);
			gallery.ItemUpdating += new EventHandler<ListViewUpdateEventArgs>(gallery_ItemUpdating);

			gallery.DataSourceID = "ods";
			gallery.ThumbResizeProperties = new ResizerImage() { Action = (ResizerImage.ResizerAction)Enum.Parse(typeof(ResizerImage.ResizerAction), sAction, true), Height = iHeight, Width = iWidth };
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

			gallery.PreviewResizeProperties = new ResizerImage() { Action = (ResizerImage.ResizerAction)Enum.Parse(typeof(ResizerImage.ResizerAction), sAction, true), Height = iHeight, Width = iWidth };

			gallery.DataBound += new EventHandler(gallery_Load);

			panelGallery.ContentTemplateContainer.Controls.Add(gallery);
			panelGallery.ContentTemplateContainer.Controls.Add(ods);

			return panelGallery;
		}

		void gallery_Load(object sender, EventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);

			ourSM.RegisterPostBackControl(gallery.InsertItem);

		}

		void gallery_ItemUpdating(object sender, ListViewUpdateEventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			FileUpload fu = helpers.FindControlRecursive(gallery.EditItem, "fileupload") as FileUpload;
			TextBox title = helpers.FindControlRecursive(gallery.EditItem, "txtTitle") as TextBox;
			TextBox desc = helpers.FindControlRecursive(gallery.EditItem, "txtDescription") as TextBox;
			//TextBox alttag = helpers.FindControlRecursive(gallery.EditItem, "txtAltTag") as TextBox;
			PhotoInfo pi = new PhotoInfo();
			//pi.AltTag = alttag.Text;
			pi.Title = title.Text;
			pi.Description = desc.Text;

			if (fu.HasFile)
			{
				FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
				if (fud.error)
				{
					LiteralControl labelStatus = new LiteralControl();
					labelStatus.Text = "Invalid File: " + fud.errormessage;
					gallery.Parent.Controls.Add(labelStatus);
					throw new Exception("File Upload Error: " + fud.errormessage);
				}
				pi.FileName = fud.filepath;

			}
			e.NewValues.Add("PhotoInfo", pi);
		}

		void gallery_ItemInserting(object sender, ListViewInsertEventArgs e)
		{
			AdminPhotoGallery gallery = sender as AdminPhotoGallery;
			FileUpload fu = helpers.FindControlRecursive(gallery.InsertItem, "fileupload") as FileUpload;
			TextBox title = helpers.FindControlRecursive(gallery.InsertItem, "txtTitle") as TextBox;
			TextBox desc = helpers.FindControlRecursive(gallery.InsertItem, "txtDescription") as TextBox;
			TextBox alttag = helpers.FindControlRecursive(gallery.InsertItem, "txtAltTag") as TextBox;
			FileUploadDetails fud = helpers.fileUploader(fu, gallery.FileUploadPath);
			if (fud.error)
			{
				LiteralControl labelStatus = new LiteralControl();
				labelStatus.Text = "Invalid File: " + fud.errormessage;
				gallery.Parent.Controls.Add(labelStatus);
				throw new Exception("File Upload Error: " + fud.errormessage);
			}
			PhotoInfo pi = new PhotoInfo();
			//pi.AltTag = alttag.Text;
			pi.Title = title.Text;
			pi.Description = desc.Text;
			pi.FileName = fud.filepath;
			e.Values.Add("PhotoInfo", pi);
			e.Values.Add("LinkKey", _iPKey);
		}
	}
}
