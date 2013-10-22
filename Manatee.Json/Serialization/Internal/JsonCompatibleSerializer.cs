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
 
	File Name:		JsonCompatibleSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		JsonCompatibleSerializer
	Purpose:		Converts objects which implement IJsonCompatible to and from
					JsonValues.

***************************************************************************************/

namespace Manatee.Json.Serialization.Internal
{
	internal class JsonCompatibleSerializer : ISerializer
	{
		public bool ShouldMaintainReferences { get { return true; } }

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var compatible = (IJsonCompatible) obj;
			return compatible.ToJson();
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = (IJsonCompatible) JsonSerializationAbstractionMap.CreateInstance<T>(json);
			value.FromJson(json);
			return (T) value;
		}
	}
}