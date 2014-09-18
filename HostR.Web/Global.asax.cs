#region References

using System;
using System.Web;
using System.Web.Http;

#endregion

namespace HostR.Web
{
	public class Global : HttpApplication
	{
		#region Methods

		protected void Application_Start(object sender, EventArgs e)
		{
			WebApiConfig.Register(GlobalConfiguration.Configuration);
		}

		#endregion
	}
}