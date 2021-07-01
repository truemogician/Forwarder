using System;
using Newtonsoft.Json;

namespace Forwarder.Converter {
	public abstract class SimpleJsonConverter<T> : JsonConverter<T> {
		protected abstract string MapToString(T source);
		protected abstract T MapFromString(string source);

		public sealed override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer) => writer.WriteValue(MapToString(value));

		public sealed override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) => reader.Value is string str ? MapFromString(str) : default;
	}
}