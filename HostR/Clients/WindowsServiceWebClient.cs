﻿#region References

using System.Net.Http;
using HostR.Services;
using Newtonsoft.Json;

#endregion

namespace HostR.Clients
{
	public class WindowsServiceWebClient : IWindowsServiceWebService
	{
		private readonly LoginCredentials _credentials;

		#region Fields

		private readonly HttpClient _client;

		#endregion

		#region Constructors

		public WindowsServiceWebClient(string uri, LoginCredentials credentials = null)
			: this (new HttpClient(uri), credentials)
		{
		}

		public WindowsServiceWebClient(HttpClient client, LoginCredentials credentials = null)
		{
			_client = client;
			_credentials = credentials;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks to see if there is an update for the service. The size of the update will be return. 
		/// If the service returns an empty name and 0 size if no update is available.
		/// </summary>
		/// <param name="details">The details of the service that is checking for the update.</param>
		/// <returns>The size of the update.</returns>
		public WindowsServiceUpdate CheckForUpdate(WindowsServiceDetails details)
		{
			if (_credentials != null)
			{
				Login(_credentials);
			}

			using (var response = _client.Post("CheckForUpdate", details))
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
			if (_credentials != null)
			{
				Login(_credentials);
			}

			using (var response = _client.Post("DownloadUpdateChunk", request))
			{
				CheckResponse(response);

				using (var content = response.Content)
				{
					return JsonConvert.DeserializeObject<byte[]>(content.ReadAsStringAsync().Result);
				}
			}
		}

		/// <summary>
		/// Allows the client to log in to the service. This only has to be implemented by services that require
		/// authentication. If you service does not require authentication then just leave this method not implemented.
		/// </summary>
		/// <param name="credentials">The credentials to use for authentication.</param>
		public void Login(LoginCredentials credentials)
		{
			using (var response = _client.Post("Login", credentials))
			{
				CheckResponse(response);
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