using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Serialization.Internal;
using Manatee.Json.Serialization.Internal.Serializers;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides access to custom serializers.
	/// </summary>
	public static class SerializerFactory
	{
		private static readonly ITypeSerializer _autoSerializer;
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
			_serializers = typeof(ISerializer).GetTypeInfo()
											  .Assembly
											  .DefinedTypes
											  .Where(t => t.ImplementedInterfaces.Contains(typeof(ISerializer)) &&
														  t.IsClass && !t.IsAbstract)
											  .Except(_dependentSerializers)
											  .Select(t => Activator.CreateInstance(t.AsType()))
											  .Cast<ISerializer>()
											  .ToList();
			_autoSerializer = _serializers.OfType<ITypeSerializer>().FirstOrDefault();
			_orderedSerializers = _serializers.OrderBy(s => (s as IPrioritizedSerializer)?.Priority ?? 0);
		}

		/// <summary>
		/// Adds a new custom serializer.
		/// </summary>
		/// <param name="serializer">The serializer to add.</param>
		public static void AddSerializer(ISerializer serializer)
		{
			var existing = _serializers.FirstOrDefault(s => s.GetType() == serializer.GetType());
			if (existing == null)
				_serializers.Add(serializer);
		}
		/// <summary>
		/// Removes a custom serializer.
		/// </summary>
		/// <typeparam name="T">The concrete type of serializer to remove.</typeparam>
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