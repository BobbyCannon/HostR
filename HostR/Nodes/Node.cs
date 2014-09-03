#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Hostr.Extensions;
using Hostr.Processors;

#endregion

namespace Hostr.Nodes
{
	public class Node : INode
	{
		#region Fields

		private readonly List<ProcessorMonitor> _monitors;

		#endregion

		#region Constructors

		public Node(NodeSettings settings)
		{
			_monitors = new List<ProcessorMonitor>();

			Settings = settings;
			Processors = settings.ProcessorSettings.Select(x => x.CreateInstance()).ToList();
			Processors.ForEach(x => x.WriteLine += (sender, message) => OnWriteLine(message));
		}

		#endregion

		#region Properties

		public bool IsRunning
		{
			get { return _monitors.Any(x => x.IsRunning); }
		}

		public IEnumerable<IProcessor> Processors { get; private set; }
		public NodeSettings Settings { get; private set; }

		#endregion

		#region Methods

		public void Start()
		{
			OnWriteLine("We are starting the node...");
			foreach (var processor in Processors)
			{
				var monitor = new ProcessorMonitor(processor);
				_monitors.Add(monitor);
				monitor.Start();
			}
		}

		public void Stop()
		{
			OnWriteLine("We are stopping the node...");
			foreach (var monitor in _monitors)
			{
				monitor.Cancel();
			}

			_monitors.Clear();
		}

		protected virtual void OnWriteLine(string message)
		{
			var handler = WriteLine;
			if (handler != null)
			{
				handler(this, message);
			}
		}

		#endregion

		#region Events

		public event EventHandler<string> WriteLine;

		#endregion
	}
}