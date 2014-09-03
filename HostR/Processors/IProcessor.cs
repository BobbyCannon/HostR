#region References

using System;
using System.Collections.Generic;
using Hostr.Triggers;

#endregion

namespace Hostr.Processors
{
	public interface IProcessor
	{
		#region Properties

		/// <summary>
		/// The triggers for the processor.
		/// </summary>
		IEnumerable<ITrigger> Triggers { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Processes a trigger that has been triggered. This method should be called during ProcessTriggers.
		/// </summary>
		/// <param name="trigger">The trigger that was triggered.</param>
		void Process(ITrigger trigger);

		/// <summary>
		/// Reads the triggers and allows a processor to process based on triggered triggers.
		/// </summary>
		void ProcessTriggers();

		#endregion

		#region Events

		event EventHandler<string> WriteLine;

		#endregion
	}
}