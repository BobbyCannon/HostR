#region References

using System;

#endregion

namespace Hostr.Triggers
{
	public class ScheduleTrigger : Trigger
	{
		#region Fields

		private DateTime _lastTriggeredOn;

		#endregion

		#region Constructors

		public ScheduleTrigger(ScheduleTriggerSettings settings)
			: base(settings)
		{
			_lastTriggeredOn = DateTime.UtcNow;
		}

		#endregion

		#region Properties

		public new ScheduleTriggerSettings Settings
		{
			get { return (ScheduleTriggerSettings) base.Settings; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reads the resources the trigger is monitoring.
		/// </summary>
		public override bool Read()
		{
			switch (Settings.Type)
			{
				case ScheduleTriggerType.Interval:
					var currentTime = DateTime.UtcNow;
					var passedTime = currentTime.Subtract(_lastTriggeredOn);
					if (passedTime >= Settings.Interval)
					{
						OnWriteLine("Hey we triggered! Woot!");
						_lastTriggeredOn = currentTime;
						Set();
					}
					break;
			}

			// Return true if the trigger was set.
			return State == TriggerState.Set;
		}

		#endregion
	}
}