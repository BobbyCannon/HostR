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
		public void ParseHelp()
		{
			var expected = new WindowsServiceArguments();
			expected.ShowHelp = true;

			var actual = new WindowsServiceArguments(new[] { "-h" });
			actual.Parse();

			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseInstall()
		{
			var expected = new WindowsServiceArguments();
			expected.InstallService = true;

			var actual = new WindowsServiceArguments(new[] { "-i" });
			actual.Parse();

			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseInstallUninstall()
		{
			var expected = new WindowsServiceArguments();
			expected.InstallService = true;
			expected.UninistallService = true;

			var actual = new WindowsServiceArguments(new[] { "-i", "-u" });
			actual.Parse();

			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseServiceName()
		{
			var expected = new WindowsServiceArguments();
			expected.ServiceName = "MyService";
			expected.ServiceArguments = "-n MyService";

			var actual = new WindowsServiceArguments(new[] { "-n", "MyService" });
			actual.Parse();

			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseServiceWebApi()
		{
			var expected = new WindowsServiceArguments();
			expected.ServiceWebApi = "http://localhost/api/service";
			expected.ServiceArguments = "-w http://localhost/api/service";

			var actual = new WindowsServiceArguments(new[] { "-w", "http://localhost/api/service" });
			actual.Parse();

			TestHelper.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseUninstall()
		{
			var expected = new WindowsServiceArguments();
			expected.UninistallService = true;

			var actual = new WindowsServiceArguments(new[] { "-u" });
			actual.Parse();

			TestHelper.AreEqual(expected, actual);
		}

		#endregion
	}
}