using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaValidator : ISerializer
	{
		private static readonly ConcurrentDictionary<TypeInfo, JsonSchema> _schemas
			= new ConcurrentDictionary<TypeInfo, JsonSchema>();

		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences => _innerSerializer.ShouldMaintainReferences;

		public SchemaValidator(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return true;
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			return _innerSerializer.Serialize(context, location);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			var typeInfo = typeof(T).GetTypeInfo();
			var schema = _GetSchema(typeInfo);
			if (schema != null)
			{
				var results = schema.Validate(json);
				if (!results.IsValid)
					throw new JsonSerializationException($"JSON did not pass schema defined by type '{typeof(T)}'.\n" +
					                                     "Errors:\n" +
					                                     serializer.Serialize(results));
			}

			return _innerSerializer.Deserialize<T>(context, root);
		}
		private static JsonSchema _GetSchema(TypeInfo typeInfo)
		{
			return _schemas.GetOrAdd(typeInfo, _GetSchemaSlow);
		}
		private static JsonSchema _GetSchemaSlow(TypeInfo typeInfo)
		{
			var attribute = typeInfo.GetCustomAttribute<SchemaAttribute>();
			if (attribute == null)
				return null;

			Exception exception = null;
			JsonSchema schema = null;
			try
			{
				schema = _GetPropertySchema(typeInfo, attribute) ?? _GetFileSchema(attribute);
			}
			catch (FileNotFoundException e)
			{
				exception = e;
			}
			catch (UriFormatException e)
			{
				exception = e;
			}

			if (schema == null)
				throw new JsonSerializationException($"The value '{attribute.Source}' could not be translated into a valid schema. " +
				                                     $"This value should represent either a public static property on the {typeInfo.Name} type " +
				                                     $"or a file with this name should exist at the execution path.", exception);

			return schema;
		}
		private static JsonSchema _GetPropertySchema(TypeInfo typeInfo, SchemaAttribute attribute)
		{
			var propertyName = attribute.Source;
			var property = typeInfo.GetAllProperties()
			                       .FirstOrDefault(p => typeof(JsonSchema).GetTypeInfo().IsAssignableFrom(p.PropertyType.GetTypeInfo()) &&
			                                            p.GetMethod.IsStatic && p.Name == propertyName);
			var schema = (JsonSchema) property?.GetMethod.Invoke(null, new object[] { });
			return schema;
		}
		private static JsonSchema _GetFileSchema(SchemaAttribute attribute)
		{
			var uri = attribute.Source;
			if (!Uri.TryCreate(uri, UriKind.Absolute, out _))
			{
				uri = System.IO.Path.Combine(Directory.GetCurrentDirectory(), uri);
			}
			var schema = JsonSchemaRegistry.Get(uri);

			return schema;
		}
	}
}
