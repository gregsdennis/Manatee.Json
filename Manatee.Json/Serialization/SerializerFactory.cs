using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;
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
		private static List<ISerializer> _orderedSerializers;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		static SerializerFactory()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
			_autoSerializer = _serializers.OfType<AutoSerializer>().FirstOrDefault();
			_UpdateOrderedSerializers();
		}

		/// <summary>
		/// Adds a new custom serializer.
		/// </summary>
		/// <param name="serializer">The serializer to add.</param>
		public static void AddSerializer(ISerializer serializer)
		{
			var existing = _serializers.FirstOrDefault(s => s.GetType() == serializer.GetType());
			if (existing == null)
			{
				_serializers.Add(serializer);
			}
			_UpdateOrderedSerializers();
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
			_UpdateOrderedSerializers();
		}

		internal static ISerializer GetSerializer(SerializationContextBase context)
		{
			context.OverrideInferredType(context.RootSerializer.AbstractionMap.GetMap(context.InferredType ?? context.RequestedType));
			var theChosenOne = _orderedSerializers.First(s => s.Handles(context));

			if (theChosenOne is AutoSerializer && context.RequestedType != typeof(object))
			{
				var type = context.InferredType!;
				context.OverrideInferredType(context.RootSerializer.AbstractionMap.GetMap(context.RequestedType));
				theChosenOne = _orderedSerializers.First(s => s.Handles(context));

				if (theChosenOne is AutoSerializer)
					context.OverrideInferredType(type);
			}

			Log.Serialization(() => $"Serializer {theChosenOne.GetType().CSharpName() ?? "<not found>"} selected for type `{(context.InferredType ?? context.RequestedType).CSharpName()}`");
			return theChosenOne;
		}
		internal static ITypeSerializer GetTypeSerializer()
		{
			return _autoSerializer;
		}

		private static void _UpdateOrderedSerializers()
		{
			_orderedSerializers = _serializers.OrderBy(s => (s as IPrioritizedSerializer)?.Priority ?? 0).ToList();
		}
	}
}