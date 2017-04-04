﻿using System;
using System.Collections.Generic;
using Manatee.Json.Internal;
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

		public static ISerializer GetSerializer<T>(JsonSerializerOptions options, JsonValue json = null)
		{
			var type = typeof (T);
			var typeToSerialize = JsonSerializationAbstractionMap.GetMap(type);
			if (typeof (IJsonSchema).IsAssignableFrom(typeToSerialize))
				return BuildSerializer(_schemaSerializer);
			if (JsonSerializationTypeRegistry.IsRegistered(typeToSerialize))
				return BuildSerializer(_registeredObjectSerializer);
			if (typeof (IJsonSerializable).IsAssignableFrom(typeToSerialize))
				return BuildSerializer(_jsonSerializableSerializer);
			if (typeof (Enum).IsAssignableFrom(typeToSerialize))
			{
				if (json != null)
				{
					if (json.Type == JsonValueType.Number)
						return BuildSerializer(_enumValueSerializer);
					if (json.Type == JsonValueType.String)
						return BuildSerializer(_enumNameSerializer);
				}
				switch (options.EnumSerializationFormat)
				{
					case EnumSerializationFormat.AsInteger:
						return BuildSerializer(_enumValueSerializer);
					case EnumSerializationFormat.AsName:
						return BuildSerializer(_enumNameSerializer);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			ISerializer serializer;
			if (_library.TryGetValue(typeToSerialize, out serializer))
				return BuildSerializer(serializer);
			return BuildSerializer(_autoSerializer);
		}
		public static ITypeSerializer GetTypeSerializer<T>(JsonSerializerOptions options)
		{
			return _autoSerializer;
		}

		private static ISerializer BuildSerializer(ISerializer innerSerializer)
		{
			return new DefaultValueSerializer(new ReferencingSerializer(innerSerializer));
		}
	}
}