#region References

using System.Collections.Generic;
using Hostr.Nodes;

#endregion

namespace Hostr.Hubs
{
	public interface IHub
	{
		#region Methods

		IEnumerable<HubInfo> GetHubs();

		IEnumerable<NodeInfo> GetNodes();

		void RegisterHub(HubInfo info);

		void RegisterNode(NodeInfo info);

		void UnregisterHub(HubInfo info);

		void UnregisterNode(NodeInfo info);

		#endregion
	}
}