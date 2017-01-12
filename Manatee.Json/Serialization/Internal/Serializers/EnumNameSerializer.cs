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
 
	File Name:		EnumNameSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		EnumNameSerializer
	Purpose:		Converts enumerations to and from JsonValues by name.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

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
			EnsureDescriptions<T>();
			var attributes = typeof (T).TypeInfo().GetCustomAttributes(typeof (FlagsAttribute), false);
			if (!attributes.Any())
			{
				var entry = _descriptions[typeof (T)].FirstOrDefault(d => Equals(d.Value, obj));
				return entry == null ? obj.ToString() : entry.String;
			}
			return BuildFlagsValues(obj, serializer.Options.FlagsEnumSeparator);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			EnsureDescriptions<T>();
			var options = serializer.Options.CaseSensitiveDeserialization
#if IOS || CORE
							  ? StringComparison.OrdinalIgnoreCase
							  : StringComparison.Ordinal;
#else
							  ? StringComparison.InvariantCultureIgnoreCase
							  : StringComparison.InvariantCulture;
#endif
			var entry = _descriptions[typeof (T)].FirstOrDefault(d => string.Equals(d.String, json.String, options));
			if (entry == null)
			{
				return (T) Enum.Parse(typeof (T), json.String);
			}
			return (T) entry.Value;
		}

		private static void EnsureDescriptions<T>()
		{
			lock (_descriptions)
			{
				var type = typeof(T);
				if (_descriptions.ContainsKey(type)) return;

				var names = Enum.GetValues(type).Cast<T>();
				var descriptions = names.Select(n => new Description { Value = n, String = GetDescription<T>(n.ToString()) }).ToList();
				_descriptions.Add(type, descriptions);
			}
		}
		private static string GetDescription<T>(string name)
		{
			var type = typeof (T);
			var memInfo = type.TypeInfo().GetMember(name);
			var attributes = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
			return attributes.Any() ? ((DescriptionAttribute)attributes.First()).Description : name;
		}
		private static string BuildFlagsValues<T>(T obj, string separator)
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
			return names.Join(separator);
		}
	}
}