using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Schema;
using Manatee.Json.Serialization.Internal;
using Manatee.Json.Serialization.Internal.Serializers;

namespace Manatee.Json.Serialization
{
	public static class SerializerFactory
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
		private static readonly List<ISerializer> _userSerializers;
		private static readonly List<ISerializer> _serializers;
		private static readonly TypeInfo[] _dependentSerializers =
			{
				typeof(ReferencingSerializer).GetTypeInfo(),
				typeof(SchemaValidator).GetTypeInfo(),
				typeof(DefaultValueSerializer).GetTypeInfo()
			};
		private static readonly IEnumerable<ISerializer> _orderedSerializers;

		static SerializerFactory()
		{
			// todo: reduce this to a single, prioritized list
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
					{typeof (string), _stringSerializer}
				};
			_userSerializers = new List<ISerializer>
				{
					new ArraySerializer(),
				};
			_serializers = typeof(ISerializer).GetTypeInfo()
			                                     .Assembly
			                                     .DefinedTypes
			                                     .Where(t => t.ImplementedInterfaces.Contains(typeof(ISerializer)) &&
			                                                 t.IsClass && !t.IsAbstract)
			                                     .Except(_dependentSerializers)
			                                     .Select(t => Activator.CreateInstance(t.AsType()))
			                                     .Cast<ISerializer>()
			                                     .ToList();
			_orderedSerializers = _serializers.OrderBy(s => (s as IPrioritizedSerializer)?.Priority ?? 0);
		}

		public static void AddSerializer(ISerializer serializer)
		{
			var existing = _serializers.FirstOrDefault(s => s.GetType() == serializer.GetType());
			if (existing == null)
				_serializers.Add(serializer);
		}
		public static void RemoveSerializer<T>()
			where T : ISerializer
		{
			var serializer = _serializers.OfType<T>().FirstOrDefault();
			_serializers.Remove(serializer);
		}

		internal static ISerializer GetSerializer(Type typeToSerialize, JsonSerializer serializer, JsonValue json = null)
		{
			typeToSerialize = serializer.AbstractionMap.GetMap(typeToSerialize);
			var theChosenOne = _orderedSerializers.FirstOrDefault(s => s.Handles(typeToSerialize, serializer.Options, json));

			return _BuildSerializer(theChosenOne, typeToSerialize.GetTypeInfo());

			//if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeInfo))
			//{
			//	if (json != null)
			//	{
			//		if (json.Type == JsonValueType.Number)
			//			return _BuildSerializer(_enumValueSerializer, typeInfo);
			//		if (json.Type == JsonValueType.String)
			//			return _BuildSerializer(_enumNameSerializer, typeInfo);
			//	}
			//	switch (serializer.Options.EnumSerializationFormat)
			//	{
			//		case EnumSerializationFormat.AsInteger:
			//			return _BuildSerializer(_enumValueSerializer, typeInfo);
			//		case EnumSerializationFormat.AsName:
			//			return _BuildSerializer(_enumNameSerializer, typeInfo);
			//		default:
			//			throw new ArgumentOutOfRangeException();
			//	}
			//}
		}
		internal static ITypeSerializer GetTypeSerializer()
		{
			return _autoSerializer;
		}

		private static ISerializer _BuildSerializer(ISerializer innerSerializer, TypeInfo typeInfo)
		{
			if (!typeInfo.IsValueType && innerSerializer.ShouldMaintainReferences)
				innerSerializer = new ReferencingSerializer(innerSerializer);

			return new SchemaValidator(new DefaultValueSerializer(innerSerializer));
		}
	}
}