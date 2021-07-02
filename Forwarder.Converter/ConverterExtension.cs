using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Forwarder.Converter {
	public static class JsonWriterExtension {
		public static void WriteValue<T, TConverter>(this JsonWriter writer, T value) where T : class where TConverter : JsonConverter<T>, new() => writer.WriteValue(value, new TConverter());

		public static void WriteValue<T>(this JsonWriter writer, T value, JsonConverter<T> converter) {
			if (value is null)
				writer.WriteNull();
			else
				writer.WriteRawValue(JsonConvert.SerializeObject(value, converter));
		}

		public static void WriteValueWithName<T>(this JsonWriter writer, string name, T value) {
			writer.WritePropertyName(name);
			writer.WriteRawValue(JsonConvert.SerializeObject(value));
		}

		public static void WriteValueWithName<T, TConverter>(this JsonWriter writer, string name, T value) where TConverter : JsonConverter<T>, new() => writer.WriteValueWithName(name, value, new TConverter());

		public static void WriteValueWithName<T>(this JsonWriter writer, string name, T value, JsonConverter<T> converter) {
			writer.WritePropertyName(name);
			writer.WriteValue(value, converter);
		}
	}

	public static class JObjectExtension {
		public static T GetValue<T>(this JObject jObject, string name, JTokenType type) {
			if (jObject.GetValue(name) is { } token && token.Type == type)
				return token.ToObject<T>();
			return default;
		}

		public static T GetValue<T>(this JObject jObject, string name, JTokenType type, JsonConverter<T> converter) {
			if (jObject.GetValue(name) is { } token && token.Type == type)
				return token.ToObject<T>(
					new JsonSerializer {
						Converters = {
							converter
						}
					}
				);
			return default;
		}

		public static T GetValue<T, TConverter>(this JObject jObject, string name, JTokenType type) where TConverter : JsonConverter<T>, new() => jObject.GetValue(name, type, new TConverter());
	}

	public static class HttpExtension {
		#nullable enable
		public static Encoding? GetEncoding(this HttpContent content) {
			string? charset = content.Headers.ContentType?.CharSet;
			if (charset is null)
				return null;
			try {
				return Encoding.GetEncoding(charset);
			}
			catch (Exception) {
				return null;
			}
		}

		private static bool Contains(HttpHeaders? headers, string name) {
			try {
				return headers?.Contains(name) == true;
			}
			catch {
				return false;
			}
		}

		private static bool Remove(HttpHeaders? headers, string name) {
			try {
				return headers?.Remove(name) == true;
			}
			catch {
				return false;
			}
		}

		public static bool ContainsHeader(this HttpRequestMessage request, string name) => Contains(request.Headers, name) || Contains(request.Content?.Headers, name);

		public static bool TryRemoveHeader(this HttpRequestMessage request, string name) => Remove(request.Headers, name) || Remove(request.Content?.Headers, name);

		public static bool TryAddHeader(this HttpRequestMessage request, string name, string? value)
			=> request.Headers.TryAddWithoutValidation(name, value) ||
				request.Content?.Headers.TryAddWithoutValidation(name, value) == true;

		public static bool TryAddHeader(this HttpRequestMessage request, string name, IEnumerable<string?> values) {
			string?[] valueArray = values as string?[] ?? values.ToArray();
			return request.Headers.TryAddWithoutValidation(name, valueArray) ||
				request.Content?.Headers.TryAddWithoutValidation(name, valueArray) == true;
		}

		public static bool ContainsHeader(this HttpResponseMessage response, string name)
			=> Contains(response.Headers, name) ||
				Contains(response.Content.Headers, name) ||
				Contains(response.TrailingHeaders, name);

		public static bool TryRemoveHeader(this HttpResponseMessage response, string name)
			=> Remove(response.Headers, name) ||
				Remove(response.Content.Headers, name) ||
				Remove(response.TrailingHeaders, name);

		public static bool TryAddHeader(this HttpResponseMessage response, string name, string? value)
			=> response.Headers.TryAddWithoutValidation(name, value) ||
				response.Content.Headers.TryAddWithoutValidation(name, value) ||
				response.TrailingHeaders.TryAddWithoutValidation(name, value);

		public static bool TryAddHeader(this HttpResponseMessage response, string name, IEnumerable<string?> values) {
			string?[] valueArray = values as string?[] ?? values.ToArray();
			return response.Headers.TryAddWithoutValidation(name, valueArray) ||
				response.Content.Headers.TryAddWithoutValidation(name, valueArray) ||
				response.TrailingHeaders.TryAddWithoutValidation(name, valueArray);
		}
		#nullable disable
	}
}