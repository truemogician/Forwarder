using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Util;
using Forwarder.Converter;
using Newtonsoft.Json;

namespace Forwarder.Client.Flurl {
	/// <summary>
	/// </summary>
	public class ForwarderResponse : IFlurlResponse {
		internal ForwarderResponse(IFlurlResponse response, ForwarderRequest request, HttpRequestMessage targetRequest) {
			OriginalResponse = response;
			Request = request;
			TargetRequest = targetRequest;
			TargetResponse = (FlurlResponse)this;
		}

		/// <summary>
		/// </summary>
		public bool ForwardSuccess => OriginalResponse.ResponseMessage.IsSuccessStatusCode;

		/// <summary>
		/// </summary>
		public IFlurlResponse OriginalResponse { get; }

		/// <summary>
		/// </summary>
		protected FlurlResponse TargetResponse { get; }

		/// <summary>
		/// </summary>
		public ForwarderRequest Request { get; }

		/// <summary>
		/// </summary>
		protected HttpRequestMessage TargetRequest { get; }

		/// <inheritdoc />
		public IReadOnlyNameValueList<string> Headers => TargetResponse?.Headers;

		/// <inheritdoc />
		public IReadOnlyList<FlurlCookie> Cookies => TargetResponse?.Cookies.ToList();

		/// <inheritdoc />
		public HttpResponseMessage ResponseMessage => TargetResponse?.ResponseMessage;

		/// <inheritdoc />
		public int StatusCode => TargetResponse.StatusCode;

		/// <inheritdoc />
		public Task<T> GetJsonAsync<T>() => TargetResponse?.GetJsonAsync<T>();

		/// <inheritdoc />
		public Task<dynamic> GetJsonAsync() => TargetResponse?.GetJsonAsync();

		/// <inheritdoc />
		public Task<IList<dynamic>> GetJsonListAsync() => TargetResponse?.GetJsonListAsync();

		/// <inheritdoc />
		public Task<string> GetStringAsync() => TargetResponse?.GetStringAsync();

		/// <inheritdoc />
		public Task<byte[]> GetBytesAsync() => TargetResponse?.GetBytesAsync();

		/// <inheritdoc />
		public Task<Stream> GetStreamAsync() => TargetResponse?.GetStreamAsync();

		/// <inheritdoc />
		public void Dispose() {
			OriginalResponse.Dispose();
			TargetResponse?.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// </summary>
		/// <param name="forwarderResponse"></param>
		/// <returns></returns>
		public static explicit operator FlurlResponse(ForwarderResponse forwarderResponse) {
			if (!forwarderResponse.ForwardSuccess)
				return null;
			var task = forwarderResponse.OriginalResponse.GetStringAsync();
			task.Wait();
			var response = JsonConvert.DeserializeObject<HttpResponseMessage>(task.Result, new HttpResponseMessageConverter());
			response!.RequestMessage = forwarderResponse.TargetRequest;
			return new FlurlResponse(response);
		}
	}
}