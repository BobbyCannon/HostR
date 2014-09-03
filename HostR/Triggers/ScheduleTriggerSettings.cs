#region References

using System;

#endregion

namespace Hostr.Triggers
{
	public class ScheduleTriggerSettings : TriggerSettings
	{
		#region Properties

		public DateTime Date { get; set; }
		public TimeSpan Interval { get; set; }
		public ScheduleTriggerType Type { get; set; }

		#endregion
	}
}