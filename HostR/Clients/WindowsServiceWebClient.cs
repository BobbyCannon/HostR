#region References

using System.Net.Http;
using HostR.Services;
using Newtonsoft.Json;

#endregion

namespace HostR.Clients
{
	public class WindowsServiceWebClient<T> : IWindowsServiceWebService
		where T : class
	{
		#region Fields

		private readonly LoginHttpClient<T> _client;
		private readonly string _serviceRoute;

		#endregion

		#region Constructors

		public WindowsServiceWebClient(string baseUri, string serviceRoute, T credentials)
			: this(baseUri, serviceRoute, serviceRoute + "/Login", credentials)
		{
		}

		public WindowsServiceWebClient(string baseUri, string serviceRoute, string loginRoute, T credentials)
		{
			_serviceRoute = serviceRoute;
			_client = new LoginHttpClient<T>(baseUri, loginRoute, credentials);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks to see if there is an update for the service. The size of the update will be return. 
		/// If the service returns 0 if no update is available.
		/// </summary>
		/// <param name="details">The details of the service that is checking for the update.</param>
		/// <returns>The size of the update.</returns>
		public WindowsServiceUpdate CheckForUpdate(WindowsServiceDetails details)
		{
			using (var response = _client.Post(_serviceRoute + "/CheckForUpdate", details))
			{
				CheckResponse(response);

				using (var content = response.Content)
				{
					return JsonConvert.DeserializeObject<WindowsServiceUpdate>(content.ReadAsStringAsync().Result);
				}
			}
		}

		/// <summary>
		/// Downloads a chuck of the update based on the offset.
		/// </summary>
		/// <param name="request">The request to download the chuck for..</param>
		/// <returns>A chuck of the update starting from the update.</returns>
		public byte[] DownloadUpdateChunk(WindowsServiceUpdateRequest request)
		{
			using (var response = _client.Post(_serviceRoute + "/DownloadUpdateChunk", request))
			{
				CheckResponse(response);

				using (var content = response.Content)
				{
					return JsonConvert.DeserializeObject<byte[]>(content.ReadAsStringAsync().Result);
				}
			}
		}

		/// <summary>
		/// Validate the response for the request.
		/// </summary>
		/// <param name="response">The response to check.</param>
		protected void CheckResponse(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				return;
			}

			if (response.Content == null)
			{
				throw new HttpRequestException(response.StatusCode.ToString());
			}

			using (var content = response.Content)
			{
				throw new HttpRequestException(response.StatusCode + ": " + content.ReadAsStringAsync().Result);
			}
		}

		#endregion
	}
}