namespace Hostr.Data
{
	public interface IDataService
	{
		#region Methods

		IRepository<T> GetRepository<T>() where T : Entity;
		int SaveChanges();

		#endregion
	}
}