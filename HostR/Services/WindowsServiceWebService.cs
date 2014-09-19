#region References

using System;
using System.IO;
using System.Linq;
using HostR.Interfaces;

#endregion

namespace HostR.Services
{
	public class WindowsServiceWebService : IWindowsServiceWebService
	{
		#region Fields

		private readonly string _appDataDirectory;

		#endregion

		#region Constructors

		public WindowsServiceWebService(string directory)
		{
			_appDataDirectory = directory;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks to see if there is an update for the service. The size of the update will be return. 
		/// If the service returns 0 if no update is available.
		/// </summary>
		/// <param name="details">The details of the service that is checking for the update.</param>
		/// <returns>The size of the update.</returns>
		public WindowsServiceUpdate CheckForUpdate(WindowsServiceDetails details)
		{
			var response = new WindowsServiceUpdate();
			if (!Directory.Exists(_appDataDirectory))
			{
				response.Size = -1;
				return response;
			}

			var filter = details.Name + "-*.zip";
			var zipFilePath = Directory.GetFiles(_appDataDirectory, filter).OrderByDescending(x => x).FirstOrDefault();
			if (zipFilePath == null)
			{
				response.Size = -2;
				return response;
			}

			var fileNameParts = Path.GetFileNameWithoutExtension(zipFilePath).Split('-');
			if (fileNameParts.Length != 2)
			{
				response.Size = -3;
				return response;
			}

			var version = fileNameParts[1];
			if (version != details.Version)
			{
				response.Name = Path.GetFileName(zipFilePath);
				response.Size = new FileInfo(zipFilePath).Length;
				return response;
			}

			return response;
		}

		/// <summary>
		/// Downloads a chuck of the update based on the offset.
		/// </summary>
		/// <param name="request">The request to download the chuck for..</param>
		/// <returns>A chuck of the update starting from the update.</returns>
		public byte[] DownloadUpdateChunk(WindowsServiceUpdateRequest request)
		{
			if (!Directory.Exists(_appDataDirectory))
			{
				throw new Exception("Could not find the directory update.");
			}

			var filePath = _appDataDirectory + "\\" + request.Name;
			var fileInfo = new FileInfo(filePath);
			return !fileInfo.Exists ? new byte[0] : FileChunk(fileInfo, request.Offset);
		}

		private static byte[] FileChunk(FileInfo info, long offset)
		{
			if (offset < 0 || offset >= info.Length)
			{
				throw new ArgumentException("The offset is out of range.", "offset");
			}

			using (var file = File.Open(info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				if (offset >= file.Length)
				{
					throw new ArgumentOutOfRangeException("offset", "The offset is larger than the file size.");
				}

				long length = 1048576;
				if ((offset + length) >= file.Length)
				{
					length = file.Length - offset;
				}

				var response = new byte[length];
				file.Position = offset;
				file.Read(response, 0, response.Length);
				return response;
			}
		}

		#endregion
	}
}