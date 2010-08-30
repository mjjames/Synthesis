using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using mjjames.ControlLibrary.AdminWebControls;

/// <summary>
/// Summary description for PhotoInfoMock
/// </summary>
namespace mjjames.AdminSystem
{
	public class MediaInfoData
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
		/// Returns all the media attached to a link key
		/// </summary>
		/// <param name="linkkey">Key of Link Item</param>
		/// <param name="lookupid"></param>
		/// <returns>List of MediaInfo</returns>
		public List<MediaInfo> GetMedia(int linkkey, string lookupid)
		{
			int iLookupKey = GetLookupKey(lookupid);
			List<MediaInfo> mediaitems = (from ml in _adminDC.media_links
											 where ml.link_fkey == linkkey
												 && ml.linktype_lookup == iLookupKey
											 select new MediaInfo
											 {
												 Key = ml.media_fkey,
												 Title = ml.media.title,
												 //AltTag = ml.media.title,
												 FileName = ml.media.filename,
												 Description = ml.media.description
											 }).ToList();
			return mediaitems;
		}


		/// <summary>
		/// Saves New Item to Database
		/// </summary>
		/// <param name="mediaInfo">Media Info to Insert</param>
		/// <param name="linkKey">Link Key</param>
		/// <param name="lookupid">LookupID of link type</param>
		/// <param name="siteKey">Key of the site to add these media items too</param>
		/// <returns></returns>
		public int SaveMedia(MediaInfo mediaInfo, int linkKey, string lookupid, int siteKey)
		{
			int iLookupKey = GetLookupKey(lookupid);
		
			media newimage = new media
								{
									active = true,
									description = mediaInfo.Description,
									title = mediaInfo.Title,
									filename = mediaInfo.FileName,
									mediatype_lookup = iLookupKey,
									site_fkey = siteKey
								};

			media_link newimagelink = new media_link
										{
											link_fkey = linkKey,
											linktype_lookup = iLookupKey,
											media_fkey = newimage.media_key,
											site_fkey = siteKey
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
		/// Updates an existing media item with new details
		/// </summary>
		/// <param name="mediaInfo">Media Info to Update</param>
		/// <param name="key">key of item to update</param>
		public void UpdateMedia(MediaInfo mediaInfo, int key)
		{
			media image = (from m in _adminDC.medias
						   where m.media_key == key
						   select m).SingleOrDefault();

			if (!String.IsNullOrEmpty(mediaInfo.FileName))
			{
				image.filename = mediaInfo.FileName;
			}

			image.description = mediaInfo.Description;
			image.title = mediaInfo.Title;

			_adminDC.SubmitChanges();
		}

		/// <summary>
		/// Removes an image
		/// </summary>
		public void DeleteMedia(int key, int linkkey, string lookupid)
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