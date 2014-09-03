#region References

using System;
using System.Collections.Generic;
using Hostr.Processors;

#endregion

namespace Hostr.Nodes
{
	public interface INode
	{
		#region Properties

		bool IsRunning { get; }
		IEnumerable<IProcessor> Processors { get; }
		NodeSettings Settings { get; }

		#endregion

		#region Methods

		void Start();
		void Stop();

		#endregion

		#region Events

		event EventHandler<string> WriteLine;

		#endregion
	}
}