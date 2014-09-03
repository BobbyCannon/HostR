#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Hostr.Extensions;
using Hostr.Triggers;

#endregion

namespace Hostr.Processors
{
	public abstract class Processor : IProcessor
	{
		#region Constructors

		protected Processor(ProcessorSettings settings)
		{
			Settings = settings;
			Triggers = Settings.TriggerSettings.Select(x => x.CreateInstance()).ToList();
			Triggers.ForEach(x => x.WriteLine += (sender, message) => OnWriteLine(message));
		}

		#endregion

		#region Properties

		public ProcessorSettings Settings { get; set; }

		/// <summary>
		/// The triggers for the processor.
		/// </summary>
		public IEnumerable<ITrigger> Triggers { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Processes a trigger that has been triggered. This method should be called during ProcessTriggers.
		/// </summary>
		/// <param name="trigger">The trigger that was triggered.</param>
		public abstract void Process(ITrigger trigger);

		/// <summary>
		/// Reads the triggers and allows a processor to process based on triggered triggers.
		/// </summary>
		public abstract void ProcessTriggers();

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