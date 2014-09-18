#region References

using System.Collections.Generic;
using System.Text;

#endregion

namespace HostR.Services
{
	public class WindowsServiceArguments
	{
		#region Fields

		private readonly string[] _arguments;

		#endregion

		#region Constructors

		public WindowsServiceArguments()
			: this(new string[0])
		{
		}

		public WindowsServiceArguments(string[] arguments)
		{
			_arguments = arguments;
			DirectoryToUpgrade = string.Empty;
			OtherValues = new Dictionary<string, string>();
			ServiceArguments = string.Empty;
			ServiceWebApi = string.Empty;
			ServiceWebApiPassword = string.Empty;
			ServiceWebApiUserName = string.Empty;
		}

		#endregion

		#region Properties

		public string DirectoryToUpgrade { get; set; }
		public bool InstallService { get; set; }
		public Dictionary<string, string> OtherValues { get; set; }
		public string ServiceArguments { get; set; }
		public string ServiceName { get; set; }
		public string ServiceWebApi { get; set; }
		public string ServiceWebApiPassword { get; set; }
		public string ServiceWebApiUserName { get; set; }
		public bool ShowHelp { get; set; }
		public bool UninistallService { get; set; }
		public bool UpdateService { get; set; }
		public bool WaitForDebugger { get; set; }

		#endregion

		#region Methods

		public void Parse()
		{
			var builder = new StringBuilder();
			string key = null;

			for (var i = 0; i < _arguments.Length; i++)
			{
				var argument = _arguments[i];

				switch (argument.ToLower())
				{
					case "-i":
					case "/i":
						InstallService = true;
						continue;

					case "-u":
					case "/u":
						UninistallService = true;
						continue;

					case "-n":
					case "/n":
						if (++i < _arguments.Length)
						{
							ServiceName = _arguments[i];
							builder.AppendFormat(" {0} {1}", argument, ServiceName);
						}
						continue;

					case "-w":
					case "/w":
						if (++i < _arguments.Length)
						{
							ServiceWebApi = _arguments[i];
							builder.AppendFormat(" {0} {1}", argument, ServiceWebApi);
						}
						continue;

					case "-l":
					case "/l":
						if (++i < _arguments.Length)
						{
							ServiceWebApiUserName = _arguments[i];
							builder.AppendFormat(" {0} {1}", argument, ServiceWebApiUserName);
						}

						continue;

					case "-p":
					case "/p":
						if (++i < _arguments.Length)
						{
							ServiceWebApiPassword = _arguments[i];
							builder.AppendFormat(" {0} {1}", argument, ServiceWebApiPassword);
						}
						continue;

					case "-r":
					case "/r":
						UpdateService = true;
						if (++i < _arguments.Length)
						{
							DirectoryToUpgrade = _arguments[i];
						}
						continue;

					case "-d":
					case "/d":
						WaitForDebugger = true;
						continue;

					case "-h":
					case "/h":
						ShowHelp = true;
						continue;
				}

				if (argument.StartsWith("-") || argument.StartsWith("/"))
				{
					key = argument;
					OtherValues.Add(key, string.Empty);
					continue;
				}

				if (key != null)
				{
					OtherValues[key] = OtherValues[key].Length > 0 ? " " + argument : argument;
				}
			}

			ServiceArguments = builder.ToString().Trim();
		}

		#endregion

		#region Static Methods

		public static WindowsServiceArguments Create(string[] args)
		{
			var response = new WindowsServiceArguments(args);
			response.Parse();
			return response;
		}

		#endregion
	}
}