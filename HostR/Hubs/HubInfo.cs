namespace Hostr.Hubs
{
	public class HubInfo
	{
		#region Properties

		public string Name { get; set; }
		public string UniqueId { get; set; }

		#endregion

		#region Static Methods

		public static HubInfo Create(Data.Entities.HubInfo arg)
		{
			return new HubInfo
			{
				Name = arg.Name,
				UniqueId = arg.UniqueId
			};
		}

		#endregion
	}
}