#region References

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hostr.Extensions;
using HostR.Extensions;
using HostR.Interfaces;
using NLog;
using NLog.Config;
using NLog.Targets;

#endregion

namespace HostR.Services
{
	/// <summary>
	/// Represents a windows service.
	/// </summary>
	public abstract class WindowsService : ServiceBase
	{
		#region Fields

		private readonly IWindowsServiceWebService _client;
		private readonly string _description;
		private readonly string _displayName;
		private readonly Thread _serviceThread;
		private string _applicationDirectory;
		private string _applicationFilePath;
		private string _applicationName;
		private Logger _logger;
		private string _version;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the WindowsSerivce class.
		/// </summary>
		protected WindowsService(string displayName, string description, WindowsServiceArguments arguments, IWindowsServiceWebService client = null)
		{
			Arguments = arguments;
			_displayName = displayName;
			_description = description;
			_client = client;
			_serviceThread = new Thread(ServiceThread);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the service is running.
		/// </summary>
		public bool IsRunning { get; set; }

		/// <summary>
		/// Gets a value indicating if the service is being trigger.
		/// </summary>
		public bool TriggerPending { get; set; }

		/// <summary>
		/// The arguments the service was started with
		/// </summary>
		protected WindowsServiceArguments Arguments { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Allows public access to the OnStart method.
		/// </summary>
		public void Start()
		{
			var assembly = Assembly.GetCallingAssembly();
			_applicationFilePath = assembly.Location;
			_applicationDirectory = Path.GetDirectoryName(_applicationFilePath);
			_applicationName = Path.GetFileNameWithoutExtension(_applicationFilePath);
			_version = assembly.GetName().Version.ToString();

			if (string.IsNullOrEmpty(Arguments.ServiceName))
			{
				Arguments.ServiceName = _applicationName;
			}

			if (LogManager.Configuration == null || LogManager.Configuration.AllTargets.Count <= 0)
			{
				EnableDefaultLogTargets();
			}

			_logger = LogManager.GetLogger(Arguments.ServiceName);
			WriteLine("{0} v{1}", _displayName, _version);

			if (Arguments.ShowHelp)
			{
				WriteLine(BuildHelpInformation());
				return;
			}

			if (Arguments.UpdateService)
			{
				UpdateService();
				return;
			}

			if (Arguments.InstallService)
			{
				InstallService();
				return;
			}

			if (Arguments.UninistallService)
			{
				UninstallService();
				return;
			}

			// Check to see if we need to run in service mode.
			if (!Environment.UserInteractive)
			{
				// Run the service in service mode.
				Run(this);
				return;
			}

			// Start the process in debug mode.
			OnStart(null);
			HandleConsole();
		}

		/// <summary>
		/// Builds the help information that will be displayed with the -h command option is requested.
		/// </summary>
		/// <returns>The string to be displayed.</returns>
		protected virtual string BuildHelpInformation()
		{
			var builder = new StringBuilder();

			builder.AppendFormat("{0} [-i] [-u] [-n] [ServiceName] [-w] [WebServiceUri] [-l] [UserName] [-p] [Password] [-h] [-v]\r\n", _applicationName);
			builder.AppendLine("[-i] Install the service.");
			builder.AppendLine("[-u] Uninstall the service.");
			builder.AppendLine("[-n] The name for the service. Defaults to FileName.");
			builder.AppendLine("[-w] The URI of the service API.");
			builder.AppendLine("[-l] The username for the service API.");
			builder.AppendLine("[-p] The password for the service API.");
			builder.AppendLine("[-d] Developer option to wait for debugger.");
			builder.AppendLine("[-r] The path of the installation to upgrade.");
			builder.AppendLine("[-v] Enables verbose logging.");
			builder.Append("[-h] Prints the help menu.");

			return builder.ToString();
		}

		/// <summary>
		/// Checks the client to see if there is an update available. If so it starts the update process
		/// and returns true. If no update is found then return false.
		/// </summary>
		/// <returns>True if an update has started or false otherwise.</returns>
		protected void CheckForUpdate()
		{
			try
			{
				WriteLine("Check for a service update.", LogLevel.Trace);
				var serviceDetails = new WindowsServiceDetails { Name = _applicationName, Version = _version };
				var update = _client.CheckForUpdate(serviceDetails);

				if (update.Size > 0)
				{
					WriteLine("Starting to update the service.");
					StartServiceUpdate(update);
				}
			}
			catch (Exception ex)
			{
				WriteLine(ex.ToDetailedString(), LogLevel.Fatal);
			}
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system 
		/// starts (for a service that starts automatically). Specifies actions to take when the service starts.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		protected override void OnStart(string[] args)
		{
			WriteLine("Starting the service...");
			IsRunning = true;
			_serviceThread.Start();
			WriteLine("The service has started.");
			base.OnStart(args);
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when 
		/// a service stops running.
		/// </summary>
		protected override void OnStop()
		{
			if (!IsRunning)
			{
				// Return because we are not running.
				return;
			}

			WriteLine("Stopping the service...");
			IsRunning = false;
			_serviceThread.Join(new TimeSpan(0, 1, 0));
			WriteLine("The service has stopped.");
			base.OnStop();
		}

		/// <summary>
		/// The thread for the service.
		/// </summary>
		protected abstract void Process();

		/// <summary>
		/// Puts the service to sleep for provided delay (in milliseconds). The service will be woke up if the service gets a request to close or to trigger the service.
		/// </summary>
		protected void Sleep(int delay)
		{
			Sleep(TimeSpan.FromMilliseconds(delay));
		}

		/// <summary>
		/// Puts the service to sleep for provided delay. The service will be woke up if the service gets a request to close or to trigger the service.
		/// </summary>
		protected void Sleep(TimeSpan delay)
		{
			var watch = Stopwatch.StartNew();
			while (watch.Elapsed < delay && IsRunning && !TriggerPending)
			{
				Thread.Sleep(50);
			}

			// Clear the pending trigger.
			TriggerPending = false;
		}

		/// <summary>
		/// Writes an "info" message to the logger.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="parameters">The optional parameters for the message.</param>
		protected void WriteLine(string message, params object[] parameters)
		{
			WriteLine(message, LogLevel.Info, parameters);
		}

		/// <summary>
		/// Writes an message to the logger at a provided level.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="level">The level at which to write the message.</param>
		/// <param name="parameters">The optional parameters for the message.</param>
		protected void WriteLine(string message, LogLevel level, params object[] parameters)
		{
			if (_logger == null)
			{
				return;
			}

			if (level == LogLevel.Debug)
			{
				_logger.Debug(message, parameters);
			}
			else if (level == LogLevel.Fatal)
			{
				_logger.Fatal(message, parameters);
			}
			else if (level == LogLevel.Trace)
			{
				_logger.Trace(message, parameters);
			}
			else if (level == LogLevel.Warn)
			{
				_logger.Warn(message, parameters);
			}
			else
			{
				_logger.Info(message, parameters);
			}
		}

		/// <summary>
		/// Copy the source directory into the destination directory.
		/// </summary>
		/// <param name="source">The directory containing the source files and folders.</param>
		/// <param name="destination">The directory to copy the source into.</param>
		private void CopyDirectory(string source, string destination)
		{
			Console.WriteLine("Emptying the directory [" + destination + "]");
			var destinationInfo = new DirectoryInfo(destination);
			destinationInfo.Empty();

			var sourceInfo = new DirectoryInfo(source);
			foreach (var fileInfo in sourceInfo.GetFiles())
			{
				fileInfo.CopyTo(Path.Combine(destination, fileInfo.Name));
			}

			foreach (var directoryInfo in sourceInfo.GetDirectories())
			{
				CopyDirectory(Path.Combine(source, directoryInfo.Name), Path.Combine(destination, directoryInfo.Name));
			}
		}

		private byte[] DownloadUpdate(WindowsServiceUpdate update)
		{
			// Get the file from the deployment service.
			var data = new byte[update.Size];
			var request = new WindowsServiceUpdateRequest { Name = update.Name, Offset = 0 };

			// Read the whole file.
			while (request.Offset < data.Length)
			{
				var chunk = _client.DownloadUpdateChunk(request);
				Array.Copy(chunk, 0, data, request.Offset, chunk.Length);
				request.Offset += chunk.Length;
			}

			// Return the data read.
			return data;
		}

		/// <summary>
		/// Configures the log targets that this service will use. The default log targets is a colored console target that logs verbose and a event log 
		/// target that logs "info" or higher.
		/// </summary>
		private void EnableDefaultLogTargets()
		{
			// Start configuring the logger.
			var loggingConfiguration = new LoggingConfiguration();

			// Configure the console logger.
			var consoleTarget = new ColoredConsoleTarget();
			consoleTarget.Layout = "${message}";
			loggingConfiguration.AddTarget("Console", consoleTarget);
			loggingConfiguration.LoggingRules.Add(new LoggingRule("*", Arguments.VerboseLogging ? LogLevel.Trace : LogLevel.Info, consoleTarget));

			// Configure the event logger for exceptions.
			var eventLogTarget = new EventLogTarget();
			eventLogTarget.Source = Arguments.ServiceName;
			eventLogTarget.Layout = "${message}";
			loggingConfiguration.AddTarget("EventLog", eventLogTarget);
			loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, eventLogTarget));

			// Enable the logger and print the current version.
			LogManager.Configuration = loggingConfiguration;
		}

		/// <summary>
		/// Grab the console and wait for it to close.
		/// </summary>
		private void HandleConsole()
		{
			// Redirects the 'X' for the console window so we can close the service cleanly.
			var stopDebugHandler = new HandlerRoutine(OnStop);
			SetConsoleCtrlHandler(stopDebugHandler, true);

			// Loop here while the service is running.
			while (IsRunning)
			{
				// Minor delay for process management.
				Thread.Sleep(50);

				// Check to see if someone pressed a key.
				if (Console.KeyAvailable)
				{
					// Check to see if the key was a space.
					if (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
					{
						// It was not a space so break the running loop and close the service.
						break;
					}

					// Set the pending trigger flag. This allows the service to break out of a delay.
					TriggerPending = true;
				}
			}

			// It was not a space so break the running loop and close the service.
			OnStop();

			// If we don't have this the handler will get garbage collected and will result in a
			// null reference exception when the console windows is closed with the 'X'.
			GC.KeepAlive(stopDebugHandler);
		}

		/// <summary>
		/// Install the service as a windows service.
		/// </summary>
		private void InstallService()
		{
			try
			{
				var displayName = Arguments.ServiceName != _applicationName ? _displayName + " (" + Arguments.ServiceName + ")" : _displayName;
				WindowsServiceInstaller.InstallService(_applicationFilePath, Arguments.ServiceName, displayName, _description, ServiceStartMode.Automatic);
				WindowsServiceInstaller.SetServiceArguments(Arguments.ServiceName, Arguments.ServiceArguments);
			}
			catch (Exception ex)
			{
				WriteLine(ex.ToDetailedString(), LogLevel.Fatal);
			}
		}

		private string SaveUpdate(byte[] agentBits)
		{
			// Download the latest updated bits.
			var agentUpdateDirectory = Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine) + "\\" + Arguments.ServiceName;
			var agentUpdateFilePath = agentUpdateDirectory + "\\Update.zip";

			WriteLine("Cleaning up the update folder.");
			CreateOrCleanDirectory(agentUpdateDirectory + "\\Update");

			WriteLine("Save the new service to the update folder.");
			File.WriteAllBytes(agentUpdateFilePath, agentBits);

			// Extract the bits to a temp folder.
			WriteLine("Extract the new service to the update folder.");
			ExtractZipfile(agentUpdateFilePath);

			return agentUpdateDirectory + "\\Update";
		}

		/// <summary>
		/// Internal management of service thread.
		/// </summary>
		private void ServiceThread()
		{
			try
			{
				// Run the windows service process.
				Process();
			}
			catch (Exception ex)
			{
				WriteLine(ex.ToDetailedString(), LogLevel.Fatal);
			}
		}

		/// <summary>
		/// Shutdown all the other services.
		/// </summary>
		private void ShutdownServices()
		{
			WriteLine("Shutting down all the running services.");

			if (ServiceController.GetServices().Any(x => x.ServiceName == Arguments.ServiceName))
			{
				var service = new ServiceController(Arguments.ServiceName);
				if (service.Status == ServiceControllerStatus.Running)
				{
					service.Stop();
					service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
				}
			}

			// Get a list of all other agents except this one.
			var myProcess = System.Diagnostics.Process.GetCurrentProcess();
			var otherProcesses = System.Diagnostics.Process.GetProcessesByName(Arguments.ServiceName)
				.Where(p => p.Id != myProcess.Id)
				.ToList();

			// Cycle through all the other processes and close them gracefully.
			foreach (var process in otherProcesses)
			{
				// Ask the process to close gracefully and give it 10 seconds to do so.
				process.CloseMainWindow();
				process.WaitForExit(10000);

				// See if the process has gracefully shutdown.
				if (!process.HasExited)
				{
					// OK, no more Mr. Nice Guy time to just kill the process.
					process.Kill();
				}
			}
		}

		/// <summary>
		/// Starts a process without waiting for any feedback.
		/// </summary>
		private void StartProcess(string directory, string fileName, string arguments)
		{
			var filePath = directory + "\\" + fileName;
			WriteLine("StartProcess: " + filePath + " " + arguments);

			var processStart = new ProcessStartInfo(filePath);

			if (!String.IsNullOrWhiteSpace(arguments))
			{
				processStart.Arguments = arguments;
			}

			processStart.WorkingDirectory = directory;
			processStart.RedirectStandardOutput = false;
			processStart.RedirectStandardError = false;
			processStart.UseShellExecute = true;
			processStart.CreateNoWindow = true;

			System.Diagnostics.Process.Start(processStart);
		}

		/// <summary>
		/// Restarts the service after the update.
		/// </summary>
		private void StartServiceAfterUpdate()
		{
			if (Environment.UserInteractive)
			{
				// Starts the deployment service in runtime mode.
				StartProcess(Arguments.DirectoryToUpgrade, _applicationName + ".exe", Arguments.ServiceArguments);
			}
			else
			{
				// Starts the deployment service in service mode.
				using (var service = new ServiceController(Arguments.ServiceName))
				{
					service.Start();
					service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
				}
			}
		}

		private void StartServiceUpdate(WindowsServiceUpdate update)
		{
			var agentBits = DownloadUpdate(update);
			var directory = SaveUpdate(agentBits);

			StartProcess(directory, _applicationName + ".exe", Arguments.ServiceArguments + " -r \"" + _applicationDirectory + "\"");

			WriteLine("Shutting down service for updating.");
			Task.Run(() => OnStop());
		}

		/// <summary>
		/// Uninstalls the service.
		/// </summary>
		private void UninstallService()
		{
			try
			{
				WindowsServiceInstaller.UninstallService(Arguments.ServiceName);
			}
			catch (Exception ex)
			{
				WriteLine(ex.ToDetailedString(), LogLevel.Fatal);
			}
		}

		/// <summary>
		/// Updates the service.
		/// </summary>
		private void UpdateService()
		{
			WriteLine("Starting to update the service to v" + _version);

			// Shutdown all other agents.
			ShutdownServices();

			// Make sure all other agents are shutdown.
			WaitForServiceShutdown();

			// Make sure the current upgrading executable is not running in the target path.
			if (Arguments.DirectoryToUpgrade.Equals(_applicationDirectory, StringComparison.OrdinalIgnoreCase))
			{
				WriteLine("You cannot update from the same directory as the target.", LogLevel.Fatal);
				return;
			}

			// Copy the application directory to the upgrade directory.
			CopyDirectory(_applicationDirectory, Arguments.DirectoryToUpgrade);

			// Finished updating the server so log the success.
			WriteLine("Finished updating the service to v" + _version);

			// Start the service back up.
			var mode = Environment.UserInteractive ? "runtime" : "service";
			WriteLine(String.Format("Starting the updated service in {0} mode.", mode));
			StartServiceAfterUpdate();
		}

		/// <summary>
		/// Wait for the other services to shutdown.
		/// </summary>
		private void WaitForServiceShutdown()
		{
			// Get all the other service processes other than this one.
			var myProcess = System.Diagnostics.Process.GetCurrentProcess();
			var otherProcesses = System.Diagnostics.Process.GetProcessesByName(_applicationName)
				.Where(p => p.Id != myProcess.Id)
				.ToList();

			// Start a timeout timer so we can give up after 1 minute and 30 seconds.
			var watch = Stopwatch.StartNew();
			var timeout = new TimeSpan(0, 1, 30);

			// Keep checking for other processes for 1.5 minutes. The other service should have stopped within the timeout.
			while ((otherProcesses.Count > 0) && (watch.Elapsed < timeout))
			{
				// Display we are waiting.
				WriteLine("Waiting for the other services to shutdown.");

				// Delay for a second.
				Thread.Sleep(1000);

				// Refresh the process list.
				otherProcesses = System.Diagnostics.Process.GetProcessesByName(_applicationName)
					.Where(p => p.Id != myProcess.Id)
					.ToList();
			}

			// See if we timed out waiting for the other agents to stop.
			if (otherProcesses.Count > 0)
			{
				throw new Exception("The service failed to stop so we cannot update the service.");
			}
		}

		#endregion

		#region Static Methods

		private static void CreateOrCleanDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				// Empty the directory.
				new DirectoryInfo(path).Empty();
			}
			else
			{
				// Create a new temp folder and give it some time to complete.
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// Extracts the zip file contents into the directory then deletes the file.
		/// </summary>
		/// <param name="filePath">The path of the file.</param>
		/// <param name="outputPath">The path to extract the files to.</param>
		private static void ExtractZipfile(string filePath, string outputPath = "")
		{
			if (String.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("The filePath is required.", "filePath");
			}

			if (String.IsNullOrWhiteSpace(outputPath))
			{
				outputPath = Path.GetDirectoryName(filePath);
				if (outputPath == null)
				{
					throw new ArgumentException("Failed to calculate the output path.", "filePath");
				}

				outputPath += "\\" + Path.GetFileNameWithoutExtension(filePath);
			}

			// Make sure the file exist.
			if (!File.Exists(filePath))
			{
				// Oh no, we could not find the file. Let the caller know.
				throw new ArgumentException("The file does not exist.", "filePath");
			}

			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			// Open the zip file and start the extraction.
			using (var stream = File.OpenRead(filePath))
			{
				var zip = new ZipArchive(stream);
				zip.ExtractToDirectory(outputPath);
			}

			// Delete the zip.
			File.Delete(filePath);
		}

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

		#endregion

		#region Delegates

		private delegate void HandlerRoutine();

		#endregion
	}
}