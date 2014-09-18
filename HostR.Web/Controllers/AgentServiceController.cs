#region References

using System.Web.Hosting;
using System.Web.Http;
using HostR.Services;

#endregion

namespace HostR.Web.Controllers
{
	public class AgentServiceController : ApiController, IWindowsServiceWebService
	{
		#region Fields

		private readonly WindowsServiceWebService _service;

		#endregion

		#region Constructors

		public AgentServiceController()
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
		public long CheckForUpdate([FromBody] WindowsServiceDetails details)
		{
			return _service.CheckForUpdate(details);
		}

		/// <summary>
		/// Downloads a chuck of the update based on the offset.
		/// </summary>
		/// <param name="request">The request to download the chuck for..</param>
		/// <returns>A chuck of the update starting from the update.</returns>
		[HttpPost]
		[ActionName("DownloadUpdateChunk")]
		public byte[] DownloadUpdateChunk([FromBody] WindowsServiceUpdateRequest request)
		{
			return _service.DownloadUpdateChunk(request);
		}

		#endregion
	}
}