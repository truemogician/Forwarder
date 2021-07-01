using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Forwarder.Converter {
	public class HttpContentConverter : JsonConverter<HttpContent> {
		public override void WriteJson(JsonWriter writer, HttpContent value, JsonSerializer serializer) {
			var stream = new MemoryStream();
			value.CopyTo(stream, null, CancellationToken.None);
			writer.WriteValue((value.GetEncoding() ?? Encoding.UTF8).GetString(stream.ToArray()));
		}

		public override HttpContent ReadJson(JsonReader reader, Type objectType, HttpContent existingValue, bool hasExistingValue, JsonSerializer serializer) {
			string buffer;
			if (reader is JTokenReader jReader)
				buffer = jReader.TokenType switch {
					JsonToken.String  => jReader.CurrentToken?.Value<string>(),
					JsonToken.Boolean => jReader.CurrentToken?.Value<bool>().ToString(),
					JsonToken.Integer => jReader.CurrentToken?.Value<int>().ToString(),
					JsonToken.Float   => jReader.CurrentToken?.Value<float>().ToString(CultureInfo.InvariantCulture),
					_                 => throw new JsonReaderException("Content is not a buffer or string")
				};
			else
				buffer = reader.ReadAsString();
			if (buffer is null)
				throw new JsonReaderException("Content is not a buffer or string");
			return new StringContent(buffer);
		}
	}
}