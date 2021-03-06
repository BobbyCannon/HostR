﻿#region References

using HostR.Services;

#endregion

namespace HostR.BasicService
{
	internal class Program
	{
		#region Static Methods

		private static void Main(string[] args)
		{
			var arguments = WindowsServiceArguments.Create(args);
			var service = new Service("HostR BasicService", "Really simple windows service.", arguments);
			service.Start();
		}

		#endregion

		#region Classes

		private class Service : WindowsService
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of the WindowsService class.
			/// </summary>
			public Service(string displayName, string description, WindowsServiceArguments arguments)
				: base(displayName, description, arguments)
			{
			}

			#endregion

			#region Methods

			/// <summary>
			/// The thread for the service.
			/// </summary>
			protected override void Process()
			{
				var count = 0;

				while (IsRunning)
				{
					WriteLine("Count: " + count++);
					Sleep(5000);
				}
			}

			#endregion
		}

		#endregion
	}
}