#region References

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Hostr.Processors
{
	public class ProcessorMonitor
	{
		#region Fields

		private readonly IProcessor _processor;
		private readonly Task _task;
		private readonly CancellationTokenSource _token;

		#endregion

		#region Constructors

		public ProcessorMonitor(IProcessor processor)
		{
			_processor = processor;
			_token = new CancellationTokenSource();
			_task = new Task(Monitor, _token.Token);
		}

		#endregion

		#region Properties

		public bool IsRunning
		{
			get { return _task.Status == TaskStatus.Running; }
		}

		#endregion

		#region Methods

		public void Cancel()
		{
			_token.Cancel();
		}

		public void CancelAfter(TimeSpan delay)
		{
			_token.CancelAfter(delay);
		}

		public void Start()
		{
			_task.Start();
		}

		private void Monitor()
		{
			// Monitor the triggers and allow them to read their resources.
			while (!_token.IsCancellationRequested)
			{
				_processor.ProcessTriggers();
			}
		}

		#endregion
	}
}