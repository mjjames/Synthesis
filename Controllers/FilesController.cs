using MKS.GarageManagement.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mjjames.AdminSystem.Models;
using System.IO;

namespace mjjames.AdminSystem.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly IFileStorage _fileStorage;

        public FilesController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        //
        // POST: /Files/Image
        [HttpPost]
        [ActionName("Image")]
        public JsonResult AddImage(HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("no file provided", "file");
            }
            return AddImages(new[] { file });
        }

        //
        // POST: /Files/Document
        [HttpPost]
        [ActionName("Document")]
        public JsonResult AddDocument(HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("no file provided", "file");
            }
            return AddDocuments(new[] { file });
        }

        //
        // POST: /Files/Media
        [HttpPost]
        [ActionName("Media")]
        public JsonResult AddMedia(HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("no file provided", "file");
            }
            return AddMedias(new[] { file });
        }

        //
        // POST: /Files/File
        [HttpPost]
        [ActionName("File")]
        public JsonResult AddFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("no file provided", "file");
            }

            var validContentTypes = new List<string>
                {
                    ".doc",
                    ".docx",
                    ".pdf",
                    ".gif",
                    ".jpeg",
                    ".jpg",
                    ".png", 
                    ".mp3",
                    ".wma",
                    ".m4a"
                };
            return UploadFiles(new []{ file }, validContentTypes);
        }


        //
        // POST: /Files/Documents
        [HttpPost]
        [ActionName("Documents")]
        public JsonResult AddDocuments(IEnumerable<HttpPostedFileBase> files)
        {
            var validContentTypes = new List<string>
                {
                    ".doc",
                    ".docx",
                    ".pdf"
                };
            return UploadFiles(files, validContentTypes);
        }

        //
        // POST: /Files/Medias
        [HttpPost]
        [ActionName("Medias")]
        public JsonResult AddMedias(IEnumerable<HttpPostedFileBase> files)
        {
            var validContentTypes = new List<string>
                {
                    ".mp3",
                    ".wma",
                    ".m4a"
                };
            return UploadFiles(files, validContentTypes);
        }

        private JsonResult UploadFiles(IEnumerable<HttpPostedFileBase> files, IList<string> validContentTypes)
        {
            //if no files are passed the form variable is probably named wrong
            //we just return an empty object and stop processing
            if (files == null)
            {
                return Json(new object());
            }

            var postedFileBases = files as HttpPostedFileBase[] ?? files.ToArray();
            var filePaths = new List<Uri>(postedFileBases.Count());

            filePaths.AddRange(from fileBase in postedFileBases
                               let extension = Path.GetExtension(fileBase.FileName)
                               where extension != null && (validContentTypes.Contains(extension.ToLower())
                                                           && fileBase.ContentLength != 0)
                               select _fileStorage.PersistFile(fileBase.FileName, fileBase.InputStream));
            return Json(filePaths);
        }

        [HttpGet]
        public HttpStatusCodeResult RotateImage(string fileName, Rotation rotation)
        {
            var fileStream = _fileStorage.LoadFile(fileName, true);
            if (fileStream == null)
            {
                return new HttpNotFoundResult();
            }
            var resizeSettings = new ImageResizer.ResizeSettings();
            switch (rotation)
            {
                case Rotation.Left:
                    resizeSettings.Rotate = -90;
                    break;
                case Rotation.Right:
                    resizeSettings.Rotate = 90;
                    break;
                case Rotation.OneEighty:
                    resizeSettings.Rotate = 180;
                    break;

            }
            var resizedStream = new MemoryStream();
            ImageResizer.ImageBuilder.Current.Build(fileStream, resizedStream, resizeSettings);
            _fileStorage.PersistFile(Path.GetFileName(fileName), resizedStream, true);

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        //
        // POST: /Files/Images
        [HttpPost]
        [ActionName("Images")]
        public JsonResult AddImages(IEnumerable<HttpPostedFileBase> files)
        {
            var validContentTypes = new List<string>
                {
                    ".gif",
                    ".jpeg",
                    ".jpg",
                    ".png"
                };

            return UploadFiles(files, validContentTypes);
        }

    }
}