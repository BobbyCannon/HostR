#region References

using System.Net;
using System.Net.Http;

#endregion

namespace HostR.Clients
{
	public class LoginHttpClient<TCredentials> : HttpClient 
		where TCredentials : class
	{
		#region Fields

		private readonly string _loginRoute;

		#endregion

		#region Constructors

		public LoginHttpClient(string baseUri, string loginRoute, TCredentials credentials)
			: base(baseUri)
		{
			Credentials = credentials;
			_loginRoute = loginRoute;
		}

		#endregion

		#region Properties

		public TCredentials Credentials { get; set; }

		#endregion

		#region Methods

		public override HttpResponseMessage Get(string uri)
		{
			if (!IsAuthenticated && !Login())
			{
				return new HttpResponseMessage(HttpStatusCode.Unauthorized);
			}

			return base.Get(uri);
		}

		public bool Login()
		{
			var results = base.Post(_loginRoute, Credentials);
			return results.IsSuccessStatusCode;
		}

		public override HttpResponseMessage Post<T>(string uri, T content)
		{
			if (!IsAuthenticated && !Login())
			{
				return new HttpResponseMessage(HttpStatusCode.Unauthorized);
			}

			return base.Post(uri, content);
		}

		#endregion
	}
}