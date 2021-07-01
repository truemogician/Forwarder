using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Forwarder.Converter {
	public class HttpResponseMessageConverter : JsonConverter<HttpResponseMessage> {
		public override void WriteJson(JsonWriter writer, HttpResponseMessage response, JsonSerializer serializer) {
			writer.WriteStartObject();
			writer.WritePropertyName("statusCode");
			writer.WriteValue((int)response.StatusCode);
			var headers = new Dictionary<string, string>();
			foreach ((string name, var values) in response.Headers)
				if (headers.ContainsKey(name))
					headers[name] += $",{string.Join(',', values)}";
				else
					headers[name] = string.Join(',', values);
			foreach ((string name, var values) in response.Content.Headers)
				if (headers.ContainsKey(name))
					headers[name] += $",{string.Join(',', values)}";
				else
					headers[name] = string.Join(',', values);
			writer.WritePropertyName("headers");
			writer.WriteRawValue(JsonConvert.SerializeObject(headers));
			writer.WriteValueWithName<HttpContent, HttpContentConverter>("content", response.Content);
			writer.WriteValueWithName<HttpResponseHeaders, HttpResponseHeadersConverter>("trailingHeaders", response.TrailingHeaders);
			writer.WriteValueWithName<HttpRequestMessage, HttpRequestMessageConverter>("request", response.RequestMessage);
			writer.WriteEndObject();
		}

		public override HttpResponseMessage ReadJson(JsonReader reader, Type objectType, HttpResponseMessage existingValue, bool hasExistingValue, JsonSerializer serializer) {
			var response = new HttpResponseMessage();
			var jObject = JObject.Load(reader);
			if (jObject.GetValue("statusCode") is {
				Type: JTokenType.Integer
			} statusCode)
				response.StatusCode = (HttpStatusCode)statusCode.ToObject<int>();
			if (jObject.GetValue<HttpResponseHeaders, HttpResponseHeadersConverter>("headers", JTokenType.Object) is { } headers)
				foreach ((string name, var value) in headers)
					response.TryAddHeader(name, value);
			if (jObject.GetValue<HttpContent, HttpContentConverter>("content", JTokenType.String) is { } content)
				response.Content = content;
			if (jObject.GetValue<HttpRequestMessage, HttpRequestMessageConverter>("request", JTokenType.Object) is { } request)
				response.RequestMessage = request;
			return response;
		}
	}
}