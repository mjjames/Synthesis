using System.Collections.Generic;
using mjjames.AdminSystem.DataEntities;

namespace mjjames.AdminSystem.Repositories
{
	interface IComments : IRepository<Comment>
	{
		IEnumerable<Comment> GetCommentsByArticle(int articleKey);
		IEnumerable<Comment> GetCommentsByUserName(string userName);
		IEnumerable<Comment> GetCommentsByEmail(string email);
		IEnumerable<Comment> GetCommentsByWebsite(string url);
	}
}
