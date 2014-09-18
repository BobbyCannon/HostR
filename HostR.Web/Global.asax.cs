#region References

using System;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#endregion

namespace HostR.Web
{
	public class Global : HttpApplication
	{
		#region Methods

		protected void Application_Start(object sender, EventArgs e)
		{
			WebApiConfig.Register(GlobalConfiguration.Configuration);

			// Setup the JSON formatter.
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new IsoDateTimeConverter());
			settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

			// Check to see if debugging is enabled.
			if (!HttpContext.Current.IsDebuggingEnabled)
			{
				// Indent the formatting of the JSON so it's easier to view.
				settings.Formatting = Formatting.Indented;
			}

			// Add the JSON formatter then remove the XML formatter.
			GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = settings;
			GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
		}

		#endregion
	}
}