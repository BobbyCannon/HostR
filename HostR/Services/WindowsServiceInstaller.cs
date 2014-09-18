#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;

#endregion

namespace HostR.Services
{
	[ExcludeFromCodeCoverage]
	internal static class WindowsServiceInstaller
	{
		#region Static Methods

		/// <summary>
		/// Install an executable as a service.
		/// </summary>
		/// <param name="assemblyPath">The path to the executable.</param>
		/// <param name="serviceName">The name of the service.</param>
		/// <param name="displayName">THe display name of the service.</param>
		/// <param name="description">The description of the service.</param>
		/// <param name="startType">The startup type.</param>
		/// <param name="userName">The username to run as.</param>
		/// <param name="password">The password of the user.</param>
		/// <param name="dependancies"></param>
		public static void InstallService(string assemblyPath, string serviceName, string displayName, string description,
			ServiceStartMode startType, string userName = "", string password = "", IEnumerable<string> dependancies = null)
		{
			using (var procesServiceInstaller = new ServiceProcessInstaller())
			{
				if (string.IsNullOrEmpty(userName))
				{
					procesServiceInstaller.Account = ServiceAccount.LocalSystem;
				}
				else
				{
					procesServiceInstaller.Account = ServiceAccount.User;
					procesServiceInstaller.Username = userName;
					procesServiceInstaller.Password = password;
				}

				using (var installer = new ServiceInstaller())
				{
					var cmdline = new[] { string.Format("/assemblypath={0}", assemblyPath) };
					var context = new InstallContext(string.Empty, cmdline);

					installer.Context = context;
					installer.DisplayName = displayName;
					installer.Description = description;
					installer.ServiceName = serviceName;
					installer.StartType = startType;
					installer.Parent = procesServiceInstaller;

					if (dependancies != null)
					{
						installer.ServicesDependedOn = dependancies.ToArray();
					}

					IDictionary state = new Hashtable();

					try
					{
						installer.Install(state);
						installer.Commit(state);
					}
					catch (Exception ex)
					{
						installer.Rollback(state);
						throw new Exception("Failed to install the service.", ex);
					}
				}
			}
		}

		/// <summary>
		/// Sets the arguments for the service.
		/// </summary>
		/// <param name="serviceName">The name of the service.</param>
		/// <param name="arguments">The arguments for the service.</param>
		public static void SetServiceArguments(string serviceName, string arguments)
		{
			// We need to update the command line to indicate the service instance id.
			var serviceKeyName = string.Format("System\\CurrentControlSet\\Services\\{0}", serviceName);

			using (var serviceKey = Registry.LocalMachine.OpenSubKey(serviceKeyName, true))
			{
				// Ensure the service key exist.
				if (serviceKey == null)
				{
					return;
				}

				var imagePath = (string) serviceKey.GetValue("ImagePath");
				imagePath += " " + arguments.Trim();

				serviceKey.SetValue("ImagePath", imagePath);
				serviceKey.Close();
			}
		}

		/// <summary>
		/// Uninstall a service by name.
		/// </summary>
		/// <param name="serviceName">The name of the service.</param>
		public static void UninstallService(string serviceName)
		{
			using (var serviceInstallerObj = new ServiceInstaller())
			{
				var context = new InstallContext(null, null);

				serviceInstallerObj.Context = context;
				serviceInstallerObj.ServiceName = serviceName;

				try
				{
					serviceInstallerObj.Uninstall(null);
				}
				catch (Exception ex)
				{
					throw new Exception("Failed to uninstall the service.", ex);
				}
			}
		}

		#endregion
	}
}