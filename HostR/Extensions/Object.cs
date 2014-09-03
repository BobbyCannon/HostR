#region References

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#endregion

namespace Hostr.Extensions
{
	public static partial class Helper
	{
		#region Static Methods

		public static T CreateInstance<T>(this string fullName, object[] args = null) where T : class
		{
			return CreateInstance(fullName, args) as T;
		}

		public static object CreateInstance(this string fullName, object[] args = null)
		{
			var nameParts = fullName.Split(',');
			if (nameParts.Length < 2)
			{
				throw new ArgumentException("The full name is in an invalid format.");
			}

			var assemblyName = nameParts[1];
			var typeName = nameParts[0];
			var flags = BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.IgnoreCase | BindingFlags.IgnoreReturn | BindingFlags.Instance;
			return Activator.CreateInstance(assemblyName, typeName, false, flags, null, args, null, null).Unwrap();
		}

		public static T DeepClone<T>(this T item)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, item);
				ms.Position = 0;
				return (T) formatter.Deserialize(ms);
			}
		}

		public static string ToTypeString<T>(this T item)
		{
			var builder = new StringBuilder();
			var t = item.GetType();
			var pis = t.GetProperties();

			for (var i = 0; i < pis.Length; i++)
			{
				var pi = (PropertyInfo) pis.GetValue(i);
				builder.AppendFormat("{0}: {1}" + Environment.NewLine, pi.Name, pi.GetValue(item));
			}

			return builder.ToString();
		}

		#endregion
	}
}