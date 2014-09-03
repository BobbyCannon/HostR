#region References

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hostr.Extensions;
using Hostr.Processors;

#endregion

namespace Hostr.Nodes
{
	public class NodeSettings : Settings
	{
		#region Constructors

		public NodeSettings()
		{
			ProcessorSettings = new Collection<ProcessorSettings>();
		}

		#endregion

		#region Properties

		public IEnumerable<ProcessorSettings> ProcessorSettings { get; set; }

		#endregion

		#region Methods

		public INode CreateInstance()
		{
			return TargetFullName.CreateInstance(new object[] { this }) as INode;
		}

		#endregion
	}
}