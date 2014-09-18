#region References

using System.IO;

#endregion

namespace HostR.Extensions
{
	public static partial class Helper
	{
		#region Static Methods

		/// <summary>
		/// Empties a directory of all the files and the directories.
		/// </summary>
		/// <param name="directory">The directory to empty.</param>
		public static void Empty(this DirectoryInfo directory)
		{
			// See if the directory exists.
			if (!directory.Exists)
			{
				// Create the directory.
				directory.Create();
			}

			// Cycle through each file.
			foreach (var file in directory.GetFiles())
			{
				// Delete the file.
				file.Delete();
			}

			// Cycle through each sub directory.
			foreach (var subDirectory in directory.GetDirectories())
			{
				// Delete the directory.
				subDirectory.Delete(true);
			}
		}

		#endregion
	}
}