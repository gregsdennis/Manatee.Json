using System;
using System.Collections.Generic;
using System.Reflection;
using Manatee.Json.Schema;
using Manatee.Json.Serialization.Internal.Serializers;

namespace Manatee.Json.Serialization.Internal
{
	internal static class SerializerFactory
	{
		private static readonly AutoSerializer _autoSerializer;
		private static readonly BooleanSerializer _booleanSerializer;
		private static readonly EnumNameSerializer _enumNameSerializer;
		private static readonly EnumValueSerializer _enumValueSerializer;
		private static readonly JsonSerializableSerializer _jsonSerializableSerializer;
		private static readonly NumericSerializer _numericSerializer;
		private static readonly RegisteredObjectSerializer _registeredObjectSerializer;
		private static readonly StringSerializer _stringSerializer;
		private static readonly Dictionary<Type, ISerializer> _library;
		private static readonly SchemaSerializer _schemaSerializer;

		static SerializerFactory()
		{
			_autoSerializer = new AutoSerializer();
			_booleanSerializer = new BooleanSerializer();
			_enumNameSerializer = new EnumNameSerializer();
			_enumValueSerializer = new EnumValueSerializer();
			_jsonSerializableSerializer = new JsonSerializableSerializer();
			_numericSerializer = new NumericSerializer();
			_registeredObjectSerializer = new RegisteredObjectSerializer();
			_stringSerializer = new StringSerializer();
			_schemaSerializer = new SchemaSerializer();
			_library = new Dictionary<Type, ISerializer>
				{
					{typeof (sbyte), _numericSerializer},
					{typeof (byte), _numericSerializer},
					{typeof (char), _numericSerializer},
					{typeof (short), _numericSerializer},
					{typeof (ushort), _numericSerializer},
					{typeof (int), _numericSerializer},
					{typeof (uint), _numericSerializer},
					{typeof (long), _numericSerializer},
					{typeof (ulong), _numericSerializer},
					{typeof (float), _numericSerializer},
					{typeof (double), _numericSerializer},
					{typeof (decimal), _numericSerializer},
					{typeof (bool), _booleanSerializer},
					{typeof (string), _stringSerializer},
				};
		}

		public static ISerializer GetSerializer(Type typeToSerialize, JsonSerializerOptions options, JsonValue json = null)
		{
			typeToSerialize = JsonSerializationAbstractionMap.GetMap(typeToSerialize);
			var typeInfo = typeToSerialize.GetTypeInfo();
			if (typeof (IJsonSchema).GetTypeInfo().IsAssignableFrom(typeInfo))
				return _BuildSerializer(_schemaSerializer);
			if (JsonSerializationTypeRegistry.IsRegistered(typeToSerialize))
				return _BuildSerializer(_registeredObjectSerializer);
			if (typeof (IJsonSerializable).GetTypeInfo().IsAssignableFrom(typeInfo))
				return _BuildSerializer(_jsonSerializableSerializer);
			if (typeof (Enum).GetTypeInfo().IsAssignableFrom(typeInfo))
			{
				if (json != null)
				{
					if (json.Type == JsonValueType.Number)
						return _BuildSerializer(_enumValueSerializer);
					if (json.Type == JsonValueType.String)
						return _BuildSerializer(_enumNameSerializer);
				}
				switch (options.EnumSerializationFormat)
				{
					case EnumSerializationFormat.AsInteger:
						return _BuildSerializer(_enumValueSerializer);
					case EnumSerializationFormat.AsName:
						return _BuildSerializer(_enumNameSerializer);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return _BuildSerializer(_library.TryGetValue(typeToSerialize, out var serializer)
				                        ? serializer
				                        : _autoSerializer);
		}
		public static ITypeSerializer GetTypeSerializer<T>(JsonSerializerOptions options)
		{
			return _autoSerializer;
		}

		private static ISerializer _BuildSerializer(ISerializer innerSerializer)
		{
			return new SchemaValidator(
				new DefaultValueSerializer(
					new ReferencingSerializer(innerSerializer)));
		}
	}
}