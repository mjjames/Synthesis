using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.DataEntities;
using log4net;

namespace mjjames.AdminSystem.Repositories
{
	public class CommentsRepository :IComments
	{
		private readonly AdminDataContext _adc = new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		private readonly ILog _logger = LogManager.GetLogger(typeof (CommentsRepository));
		
		public Comment GetByKey(int id)
		{
			return (from c in _adc.Comments
					where c.CommentKey == id
					select c).SingleOrDefault();
		}

		public void Add(Comment entity)
		{
			_adc.Comments.InsertOnSubmit(entity);
		}

		public void Remove(Comment entity)
		{
			_adc.Comments.DeleteOnSubmit(entity);
		}

		public IEnumerable<Comment> GetAll() ///Is this really safe?
		{
			return from c in _adc.Comments
			       select c;
		}

		public void Save()
		{
			try
			{
				_adc.SubmitChanges();
			}
			catch(Exception e)
			{
				_logger.Error("Save Comments Failed  " + e.Message, e);
				throw;
			}
		}

		public IEnumerable<Comment> GetCommentsByArticle(int articleKey)
		{
			return from c in _adc.Comments
			       where c.ArticleKey == articleKey
			       select c;
		}

		public IEnumerable<Comment> GetCommentsByUserName(string userName)
		{
			return from c in _adc.Comments
			       where c.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
			       select c;
		}

		public IEnumerable<Comment> GetCommentsByEmail(string email)
		{
			return from c in _adc.Comments
			       where c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
			       select c;
		}

		public IEnumerable<Comment> GetCommentsByWebsite(string url)
		{
			return from c in _adc.Comments
			       where c.Website.Equals(url, StringComparison.OrdinalIgnoreCase)
			       select c;
		}
	}
}
