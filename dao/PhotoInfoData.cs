using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using mjjames.ControlLibrary.AdminWebControls;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;

/// <summary>
/// Summary description for PhotoInfoMock
/// </summary>
namespace mjjames.AdminSystem
{
	public class PhotoInfoData
	{

		private readonly AdminDataContext _adminDC =new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		
		/// <summary>
		/// Gets the key for the provided lookupID
		/// </summary>
		private int GetLookupKey(string lookupID)
		{	
			int iLookupKey = (from l in _adminDC.lookups
			 			  where l.lookup_id == lookupID
						  select l.lookup_key).SingleOrDefault();
			return iLookupKey;
		}

		/// <summary>
		/// Returns all the images attached to a link key
		/// </summary>
		/// <param name="linkkey">Key of Link Item</param>
		/// <param name="lookupid"></param>
		/// <returns>List of PhotoInfo</returns>
		public List<PhotoInfo> GetImages(int linkkey, string lookupid)
		{
			int iLookupKey = GetLookupKey(lookupid);
			List<PhotoInfo> galleryimages = (from ml in _adminDC.media_links
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
		/// <param name="photoInfo">Photo Info to Insert</param>
		/// <param name="linkKey">Link Key</param>
		/// <param name="lookupid"></param>
		/// <returns></returns>
		public int SaveImages(PhotoInfo photoInfo, int linkKey, string lookupid)
		{
			int iLookupKey = GetLookupKey(lookupid);
		
			media newimage = new media
			                 	{
			                 		active = true,
			                 		description = photoInfo.Description,
			                 		title = photoInfo.Title,
			                 		filename = photoInfo.FileName,
			                 		mediatype_lookup = iLookupKey
			                 	};

			media_link newimagelink = new media_link
			                          	{
			                          		link_fkey = linkKey,
			                          		linktype_lookup = iLookupKey,
			                          		media_fkey = newimage.media_key
			                          	};

			if(linkKey > 0)
			{
				newimage.media_links.Add(newimagelink);

				_adminDC.medias.InsertOnSubmit(newimage);

				_adminDC.SubmitChanges();
			}
			return _adminDC.GetChangeSet().Inserts.Count;

		}

		/// <summary>
		/// Updates an existing image with new photo details
		/// </summary>
		/// <param name="photoInfo">Photo Info to Update</param>
		/// <param name="key">key of item to update</param>
		public void UpdateImages(PhotoInfo photoInfo, int key)
		{
			media image = (from m in _adminDC.medias
						   where m.media_key == key
						   select m).SingleOrDefault();

			if (!String.IsNullOrEmpty(photoInfo.FileName))
			{
				image.filename = photoInfo.FileName;
			}

			image.description = photoInfo.Description;
			image.title = photoInfo.Title;

			_adminDC.SubmitChanges();
		}

		/// <summary>
		/// Removes an image
		/// </summary>
		public void DeleteImage(int key, int linkkey, string lookupid)
		{
			int iLookupKey = GetLookupKey(lookupid);
		
			media image = (from m in _adminDC.medias
						   where m.media_key == key
						   select m).SingleOrDefault();

			_adminDC.media_links.DeleteAllOnSubmit(image.media_links.Where(ml => ml.media_fkey == key && ml.link_fkey == linkkey && ml.linktype_lookup == iLookupKey));
			_adminDC.SubmitChanges();
		}
	}
}