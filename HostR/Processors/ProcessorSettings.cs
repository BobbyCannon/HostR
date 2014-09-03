#region References

using System.Collections.Generic;
using Hostr.Extensions;
using Hostr.Triggers;

#endregion

namespace Hostr.Processors
{
	public class ProcessorSettings : Settings
	{
		#region Properties

		public IEnumerable<TriggerSettings> TriggerSettings { get; set; }

		#endregion

		#region Methods

		public IProcessor CreateInstance()
		{
			return TargetFullName.CreateInstance(new object[] { this }) as IProcessor;
		}

		#endregion
	}
}