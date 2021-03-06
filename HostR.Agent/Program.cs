﻿#region References

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
			Service service;

			if (string.IsNullOrEmpty(arguments.ServiceWebApi))
			{
				service = new Service("HostR Agent", "Service agent for HostR.", arguments, null);
				service.Start();
			}
			else
			{
				var credentials = new LoginCredentials { UserName = arguments.ServiceWebApiUserName, Password = arguments.ServiceWebApiPassword };
				var client = string.IsNullOrWhiteSpace(credentials.UserName)
					? new WindowsServiceWebClient(arguments.ServiceWebApi)
					: new WindowsServiceWebClient(arguments.ServiceWebApi, credentials);

				service = new Service("HostR Agent", "Service agent for HostR.", arguments, client);
				service.Start();	
			}
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

				while (IsRunning)
				{
					CheckForUpdate();
					WriteLine("Count: " + count++);
					Sleep(5000);
				}
			}

			#endregion
		}

		#endregion
	}
}