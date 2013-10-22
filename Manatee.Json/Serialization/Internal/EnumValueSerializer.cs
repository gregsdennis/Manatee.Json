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
 
	File Name:		EnumValueSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		EnumValueSerializer
	Purpose:		Converts enumerations to and from JsonValues by integral value.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization.Internal
{
	internal class EnumValueSerializer : ISerializer
	{
		public bool ShouldMaintainReferences { get { return false; } }

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var value = Convert.ToInt32(obj);
			return value;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = (int) json.Number;
			return (T) Enum.ToObject(typeof (T), value);
		}
	}
}