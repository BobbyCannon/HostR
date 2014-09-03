#region References

using System;
using System.Collections.Generic;

#endregion

namespace Hostr.Extensions
{
	public static partial class Helper
	{
		#region Static Methods

		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}

		#endregion
	}
}