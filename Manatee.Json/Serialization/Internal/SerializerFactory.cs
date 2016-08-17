/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		SerializerFactory.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		SerializerFactory
	Purpose:		Manages ISerializer implementations for use by the JsonSerializer.

***************************************************************************************/

using System;
using System.Collections.Generic;
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
			if (_library.ContainsKey(typeToSerialize))
				return BuildSerializer(_library[typeToSerialize]);
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