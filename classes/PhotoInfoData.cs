using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mjjames.ControlLibrary.AdminWebControls;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for PhotoInfoMock
/// </summary>
namespace mjjames.AdminSystem
{
	public class PhotoInfoData
	{

		private adminDataContext adminDC = new adminDataContext();
		
		/// <summary>
		/// Gets the key for the provided LookupID
		/// </summary>
		private int getLookupKey(string LookupID)
		{	
			int iLookupKey = (from l in adminDC.lookups
			 			  where l.lookup_id == LookupID
						  select l.lookup_key).SingleOrDefault();
			return iLookupKey;
		}

		/// <summary>
		/// Creates a PhotoInfoData with default lookupid
		/// </summary>
		public PhotoInfoData()
		{

		}
		
		/// <summary>
		/// Returns all the images attached to a link key
		/// </summary>
		/// <param name="linkkey">Key of Link Item</param>
		/// <returns>List of PhotoInfo</returns>
		public List<PhotoInfo> GetImages(int linkkey, string lookupid)
		{
			int iLookupKey = getLookupKey(lookupid);
			List<PhotoInfo> galleryimages = (from ml in adminDC.media_links
											 where ml.link_fkey == linkkey
												 && ml.linktype_lookup == iLookupKey
											 select new PhotoInfo
											 {
												 Key = ml.media_fkey,
												 Title = ml.media.title,
												 //AltTag = ml.media.title,
												 FileName = ml.media.filename,
												 Description = ml.media.description
											 }).ToList();
			return galleryimages;
		}


		/// <summary>
		/// Saves New Image to Database
		/// </summary>
		/// <param name="PhotoInfo">Photo Info to Insert</param>
		/// <param name="LinkKey">Link Key</param>
		/// <returns></returns>
		public int SaveImages(PhotoInfo PhotoInfo, int LinkKey, string lookupid)
		{
			int iLookupKey = getLookupKey(lookupid);
		
			media newimage = new media();
			newimage.active = true;
			newimage.description = PhotoInfo.Description;
			newimage.title = PhotoInfo.Title;
			newimage.filename = PhotoInfo.FileName;
			newimage.mediatype_lookup = iLookupKey;

			media_link newimagelink = new media_link();
			newimagelink.link_fkey = LinkKey;
			newimagelink.linktype_lookup = iLookupKey;
			newimagelink.media_fkey = newimage.media_key;

			newimage.media_links.Add(newimagelink);

			adminDC.medias.InsertOnSubmit(newimage);

			adminDC.SubmitChanges();

			return adminDC.GetChangeSet().Inserts.Count;

		}

		/// <summary>
		/// Updates an existing image with new photo details
		/// </summary>
		/// <param name="PhotoInfo">Photo Info to Update</param>
		/// <param name="key">key of item to update</param>
		public void UpdateImages(PhotoInfo PhotoInfo, int key)
		{
			media image = (from m in adminDC.medias
						   where m.media_key == key
						   select m).SingleOrDefault();

			if (!String.IsNullOrEmpty(PhotoInfo.FileName))
			{
				image.filename = PhotoInfo.FileName;
			}

			image.description = PhotoInfo.Description;
			image.title = PhotoInfo.Title;

			adminDC.SubmitChanges();
		}

		/// <summary>
		/// Removes an image
		/// </summary>
		/// <param name="PhotoInfo">PhotoInfo to remove</param>
		public void DeleteImage(int key, int linkkey, string lookupid)
		{
			int iLookupKey = getLookupKey(lookupid);
		
			media image = (from m in adminDC.medias
						   where m.media_key == key
						   select m).SingleOrDefault();

			adminDC.media_links.DeleteAllOnSubmit(image.media_links.Where(ml => ml.media_fkey == key && ml.link_fkey == linkkey && ml.linktype_lookup == iLookupKey));
			adminDC.SubmitChanges();
		}
	}
}