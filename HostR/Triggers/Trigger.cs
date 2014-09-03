#region References

using System;

#endregion

namespace Hostr.Triggers
{
	public abstract class Trigger : ITrigger
	{
		#region Constructors

		protected Trigger(TriggerSettings settings)
		{
			Settings = settings;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the data that set off the trigger.
		/// </summary>
		public object Data { get; private set; }

		/// <summary>
		/// Gets the state of the trigger.
		/// </summary>
		public TriggerState State { get; private set; }

		/// <summary>
		/// Gets the settings.
		/// </summary>
		protected TriggerSettings Settings { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Clears the trigger of the current state. The data will be set to a default state.
		/// </summary>
		public void Clear()
		{
			Data = null;
			State = TriggerState.Cleared;
		}

		/// <summary>
		/// Reads the resources the trigger is monitoring.
		/// </summary>
		/// <returns>True if the read triggered during the read.</returns>
		public abstract bool Read();

		/// <summary>
		/// Sets the trigger to a triggered state using the provided data.
		/// </summary>
		/// <param name="data">The data to set the trigger with. Defaults to null.</param>
		public void Set(object data = null)
		{
			if (State == TriggerState.Set)
			{
				throw new Exception("The trigger has already been set.");
			}

			Data = data;
			State = TriggerState.Set;
		}

		protected virtual void OnWriteLine(string e)
		{
			var handler = WriteLine;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		#endregion

		#region Events

		public event EventHandler<string> WriteLine;

		#endregion
	}
}