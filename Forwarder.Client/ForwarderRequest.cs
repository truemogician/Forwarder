using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Util;
using Forwarder.Converter;
using Newtonsoft.Json;

namespace Forwarder.Client.Flurl {
	/// <summary>
	/// </summary>
	public class ForwarderRequest : IFlurlRequest {
		/// <summary>
		/// </summary>
		/// <param name="request"></param>
		/// <param name="client"></param>
		public ForwarderRequest(IFlurlRequest request, ForwarderClient client) {
			OriginalRequest = request;
			Client = client;
		}

		/// <summary>
		/// </summary>
		public ForwarderClient Client { get; set; }

		/// <summary>
		/// </summary>
		protected IFlurlRequest OriginalRequest { get; set; }

		/// <inheritdoc />
		IFlurlClient IFlurlRequest.Client {
			get => OriginalRequest.Client;
			set => OriginalRequest.Client = value;
		}

		/// <inheritdoc />
		public HttpMethod Verb {
			get => OriginalRequest.Verb;
			set => OriginalRequest.Verb = value;
		}

		/// <inheritdoc />
		public Url Url {
			get => OriginalRequest.Url;
			set => OriginalRequest.Url = value;
		}

		/// <inheritdoc />
		public FlurlHttpSettings Settings {
			get => OriginalRequest.Settings;
			set => OriginalRequest.Settings = value;
		}

		/// <inheritdoc />
		public INameValueList<string> Headers => OriginalRequest.Headers;

		/// <inheritdoc />
		public IEnumerable<(string Name, string Value)> Cookies => OriginalRequest.Cookies;

		/// <inheritdoc />
		public CookieJar CookieJar {
			get => OriginalRequest.CookieJar;
			set => OriginalRequest.CookieJar = value;
		}

		/// <inheritdoc />
		public async Task<IFlurlResponse> SendAsync(HttpMethod verb, HttpContent content = null, CancellationToken cancellationToken = new(), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead) {
			var request = new HttpRequestMessage(verb, Url) {
				Content = content
			};
			request.Headers.Clear();
			request.Content?.Headers.Clear();
			foreach ((string name, string value) in Headers)
				request.TryAddHeader(name, value);
			foreach ((string name, string value) in Client.Headers)
				if (!Client.Options.IgnoreConflictingHeader || !request.Headers.Contains(name))
					request.TryAddHeader(name, value);
			//Cookies from request are already presented by Header
			request.Headers.TryAddWithoutValidation("Cookie", Client.Cookies.Where(cookie => cookie.Match(Url)).Select(cookie => $"{cookie.Name}={cookie.Value}"));
			if (Client.Options.AutoContentLength && request.Content is not null)
				request.Content.Headers.ContentLength = (await request.Content.ReadAsStreamAsync(cancellationToken)).Length;
			var response = await Client.BaseUrl
				.WithClient(((IFlurlRequest)this).Client)
				.WithHeader("Content-Type", "application/json; charset=utf-8")
				.PostAsync(new StringContent(JsonConvert.SerializeObject(request, new HttpRequestMessageConverter())), cancellationToken, completionOption);
			return new ForwarderResponse(response, this, request);
		}
	}
}