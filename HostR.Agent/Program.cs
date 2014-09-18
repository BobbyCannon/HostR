﻿#region References

using System;
using System.Threading;
using HostR.Clients;
using HostR.Services;

#endregion

namespace HostR.Agent
{
	internal class Program
	{
		#region Static Methods

		private static void Main(string[] args)
		{
			var arguments = WindowsServiceArguments.Create(args);
			var credentials = new LoginCredentials { UserName = arguments.ServiceWebApiUserName, Password = arguments.ServiceWebApiPassword };
			var client = new WindowsServiceWebClient<LoginCredentials>(arguments.ServiceWebApi, "api/AgentService", credentials);
			var service = new Service("HostR Agent", "Service agent for HostR.", arguments, client);
			service.OnWriteLine += Console.WriteLine;
			service.Start();
		}

		#endregion

		#region Classes

		private class Service : WindowsService
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of the WindowsSerivce class.
			/// </summary>
			public Service(string displayName, string description, WindowsServiceArguments arguments, IWindowsServiceWebService client)
				: base(displayName, description, arguments, client)
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

				while (!CancellationPending)
				{
					Console.WriteLine("Count: " + count++);
					Thread.Sleep(100);
				}
			}

			#endregion
		}

		#endregion
	}
}