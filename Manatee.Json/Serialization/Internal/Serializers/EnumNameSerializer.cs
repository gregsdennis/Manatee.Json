/***************************************************************************************

	Copyright 2012 Greg Dennis

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

		public bool ShouldMaintainReferences { get { return false; } }

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			EnsureDescriptions<T>();
			var attributes = (typeof (T)).GetCustomAttributes(typeof (FlagsAttribute), false);
			return !attributes.Any()
				? _descriptions[typeof (T)].First(d => Equals(d.Value, obj)).String
				: BuildFlagsValues(obj, serializer.Options.FlagsEnumSeparator);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			EnsureDescriptions<T>();
			var options = serializer.Options.CaseSensitiveDeserialization
							  ? StringComparison.InvariantCultureIgnoreCase
							  : StringComparison.InvariantCulture;
			var value = _descriptions[typeof(T)].First(d => string.Equals(d.String, json.String, options)).Value;
			return (T) value;
		}

		private static void EnsureDescriptions<T>()
		{
			var type = typeof (T);
			if (_descriptions.ContainsKey(type)) return;
			var names = Enum.GetValues(type).Cast<T>();
			var descriptions = names.Select(n => new Description {Value = n, String = GetDescription<T>(n.ToString())}).ToList();
			_descriptions.Add(type, descriptions);
		}
		private static string GetDescription<T>(string name)
		{
			var type = typeof (T);
			var memInfo = type.GetMember(name);
			var attributes = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
			return attributes.Any() ? ((DescriptionAttribute)attributes[0]).Description : name;
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