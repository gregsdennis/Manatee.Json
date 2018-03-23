using System;
using System.Reflection;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options)
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