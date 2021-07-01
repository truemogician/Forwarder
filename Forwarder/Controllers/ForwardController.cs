using System.Net.Http;
using System.Threading.Tasks;
using Forwarder.Converter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Forwarder.Controllers {
	[ApiController]
	public class ForwardController : ControllerBase {
		private static readonly HttpClient Client = new();
		public ForwardController(ILogger<ForwardController> logger) => Logger = logger;
		private ILogger<ForwardController> Logger { get; }

		[HttpPost("/")]
		public async Task<dynamic> Forward([FromBody] HttpRequestMessage request) {
			Logger.Log(LogLevel.Information, $"URL: {request.RequestUri?.AbsoluteUri}");
			Logger.Log(LogLevel.Debug, $"Request: {JsonConvert.SerializeObject(request, new HttpRequestMessageConverter())}");
			HttpResponseMessage response = null;
			try {
				response = await Client.SendAsync(request);
				Logger.Log(LogLevel.Information, $"Status Code: {response.StatusCode}");
			}
			catch (HttpRequestException ex) {
				Logger.Log(LogLevel.Error, $"Status Code: {ex.StatusCode}, Error Message: {ex.Message}");
			}
			Logger.Log(LogLevel.Debug, $"Response: {JsonConvert.SerializeObject(response, new HttpResponseMessageConverter())}");
			return Ok(response);
			/*_logger.Log(LogLevel.Information, $"URL: {xhrRequest.Url}");
			_logger.Log(LogLevel.Debug, $"Request: {JsonConvert.SerializeObject(xhrRequest)}");
			var cookieJar = new CookieJar();
			foreach ((string name, string value) in xhrRequest.Cookies)
				cookieJar.AddOrReplace(new FlurlCookie(name, value, xhrRequest.Url.Root));
			var request = xhrRequest.Url.AllowAnyHttpStatus().WithCookies(cookieJar).WithHeaders(xhrRequest.Headers);
			string encodingName = xhrRequest.Headers.TryGetValue("Content-Type", out string contentType)
				? ContentTypeCharset.Match(contentType) is var match && match.Success
					? match.Groups[1].Value
					: null
				: null;
			var encoding = Encoding.UTF8;
			if (!string.IsNullOrEmpty(encodingName))
				try {
					encoding = Encoding.GetEncoding(encodingName);
				}
				catch (Exception) {
					// ignored
				}
			var response = await request.SendAsync(xhrRequest.Method, new StringContent(xhrRequest.Body, encoding));
			var xhrResponse = new XhrResponse {
				StatusCode = response.StatusCode,
				Cookies = response.Cookies.ToList(),
				Headers = new Dictionary<string, string>(),
				Data = await response.GetStringAsync()
			};
			foreach ((string name, string value) in response.Headers)
				if (!xhrRequest.Headers.ContainsKey(name))
					xhrRequest.Headers.Add(name, value);
				else
					xhrRequest.Headers[name] += $", {value}";
			_logger.Log(LogLevel.Information, $"Status Code: {xhrResponse.StatusCode}");
			_logger.Log(LogLevel.Debug, $"Response: {JsonConvert.SerializeObject(xhrResponse)}");
			return Ok(xhrResponse);*/
		}
	}
}