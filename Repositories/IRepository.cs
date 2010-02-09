using System.Collections.Generic;
using mjjames.AdminSystem.DataEntities;

namespace mjjames.AdminSystem.Repositories
{
	public interface IRepository<T>
	{

		T GetByKey(int id);
		void Add(T entity);
		void Remove(T entity);
		IEnumerable<T> GetAll();
		void Save();

	}
}
