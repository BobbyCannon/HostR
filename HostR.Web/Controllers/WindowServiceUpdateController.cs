#region References

using System.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Security;
using HostR.Clients;
using HostR.Services;

#endregion

namespace HostR.Web.Controllers
{
	public class WindowsServiceUpdateController : ApiController, IWindowsServiceWebService
	{
		#region Fields

		private readonly WindowsServiceWebService _service;

		#endregion

		#region Constructors

		public WindowsServiceUpdateController()
		{
			var directory = HostingEnvironment.MapPath("~/AppData/");
			_service = new WindowsServiceWebService(directory);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks to see if there is an update for the service. The size of the update will be return. 
		/// If the service returns 0 if no update is available.
		/// </summary>
		/// <param name="details">The details of the service that is checking for the update.</param>
		/// <returns>The size of the update.</returns>
		[HttpPost]
		[ActionName("CheckForUpdate")]
		public WindowsServiceUpdate CheckForUpdate([FromBody] WindowsServiceDetails details)
		{
			return _service.CheckForUpdate(details);
		}

		/// <summary>
		/// Downloads a chuck of the update based on the offset.
		/// </summary>
		/// <param name="request">The request to download the chuck for.</param>
		/// <returns>A chuck of the update starting from the update.</returns>
		[HttpPost]
		[ActionName("DownloadUpdateChunk")]
		public byte[] DownloadUpdateChunk([FromBody] WindowsServiceUpdateRequest request)
		{
			return _service.DownloadUpdateChunk(request);
		}

		[HttpPost]
		[ActionName("Login")]
		[AllowAnonymous]
		public void Login([FromBody] LoginCredentials credentials)
		{
			var expectedUserName = ConfigurationManager.AppSettings["UserName"];
			var expectedPassword = ConfigurationManager.AppSettings["Password"];

			if (credentials.UserName == expectedUserName && credentials.Password == expectedPassword)
			{
				FormsAuthentication.SetAuthCookie(credentials.UserName, false);
			}
		}

		#endregion
	}
}