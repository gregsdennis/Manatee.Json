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
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		EnumNameSerializer
	Purpose:		Converts enumerations to and from JsonValues by name.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Manatee.Json.Serialization.Internal
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
			return _descriptions[typeof (T)].First(d => Equals(d.Value, obj)).String;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			EnsureDescriptions<T>();
			var value = _descriptions[typeof(T)].First(d => Equals(d.String, json.String)).Value;
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

	}
}