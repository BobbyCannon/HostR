namespace Hostr.Nodes
{
	public class NodeInfo
	{
		#region Properties

		public string Name { get; set; }
		public string UniqueId { get; set; }

		#endregion

		#region Static Methods

		public static NodeInfo Create(Data.Entities.NodeInfo arg)
		{
			return new NodeInfo
			{
				Name = arg.Name,
				UniqueId = arg.UniqueId
			};
		}

		#endregion
	}
}