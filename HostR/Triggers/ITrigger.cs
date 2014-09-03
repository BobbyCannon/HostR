#region References

using System;

#endregion

namespace Hostr.Triggers
{
	/// <summary>
	/// A trigger will monitor some resource and allow triggering when the resource is in a specific state. 
	/// Triggers are very flexible. Some triggers may clear soon as they are read while other triggers may
	/// force you to clear them manually. Please see the documentation for your specific trigger to see how
	/// your trigger is expected to operate.
	/// </summary>
	public interface ITrigger
	{
		#region Properties

		/// <summary>
		/// Gets the data that set off the trigger.
		/// </summary>
		object Data { get; }

		/// <summary>
		/// Gets the state of the trigger.
		/// </summary>
		TriggerState State { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Clears the trigger of the current state. The data will be set to a default state.
		/// </summary>
		void Clear();

		/// <summary>
		/// Reads the resources the trigger is monitoring.
		/// </summary>
		/// <returns>True if the read triggered during the read.</returns>
		bool Read();

		/// <summary>
		/// Sets the trigger to a triggered state using the provided data.
		/// </summary>
		/// <param name="data">The data to set the trigger with.</param>
		void Set(object data);

		#endregion

		#region Events

		event EventHandler<string> WriteLine;

		#endregion
	}
}