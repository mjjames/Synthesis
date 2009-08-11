using System;
using System.Collections.Generic;
using System.Linq;
using mjjames.AdminSystem.dataEntities;

namespace mjjames.AdminSystem.classes
{
	public class RecycleRepository
	{
		private readonly DataContexts.Archive.archiveDataContext _adc = new DataContexts.Archive.archiveDataContext();
		private readonly DataContexts.AdminDataContext _admin = new DataContexts.AdminDataContext();

		/// <summary>
		/// Returns all the RecycledItems for a given dbname
		/// </summary>
		public List<RecyledItem> GetRecycledItems()
		{
			List<RecyledItem> items = (from r in _adc.v_recyleditems
									   where r.DBName.Equals(_admin.Connection.Database)
									   select new RecyledItem
									   {
										   ItemKey = r.Key,
										   ItemTitle = r.Title,
										   ItemType = r.Type
									   }).ToList();
			return items;
		}

		internal void PermenantDelete(int id, string tableName)
		{
			string sqlQuery = String.Format("DELETE FROM [{0}] WHERE [{1}_archive_key] = {2}", tableName, tableName, id);
			_adc.ExecuteCommand(sqlQuery);
		}

		internal void RestoreItem(int id, string tableName)
		{
			string sqlFields = String.Empty;
			string sqlValues = String.Empty;
			string sqlTable = String.Empty;
			object[] values = new object[] { };
			switch (tableName.ToLower())
			{
				case "articles":
					DataEntities.Archive.article article = (from a in _adc.articles
															where a.articles_archive_key == id
															select a).SingleOrDefault();
					sqlFields = "[article_key], [active], [body], [end_date], [include_in_feed], [shortdescription], [showonhome], [sortorder], [start_date], [thumbnailimage], [title], [url], [virtualurl]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}";
					values = CleanValues(new object[]{
						article.article_key, article.active, article.body, article.end_date, article.include_in_feed, article.shortdescription, 
						article.showonhome, article.sortorder, article.start_date, article.thumbnailimage, article.title, article.url, article.virtualurl
					});
					sqlTable = "articles";
					_adc.articles.DeleteOnSubmit(article);
					break;
					
				case "banners":
					DataEntities.Archive.banner banner = (from b in _adc.banners
															where b.banners_archive_key == id
															select b).SingleOrDefault();
															
					sqlFields = "[bannerid], [alttext], [category], [image], [name], [randomness], [url]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6}";
					values = CleanValues(new object[]{
						banner.bannerdid, banner.alttext, banner.category, banner.image, banner.name, banner.randomness, banner.url
					});
					sqlTable = "banners";
					_adc.banners.DeleteOnSubmit(banner);
					break;
				
				case "media":
					DataEntities.Archive.media media = (from m in _adc.medias
															where m.media_archive_key == id
															select m).SingleOrDefault();
															
					sqlFields = "[media_key], [active], [description], [filename], [link], [mediatype_lookup], [title]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6}";
					values = CleanValues(new object[]{
						media.media_key, media.active, media.description, media.filename, media.link, media.mediatype_lookup, media.title
					});
					
					sqlTable = "media";
					
					_adc.medias.DeleteOnSubmit(media);
					
					break;
				
				case "newsletters":
					DataEntities.Archive.Newsletter newsletter = (from n in _adc.Newsletters
																		where n.newsletters_archive_key == id
																		select n).SingleOrDefault();
					sqlFields = "[newsletter_key], [body], [date_created], [date_sent], [subject]";
					sqlValues = "{0},{1},{2},{3},{4}";
					values = CleanValues(new object[]{
						newsletter.newsletter_key, newsletter.body, newsletter.date_created, newsletter.date_sent, newsletter.subject
					});
					sqlTable = "newsletters";
					_adc.Newsletters.DeleteOnSubmit(newsletter);
					break;
				
				case "offers":
					DataEntities.Archive.offer offer = (from o in _adc.offers
															where o.offers_archive_key == id
															select o).SingleOrDefault();
															
					sqlFields = "[offer_key], [active], [description], [offer_end], [offer_start], [shortdescription], [showinfeed], [showonhome], [thumbnailimage], [title], [url]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}";
					values = CleanValues(new object[]{
						offer.offer_key, offer.active, offer.description, offer.offer_end, offer.offer_start, offer.shortdescription,
						offer.showinfeed, offer.showonhome, offer.thumbnailimage, offer.title, offer.url
					});
					sqlTable = "offers";
					_adc.offers.DeleteOnSubmit(offer);
					break;
				

				case "pages":
					DataEntities.Archive.page page = (from p in _adc.pages
													  where p.pages_archive_key == id
													  select p).SingleOrDefault();

					sqlFields = "[page_key],[page_fkey],[accesskey],[active],[body],[linkurl],[metadescription],[metakeywords],[navtitle],[page_url],[pageid],[password],[passwordprotect],[showinfeaturednav],[showinfooter],[showinnav],[showonhome],[sortorder],[thumbnailimage],[title]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}";
					values = CleanValues(new object[]{
											page.page_key, page.page_fkey, page.accesskey, page.active, page.body, page.linkurl, page.metadescription,
											page.metakeywords, page.navtitle, page.page_url, page.pageid, page.password, page.passwordprotect,
											page.showinfeaturednav, page.showinfooter, page.showinnav, page.showonhome, page.sortorder,
											page.thumbnailimage, page.title
											});

					sqlTable = "pages";

					_adc.pages.DeleteOnSubmit(page);
						break;
					
				case "projects":
					DataEntities.Archive.project project = (from p in _adc.projects
															where p.projects_archive_key == id
															select p).SingleOrDefault();

					sqlFields = "[project_key], [active], [description], [end_date], [include_in_rss], [photogallery_id], [start_date], [title], [url], [video_id]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}";

					values = CleanValues(new object[]{
						project.project_key, project.active, project.description, project.end_date, project.include_in_rss, project.photogallery_id,
						project.start_date, project.title, project.url, project.video_id
					});
					
					sqlTable = "projects";
					
					_adc.projects.DeleteOnSubmit(project);
					break;
				
				case "testimonies":
					DataEntities.Archive.testimony testimony = (from t in _adc.testimonies
																	where t.testimonies_archive_key == id
																	select t).SingleOrDefault();
					
					sqlFields = "[testimony_key], [active], [description], [project_fkey], [title], [url], [video_id]";
					sqlValues = "{0},{1},{2},{3},{4},{5},{6}";
										
					values = CleanValues(new object[]{
						testimony.testimony_key, testimony.active, testimony.description, testimony.project_fkey, testimony.title, testimony.url, testimony.video_id
					});
					
					sqlTable = "testimonies";
					
					_adc.testimonies.DeleteOnSubmit(testimony);
				
					break;
			}

			string sql = String.Format(
				"set identity_insert [{0}] on; insert into {1} ({2}) values ({3}); set identity_insert [{4}] off;",
				sqlTable, sqlTable, sqlFields, sqlValues, sqlTable
			);

			if (!String.IsNullOrEmpty(sqlTable))
			{
				_admin.ExecuteCommand(String.Format(sql, values));
				_adc.SubmitChanges();
			}
		}

		private object[] CleanValues(object[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{

				//Convert Nulls
				if (values[i] == null) //if we have a null value resolve it
				{
					values[i] = "NULL";
					continue;
				}
				if (values[i].GetType() == typeof(bool))
				{
					if ((bool)values[i])
					{
						values[i] = 1;
					}
					else
					{
						values[i] = 0;
					}
					continue;
				}

				if (values[i].GetType() == typeof(DateTime))
				{
					DateTime dt = (DateTime) values[i];
					values[i] = "'" + dt.ToString("yyyyMMdd") + "'";
					continue;
				}


				//Ensure Strings are SQL Safe and has quotes
				if (values[i].GetType() == typeof(string))
				{
					values[i] = String.Format("'{0}'", SQLHelpers.SQLSafe((string)values[i]));
					continue;
				}

			}
			return values;
		}
	}
}
