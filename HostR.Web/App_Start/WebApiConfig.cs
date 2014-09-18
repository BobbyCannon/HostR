#region References

using System.Web.Http;

#endregion

namespace HostR.Web
{
	public static class WebApiConfig
	{
		#region Static Methods

		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
			config.Filters.Add(new AuthorizeAttribute());
		}

		#endregion
	}
}