using System;
using System.Reflection;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : IPrioritizedSerializer
	{
		public int Priority => int.MinValue;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return typeof(IJsonSchema).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var schema = (IJsonSchema) obj;
			return schema.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = JsonSchemaFactory.FromJson(json);
			return (T)value;
		}
	}
}