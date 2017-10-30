﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Manatee.Json.Internal;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaValidator : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences => _innerSerializer.ShouldMaintainReferences;

		public SchemaValidator(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			return _innerSerializer.Serialize(obj, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var typeInfo = typeof(T).GetTypeInfo();
			var attribute = typeInfo.GetCustomAttribute<SchemaAttribute>();
			if (attribute != null)
			{
				Exception exception = null;
				IJsonSchema schema = null;
				try
				{
					schema = _GetSchema(typeInfo, attribute);
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
				{
					throw new JsonSerializationException($"The value '{attribute.Source}' could not be translated into a valid schema. " +
					                                     $"This value should represent either a public static property on the {typeof(T).Name} type " +
					                                     $"or a file with this name should exist at the execution path.", exception);
				}

				var results = schema.Validate(json);
				if (!results.Valid)
				{
					throw new JsonSerializationException($"JSON did not pass schema defined by type '{typeof(T)}'.\n" +
					                                     "Errors:\n" +
					                                     string.Join("    \n", results.Errors.Select(e => e.Message)));
				}
			}

			return _innerSerializer.Deserialize<T>(json, serializer);
		}

		private static IJsonSchema _GetSchema(TypeInfo typeInfo, SchemaAttribute attribute)
		{
			return _GetPropertySchema(typeInfo, attribute) ?? _GetFileSchema(attribute);
		}
		private static IJsonSchema _GetPropertySchema(TypeInfo typeInfo, SchemaAttribute attribute)
		{
			var propertyName = attribute.Source;
			var property = typeInfo.GetAllProperties()
			                       .FirstOrDefault(p => typeof(IJsonSchema).GetTypeInfo().IsAssignableFrom(p.PropertyType.GetTypeInfo()) &&
			                                            p.GetMethod.IsStatic && p.Name == propertyName);
			var schema = (IJsonSchema) property?.GetMethod.Invoke(null, new object[] { });
			return schema;
		}
		private static IJsonSchema _GetFileSchema(SchemaAttribute attribute)
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
