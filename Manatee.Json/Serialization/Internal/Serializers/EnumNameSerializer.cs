using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class EnumNameSerializer : IPrioritizedSerializer
	{
		private class Description
		{
			public object Value { get; set; }
			public string String { get; set; }
		}

		private static readonly Dictionary<Type, List<Description>> _descriptions = new Dictionary<Type,List<Description>>();

		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			var dContext = context as DeserializationContext;
			var serializing = dContext != null;
			return context.InferredType.GetTypeInfo().IsEnum &&
			       ((serializing && context.RootSerializer.Options.EnumSerializationFormat == EnumSerializationFormat.AsName) || // used during serialization
			        (!serializing && dContext.LocalValue?.Type == JsonValueType.String)); // used during deserialization
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var type = _GetType(context.InferredType);
			_EnsureDescriptions(type);
			var attributes = type.GetTypeInfo().GetCustomAttributes(typeof (FlagsAttribute), false);
			if (attributes.Any()) return _BuildFlagsValues(context.InferredType, context.Source, context.RootSerializer.Options);

			var entry = _descriptions[type].FirstOrDefault(d => Equals(d.Value, context.Source));
			if (entry != null) return entry.String;

			var enumValue = context.RootSerializer.Options.SerializationNameTransform(context.Source.ToString());
			return enumValue;
		}
		public object Deserialize(DeserializationContext context)
		{
			var type = _GetType(context.InferredType);
			_EnsureDescriptions(type);
			var options = context.RootSerializer.Options.CaseSensitiveDeserialization
							  ? StringComparison.OrdinalIgnoreCase
							  : StringComparison.Ordinal;
			var entry = _descriptions[type].FirstOrDefault(d => string.Equals(d.String, context.LocalValue.String, options));
			if (entry == null)
			{
				var enumValue = context.RootSerializer.Options.DeserializationNameTransform(context.LocalValue.String);
				return Enum.Parse(type, enumValue, !context.RootSerializer.Options.CaseSensitiveDeserialization);
			}
			return entry.Value;
		}

		private static Type _GetType(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = typeInfo.GenericTypeArguments[0];

			return type;
		}

		private static void _EnsureDescriptions(Type type)
		{
			lock (_descriptions)
			{
				if (_descriptions.ContainsKey(type)) return;

				var names = Enum.GetValues(type).Cast<object>();
				var descriptions = names.Select(n => new Description { Value = n, String = _GetDescription(type, n.ToString()) }).ToList();
				_descriptions.Add(type, descriptions);
			}
		}
		private static string _GetDescription(Type type, string name)
		{
			var memInfo = type.GetTypeInfo().GetDeclaredField(name);
			var attributes = memInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
			return attributes.Any() ? ((DisplayAttribute) attributes.First()).Description : name;
		}
		private static string _BuildFlagsValues(Type type, object obj, JsonSerializerOptions options)
		{
			var descriptions = _descriptions[type];
			var value = Convert.ToInt64(obj);
			var index = descriptions.Count - 1;
			var names = new List<string>();
			while (value > 0 && index > 0)
			{
				var compare = Convert.ToInt64(descriptions[index].Value);
				if (value >= compare)
				{
					names.Insert(0, descriptions[index].String);
					value -= compare;
				}
				index--;
			}
			return string.Join(options.FlagsEnumSeparator, names.Select(options.SerializationNameTransform));
		}
	}
}