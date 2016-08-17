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
 
	File Name:		DictionarySerializationDelegateProvider.cs
	Namespace:		Manatee.Json.Serialization.Internal.AutoRegistration
	Class Name:		DictionarySerializationDelegateProvider
	Purpose:		Provides delegates for serializing Dictionary types.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class DictionarySerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
		}

		private static JsonValue Encode<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(dict.Select(item => (JsonValue)(new JsonObject
				{
					{"Key", serializer.Serialize(item.Key)},
					{"Value", serializer.Serialize(item.Value)}
				})));
			return array;
		}
		private static Dictionary<TKey, TValue> Decode<TKey, TValue>(JsonValue json, JsonSerializer serializer)
		{
			return json.Array.ToDictionary(jv => serializer.Deserialize<TKey>(jv.Object["Key"]),
										   jv => serializer.Deserialize<TValue>(jv.Object["Value"]));
		}
	}
}