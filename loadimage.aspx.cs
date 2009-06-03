using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Xml.Linq;
using mjjames.Imaging;
using mjjames.flickr;
using FlickrNet;
using System.Drawing;
using System.IO;

public partial class loadimage : System.Web.UI.Page
{
	/// <summary>
	/// Page load method works out whether its a flickr or static image
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		string sPhotoURL = Request.QueryString["image"];
		string sAction = Request.QueryString["action"];
		string sHeight = Request.QueryString["height"];
		string sWidth = Request.QueryString["width"];
		string sCache = ConfigurationManager.AppSettings["cacheLocation"];

		if (sPhotoURL != null && sCache != null)
		{
			LoadImage(sPhotoURL, sAction, sHeight, sWidth, sCache);
		}
		else
		{
			Image ourImage = LoadErrorImage(sAction, sHeight, sWidth);
			Response.Clear();
			Response.ContentType = "image/jpeg";
			ourImage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
		}

	}

	/// <summary>
	/// Tries to load an image resized image from cache, if not resizes image and caches it	
	/// </summary>
	/// <param name="sPhotoID">flickr image id</param>
	/// <param name="sCache">cache folder</param>
	private void LoadImage(string sPhotoURL, string sAction, string sHeight,  string sWidth, string sCache)
	{
		Image newImage;
		string sImageCacheKey = String.Empty;
		string sFileName = Path.GetFileName(sPhotoURL); //cache key only needs filename


		sImageCacheKey = sAction + "-" + sHeight + "-" + sWidth + "-" + sFileName;

		try
		{
			newImage = Image.FromFile(Server.MapPath(sCache + "/" + sImageCacheKey));
		}
		catch (Exception e)
		{
			Resizer resize = new Resizer();

			int iHeight;
			int iWidth;

			if (!int.TryParse(sWidth, out iWidth))
			{
				iWidth = 320;
			}

			if (!int.TryParse(sHeight, out iHeight))
			{
				iHeight = 240;
			}
			

			newImage = Image.FromFile(Server.MapPath(sPhotoURL));
			switch (sAction)
			{
				case "crop":
					newImage = resize.CropImage(newImage, iWidth , iHeight);
					break;
				case "resize":
					newImage = resize.ResizeImage(newImage, iWidth, iHeight);
					break;
				case "resizecrop":
					newImage = resize.ResizeCropImage(newImage, iWidth, iHeight);
					break;

			}

			newImage.Save(Server.MapPath(sCache) + sImageCacheKey);
		}
		Response.Clear();
		Response.ContentType = "image/jpeg";
	
		Response.BufferOutput = true;
		MemoryStream MemStream = new MemoryStream(); 
		newImage.Save(MemStream, System.Drawing.Imaging.ImageFormat.Jpeg);
		MemStream.WriteTo(Response.OutputStream);
		Response.Cache.SetExpires(DateTime.Now.AddMonths(1));
		Response.Cache.SetCacheability(HttpCacheability.Public);
		Response.Cache.SetNoServerCaching();
		newImage.Dispose();
		Response.Flush();
	}

	private Image LoadErrorImage(string sAction, string sHeight, string sWidth)
	{
		Image newImage;
		newImage = Image.FromFile(Server.MapPath("/images/noimage.jpg"));

		Resizer resize = new Resizer();

		int iHeight;
		int iWidth;

		if (!int.TryParse(sWidth, out iWidth))
		{
			iWidth = 320;
		}

		if (!int.TryParse(sHeight, out iHeight))
		{
			iHeight = 240;
		}

		switch (sAction)
		{
			case "crop":
				newImage = resize.CropImage(newImage, iWidth, iHeight);
				break;
			case "resize":
				newImage = resize.ResizeImage(newImage, iWidth, iHeight);
				break;
			case "resizecrop":
				newImage = resize.ResizeCropImage(newImage, iWidth, iHeight);
				break;

		}
		return newImage;
	}
}
