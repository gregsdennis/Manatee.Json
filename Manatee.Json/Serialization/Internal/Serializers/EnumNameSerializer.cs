using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class EnumNameSerializer : ISerializer
	{
		private class Description
		{
			public object Value { get; set; }
			public string String { get; set; }
		}

		private static readonly Dictionary<Type, List<Description>> _descriptions = new Dictionary<Type,List<Description>>();

		public bool ShouldMaintainReferences => false;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			_EnsureDescriptions<T>();
			var attributes = typeof (T).GetTypeInfo().GetCustomAttributes(typeof (FlagsAttribute), false);
			if (attributes.Any()) return _BuildFlagsValues(obj, serializer.Options);

			var entry = _descriptions[typeof (T)].FirstOrDefault(d => Equals(d.Value, obj));
			if (entry != null) return entry.String;

			var enumValue = serializer.Options.SerializationNameTransform(obj.ToString());
			return enumValue;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			_EnsureDescriptions<T>();
			var options = serializer.Options.CaseSensitiveDeserialization
							  ? StringComparison.OrdinalIgnoreCase
							  : StringComparison.Ordinal;
			var entry = _descriptions[typeof (T)].FirstOrDefault(d => string.Equals(d.String, json.String, options));
			if (entry == null)
			{
				var enumValue = serializer.Options.DeserializationNameTransform(json.String);
				return (T) Enum.Parse(typeof(T), enumValue, !serializer.Options.CaseSensitiveDeserialization);
			}
			return (T) entry.Value;
		}

		private static void _EnsureDescriptions<T>()
		{
			lock (_descriptions)
			{
				var type = typeof(T);
				if (_descriptions.ContainsKey(type)) return;

				var names = Enum.GetValues(type).Cast<T>();
				var descriptions = names.Select(n => new Description { Value = n, String = _GetDescription<T>(n.ToString()) }).ToList();
				_descriptions.Add(type, descriptions);
			}
		}
		private static string _GetDescription<T>(string name)
		{
			var type = typeof(T);
			var memInfo = type.GetTypeInfo().GetDeclaredField(name);
			var attributes = memInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
			return attributes.Any() ? ((DisplayAttribute) attributes.First()).Description : name;
		}
		private static string _BuildFlagsValues<T>(T obj, JsonSerializerOptions options)
		{
			var descriptions = _descriptions[typeof (T)];
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