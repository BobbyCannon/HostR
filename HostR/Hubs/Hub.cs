#region References

using System.Collections.Generic;
using System.Linq;
using Hostr.Data;
using Hostr.Nodes;

#endregion

namespace Hostr.Hubs
{
	public class Hub : IHub
	{
		#region Fields

		private readonly IHubDataService _dataService;

		#endregion

		#region Constructors

		public Hub(IHubDataService dataService)
		{
			_dataService = dataService;
		}

		#endregion

		#region Methods

		public IEnumerable<HubInfo> GetHubs()
		{
			return _dataService
				.ServiceHubs
				.OrderBy(x => x.CreatedOn)
				.ToList()
				.Select(HubInfo.Create);
		}

		public IEnumerable<NodeInfo> GetNodes()
		{
			return _dataService
				.ServiceNodes
				.OrderBy(x => x.CreatedOn)
				.ToList()
				.Select(NodeInfo.Create);
		}

		public void RegisterHub(HubInfo info)
		{
			var foundHub = _dataService.ServiceHubs.FirstOrDefault(x => x.UniqueId == info.UniqueId);
			if (foundHub == null)
			{
				foundHub = new Data.Entities.HubInfo();
				foundHub.UniqueId = info.UniqueId;
			}

			foundHub.Name = info.Name;

			_dataService.ServiceHubs.AddOrUpdate(foundHub);
			_dataService.SaveChanges();
		}

		public void RegisterNode(NodeInfo info)
		{
			var foundNode = _dataService.ServiceNodes.FirstOrDefault(x => x.UniqueId == info.UniqueId);
			if (foundNode == null)
			{
				foundNode = new Data.Entities.NodeInfo();
				foundNode.UniqueId = info.UniqueId;
			}

			foundNode.Name = info.Name;

			_dataService.ServiceNodes.AddOrUpdate(foundNode);
			_dataService.SaveChanges();
		}

		public void UnregisterHub(HubInfo info)
		{
			var foundHub = _dataService.ServiceHubs.FirstOrDefault(x => x.UniqueId == info.UniqueId);
			if (foundHub != null)
			{
				_dataService.ServiceHubs.Remove(foundHub);
				_dataService.SaveChanges();
			}
		}

		public void UnregisterNode(NodeInfo info)
		{
			var foundNode = _dataService.ServiceNodes.FirstOrDefault(x => x.UniqueId == info.UniqueId);
			if (foundNode != null)
			{
				_dataService.ServiceNodes.Remove(foundNode);
				_dataService.SaveChanges();
			}
		}

		#endregion
	}
}