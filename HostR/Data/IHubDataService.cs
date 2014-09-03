#region References

using Hostr.Data.Entities;

#endregion

namespace Hostr.Data
{
	public interface IHubDataService : IDataService
	{
		#region Properties

		IRepository<HubInfo> ServiceHubs { get; }
		IRepository<NodeInfo> ServiceNodes { get; }

		#endregion
	}
}