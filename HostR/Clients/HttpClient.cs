#region References

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

#endregion

namespace HostR.Clients
{
	/// <summary>
	/// This client is used for making GET and POST calls to an HTTP endpoint.
	/// </summary>
	public class HttpClient
	{
		#region Constructors

		/// <summary>
		/// Initializes a new HTTP helper to point at a specific URI, and with the specified session identifier.
		/// </summary>
		/// <param name="baseUri"></param>
		public HttpClient(string baseUri)
		{
			AuthorizationCookieName = ".ASPXAUTH";
			BaseUri = baseUri;
			Cookies = new CookieCollection();
			Timeout = new TimeSpan(0, 0, 100);
		}

		#endregion

		#region Properties

		public string AuthorizationCookieName { get; set; }
		public string BaseUri { get; set; }
		public CookieCollection Cookies { get; set; }

		public bool IsAuthenticated
		{
			get { return Cookies.Cast<Cookie>().Any(cookie => cookie.Name == AuthorizationCookieName && !cookie.Expired); }
		}

		/// <summary>
		/// Gets or sets the number of milliseconds to wait before the request times out. The default value is 100 seconds.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		#endregion

		#region Methods

		public virtual HttpResponseMessage Get(string uri)
		{
			using (var handler = new HttpClientHandler())
			{
				using (var client = CreateHttpClient(uri, handler))
				{
					var response = client.GetAsync(uri).Result;
					return ProcessResponse(response, handler);
				}
			}
		}

		public virtual HttpResponseMessage Post<TContent>(string uri, TContent content)
		{
			return InternalPost(uri, content);
		}

		private System.Net.Http.HttpClient CreateHttpClient(string uri, HttpClientHandler handler)
		{
			foreach (Cookie ck in Cookies)
			{
				handler.CookieContainer.Add(ck);
			}

			var client = new System.Net.Http.HttpClient(handler);
			client.BaseAddress = new Uri(BaseUri);
			client.Timeout = Timeout;

			return client;
		}

		private HttpResponseMessage InternalPost<T>(string uri, T content)
		{
			using (var handler = new HttpClientHandler())
			{
				using (var client = CreateHttpClient(uri, handler))
				{
					var data = JsonConvert.SerializeObject(content);
					using (var stringContent = new StringContent(data, Encoding.UTF8, "application/json"))
					{
						var response = client.PostAsync(uri, stringContent).Result;
						return ProcessResponse(response, handler);
					}
				}
			}
		}

		private HttpResponseMessage ProcessResponse(HttpResponseMessage response, HttpClientHandler handler)
		{
			if (handler.CookieContainer != null && Uri.IsWellFormedUriString(BaseUri, UriKind.Absolute))
			{
				Cookies = handler.CookieContainer.GetCookies(new Uri(BaseUri));
			}

			return response;
		}

		#endregion
	}
}