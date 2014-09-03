#region References

using System;
using System.Text;

#endregion

namespace Hostr.Extensions
{
	public static partial class Helper
	{
		#region Static Methods

		public static string ToDetailedString(this Exception ex, bool includeStackTrace = false)
		{
			var builder = new StringBuilder();
			AddExceptionToBuilder(builder, ex, includeStackTrace);
			return builder.ToString();
		}

		private static void AddExceptionToBuilder(StringBuilder builder, Exception ex, bool includeStackTrace = false)
		{
			builder.Append(builder.Length > 0 ? "\r\n" + ex.Message : ex.Message);

			if (includeStackTrace && !string.IsNullOrEmpty(ex.StackTrace))
			{
				builder.Append("\r\n" + ex.StackTrace);
			}

			if (ex.InnerException != null)
			{
				AddExceptionToBuilder(builder, ex.InnerException);
			}
		}

		#endregion
	}
}