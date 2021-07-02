using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Forwarder.Converter {
	/// <inheritdoc />
	public class HttpMethodConverter : SimpleJsonConverter<HttpMethod> {
		protected override string MapToString(HttpMethod source) => source.Method;

		protected override HttpMethod MapFromString(string source) => new(source.ToUpper());
	}

	public class UriConverter : SimpleJsonConverter<Uri> {
		protected override string MapToString(Uri source) => source.AbsoluteUri;

		protected override Uri MapFromString(string source) => new(source);
	}

	public class HttpRequestMessageConverter : JsonConverter<HttpRequestMessage> {
		public override void WriteJson(JsonWriter writer, HttpRequestMessage request, JsonSerializer serializer) {
			writer.WriteStartObject();
			writer.WriteValueWithName<HttpMethod, HttpMethodConverter>("method", request.Method);
			writer.WriteValueWithName<Uri, UriConverter>("url", request.RequestUri);
			var headers = new Dictionary<string, string>();
			foreach ((string name, var values) in request.Headers)
				if (headers.ContainsKey(name))
					headers[name] += $",{string.Join(',', values)}";
				else
					headers[name] = string.Join(',', values);
			if (request.Content is not null)
				foreach ((string name, var values) in request.Content.Headers)
					if (headers.ContainsKey(name))
						headers[name] += $",{string.Join(',', values)}";
					else
						headers[name] = string.Join(',', values);
			writer.WriteValueWithName("headers", headers);
			if (request.Content is not null)
				writer.WriteValueWithName<HttpContent, HttpContentConverter>("content", request.Content);
			writer.WriteEndObject();
		}

		public override HttpRequestMessage ReadJson(JsonReader reader, Type objectType, HttpRequestMessage existingValue, bool hasExistingValue, JsonSerializer serializer) {
			var request = new HttpRequestMessage();
			var jObject = JObject.Load(reader);
			if (jObject.GetValue<HttpMethod, HttpMethodConverter>("method", JTokenType.String) is { } method)
				request.Method = method;
			if (jObject.GetValue<Uri, UriConverter>("url", JTokenType.String) is { } url)
				request.RequestUri = url;
			if (jObject.GetValue<HttpContent, HttpContentConverter>("content", JTokenType.String) is { } content)
				request.Content = content;
			if (jObject.GetValue<Dictionary<string, string>>("headers", JTokenType.Object) is { } headers)
				foreach ((string name, string value) in headers) {
					if (request.ContainsHeader(name))
						request.TryRemoveHeader(name);
					request.TryAddHeader(name, value);
				}
			return request;
		}
	}
}