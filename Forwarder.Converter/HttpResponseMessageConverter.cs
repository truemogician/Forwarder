using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Forwarder.Converter {
	public class HttpResponseMessageConverter : JsonConverter<HttpResponseMessage> {
		public override void WriteJson(JsonWriter writer, HttpResponseMessage value, JsonSerializer serializer) {
			writer.WriteStartObject();
			writer.WritePropertyName("statusCode");
			writer.WriteValue((int)value.StatusCode);
			writer.WriteValueWithName<HttpResponseHeaders, HttpResponseHeadersConverter>("headers", value.Headers);
			writer.WriteValueWithName<HttpContent, HttpContentConverter>("content", value.Content);
			writer.WriteValueWithName<HttpResponseHeaders, HttpResponseHeadersConverter>("trailingHeaders", value.TrailingHeaders);
			writer.WriteValueWithName<HttpRequestMessage, HttpRequestMessageConverter>("request", value.RequestMessage);
			writer.WriteEndObject();
		}

		public override HttpResponseMessage ReadJson(JsonReader reader, Type objectType, HttpResponseMessage existingValue, bool hasExistingValue, JsonSerializer serializer) {
			var result = new HttpResponseMessage();
			var jObject = JObject.Load(reader);
			if (jObject.GetValue("statusCode") is {
				Type: JTokenType.Integer
			} statusCode)
				result.StatusCode = (HttpStatusCode)statusCode.ToObject<int>();
			if (jObject.GetValue<HttpResponseHeaders, HttpResponseHeadersConverter>("headers", JTokenType.Object) is { } headers)
				foreach ((string name, var value) in headers)
					result.TryAddHeader(name, value);
			if (jObject.GetValue<HttpContent, HttpContentConverter>("content", JTokenType.String) is { } content)
				result.Content = content;
			if (jObject.GetValue<HttpRequestMessage, HttpRequestMessageConverter>("request", JTokenType.Object) is { } request)
				result.RequestMessage = request;
			return result;
		}
	}
}