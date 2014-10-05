#region References

using HostR.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HostR.UnitTests.Services
{
	[TestClass]
	public class WindowsServiceArgumentsTests
	{
		#region Methods

		[TestMethod]
		public void ParseCustomArgument()
		{
			var expected = new WindowsServiceArguments();
			expected.OtherValues.Add("-c", "10");
			expected.ServiceArguments = "-c 10";

			var actual = WindowsServiceArguments.Create(new[] { "-c", "10" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseCustomArgumentWithoutValueForService()
		{
			var expected = new WindowsServiceArguments();
			expected.OtherValues.Add("-c", "");
			expected.ServiceArguments = "-c";

			var actual = WindowsServiceArguments.Create(new[] { "-c" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseDebugger()
		{
			var expected = new WindowsServiceArguments();
			expected.WaitForDebugger = true;

			var actual = WindowsServiceArguments.Create(new[] { "-d" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseDirectoryToUpgrade()
		{
			var expected = new WindowsServiceArguments();
			expected.UpdateService = true;
			expected.DirectoryToUpgrade = "C:\\Users\\Bobby\\Desktop\\Test";

			var actual = WindowsServiceArguments.Create(new[] { "-r", "C:\\Users\\Bobby\\Desktop\\Test" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseHelp()
		{
			var expected = new WindowsServiceArguments();
			expected.ShowHelp = true;

			var actual = WindowsServiceArguments.Create(new[] { "-?" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseInstall()
		{
			var expected = new WindowsServiceArguments();
			expected.InstallService = true;

			var actual = WindowsServiceArguments.Create(new[] { "-i" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseInstallUninstall()
		{
			var expected = new WindowsServiceArguments();
			expected.InstallService = true;
			expected.UninistallService = true;

			var actual = WindowsServiceArguments.Create(new[] { "-i", "-u" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseServiceName()
		{
			var expected = new WindowsServiceArguments();
			expected.ServiceName = "MyService";
			expected.ServiceArguments = "-n MyService";

			var actual = WindowsServiceArguments.Create(new[] { "-n", "MyService" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseServiceWebApi()
		{
			var expected = new WindowsServiceArguments();
			expected.ServiceWebApi = "http://localhost";
			expected.ServiceArguments = "-w http://localhost";

			var actual = WindowsServiceArguments.Create(new[] { "-w", "http://localhost" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseServiceWebApiUserNamePassword()
		{
			var expected = new WindowsServiceArguments();
			expected.ServiceWebApi = "http://localhost";
			expected.ServiceWebApiPassword = "P@ssw0rd!";
			expected.ServiceWebApiUserName = "admin";
			expected.ServiceArguments = "-w http://localhost -l admin -p P@ssw0rd!";

			var actual = WindowsServiceArguments.Create(new[] { "-w", "http://localhost", "-l", "admin", "-p", "P@ssw0rd!" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseUninstall()
		{
			var expected = new WindowsServiceArguments();
			expected.UninistallService = true;

			var actual = WindowsServiceArguments.Create(new[] { "-u" });
			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseVerbose()
		{
			var expected = new WindowsServiceArguments();
			expected.VerboseLogging = true;
			expected.ServiceArguments = "-v";

			var actual = WindowsServiceArguments.Create(new[] { "-v" });
			TestHelper.AreEqual(expected, actual);
		}

		#endregion
	}
}