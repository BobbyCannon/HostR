#region References

using System.Linq;

#endregion

namespace Hostr.Data
{
	public interface IRepository<T> : IQueryable<T> where T : Entity
	{
		#region Methods

		void Add(params T[] entity);
		void AddOrUpdate(params T[] entity);
		void Remove(int id);
		void Remove(params T[] entity);

		#endregion
	}
}