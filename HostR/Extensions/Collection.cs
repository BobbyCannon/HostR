#region References

using System.Collections.Generic;

#endregion

namespace Hostr.Extensions
{
	public static partial class Helper
	{
		#region Static Methods

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
		}

		#endregion
	}
}