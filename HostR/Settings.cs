#region References

using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

#endregion

namespace Hostr
{
	public abstract class Settings
	{
		#region Fields

		private static readonly JsonSerializerSettings _settings;

		#endregion

		#region Constructors

		static Settings()
		{
			_settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects,
				TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
			};
		}

		#endregion

		#region Properties

		public string TargetFullName { get; set; }

		#endregion

		#region Methods

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented, _settings);
		}

		#endregion

		#region Static Methods

		public static object Deserialize(string data)
		{
			return JsonConvert.DeserializeObject(data, _settings);
		}

		#endregion
	}
}