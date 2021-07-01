using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Forwarder.Converter {
	public abstract class HttpHeadersConverter<T> : JsonConverter<T> where T : HttpHeaders {
		protected abstract T CreateHeaders();

		public sealed override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer) {
			writer.WriteStartObject();
			foreach ((string name, var values) in value) {
				writer.WritePropertyName(name);
				writer.WriteValue(string.Join(',', values));
			}
			writer.WriteEndObject();
		}

		public sealed override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) {
			var result = CreateHeaders();
			result.Clear();
			foreach ((string key, var value) in JObject.Load(reader))
				switch (value.Type) {
					case JTokenType.String:
						result.TryAddWithoutValidation(key, value.ToObject<string>());
						break;
					case JTokenType.Array:
						result.TryAddWithoutValidation(key, value.ToObject<string[]>()!);
						break;
					case JTokenType.Null or JTokenType.Undefined: continue;
					default:                                      throw new JsonReaderException("Invalid header value type");
				}
			return result;
		}
	}

	public class HttpRequestHeadersConverter : HttpHeadersConverter<HttpRequestHeaders> {
		protected override HttpRequestHeaders CreateHeaders() => new HttpRequestMessage().Headers;
	}

	public class HttpContentHeadersConverter : HttpHeadersConverter<HttpContentHeaders> {
		protected override HttpContentHeaders CreateHeaders()
			=> new HttpRequestMessage {
				Content = new StringContent("")
			}.Content!.Headers;
	}

	public class HttpResponseHeadersConverter : HttpHeadersConverter<HttpResponseHeaders> {
		protected override HttpResponseHeaders CreateHeaders() => new HttpResponseMessage().Headers;
	}
}