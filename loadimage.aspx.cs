using System;
using System.Configuration;
using System.Web;
using mjjames.Imaging;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Drawing.Imaging;

public partial class loadimage : System.Web.UI.Page
{
    /// <summary>
    /// Page load method works out whether its a flickr or static image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected async void Page_Load(object sender, EventArgs e)
    {
        string photoURL = Server.UrlDecode(Request.QueryString["image"]);
        string action = RouteData.Values["action"].ToString();
        string height = RouteData.Values["height"].ToString();
        string width = RouteData.Values["width"].ToString();
        string cacheLocation = ConfigurationManager.AppSettings["cacheLocation"];

        int iHeight, iWidth;
        if (!int.TryParse(width, out iWidth))
        {
            iWidth = 320;
        }

        if (!int.TryParse(height, out iHeight))
        {
            iHeight = 240;
        }

        if (photoURL != null && cacheLocation != null)
        {
            try
            {
                await LoadImage(photoURL, action, iHeight, iWidth, cacheLocation);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                RenderErrorImage(action, iHeight, iWidth);
            }
        }
        else
        {
            RenderErrorImage(action, iHeight, iWidth);
        }

    }

    private void RenderErrorImage(string action, int height, int width)
    {

        using (Image ourImage = LoadErrorImage(action, height, width))
        {
            Response.Clear();
            Response.ContentType = GetContentType(ourImage.RawFormat);
            ourImage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }

    private string GetContentType(ImageFormat format)
    {
        if (format == ImageFormat.Jpeg)
        {
            return "image/jpeg";
        }
        if (format == ImageFormat.Png)
        {
            return "image/png";
        }
        return "";
    }

    /// <summary>
    /// Tries to load an image resized image from cache, if not resizes image and caches it	
    /// </summary>
    private async Task LoadImage(string photoUrl, string action, int height, int width, string cacheLocation)
    {
        Image newImage = null;
        string fileName = Path.GetFileName(photoUrl); //cache key only needs filename
        string imageCacheKey = action + "-" + height + "-" + width + "-" + fileName;

        try
        {
            newImage = GetImageFromCache(imageCacheKey, cacheLocation);
            if (newImage == null)
            {
                newImage = await GetOriginalImage(photoUrl);
                newImage = ResizeImage(newImage, imageCacheKey, cacheLocation, width, height, action);
            }

            OuputImageToResponse(newImage);
        }
        finally
        {
            if (newImage != null)
            {
                newImage.Dispose();
            }
        }
    }

    private void OuputImageToResponse(Image newImage)
    {
        //now we have an image use it responsibly
        using (newImage)
        {
            Response.Clear();
            Response.ContentType = "image/jpeg";

            Response.BufferOutput = true;
            using (MemoryStream memStream = new MemoryStream())
            {
                newImage.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                memStream.WriteTo(Response.OutputStream);
            }
            Response.Cache.SetExpires(DateTime.Now.AddMonths(1));
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetNoServerCaching();
        }
        Response.Flush();
    }

    private Image ResizeImage(Image newImage, string imageCacheKey, string cacheLocation, int width, int height, string action)
    {
        Resizer resize = new Resizer();

        switch (action)
        {
            case "crop":
                newImage = resize.CropImage(newImage, width, height);
                break;
            case "resize":
                newImage = resize.ResizeImage(newImage, width, height);
                break;
            case "resizecrop":
                newImage = resize.ResizeCropImage(newImage, width, height);
                break;

        }
        var path = Path.Combine(Server.MapPath(cacheLocation), imageCacheKey);
        newImage.Save(path);
        return newImage;
    }

    private Image GetImageFromCache(string imageCacheKey, string cacheLocation)
    {
        var path = Server.MapPath(Path.Combine(cacheLocation, imageCacheKey));
        if (File.Exists(path))
        {
            return Image.FromFile(path);
        }
        return null;
    }

    private async Task<Image> GetOriginalImage(string photoURL)
    {
        if (photoURL.StartsWith("http"))
        {
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(photoURL);
            return Image.FromStream(stream);
        }
        return Image.FromFile(Server.MapPath(photoURL));
    }

    private Image LoadErrorImage(string action, int height, int width)
    {
        var noImage = Server.MapPath("~/images/noimage.jpg");

        Image newImage = Image.FromFile(noImage);

        Resizer resize = new Resizer();

        switch (action)
        {
            case "crop":
                newImage = resize.CropImage(newImage, width, height);
                break;
            case "resize":
                newImage = resize.ResizeImage(newImage, width, height);
                break;
            case "resizecrop":
                newImage = resize.ResizeCropImage(newImage, width, height);
                break;

        }
        return newImage;
    }
}
