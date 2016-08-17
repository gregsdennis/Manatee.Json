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
 
	File Name:		JsonSerializableSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		JsonSerializableSerializer
	Purpose:		Converts objects which implement IJsonSerializable to and from
					JsonValues.

***************************************************************************************/

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class JsonSerializableSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => true;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var serializable = (IJsonSerializable) obj;
			return serializable.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = (IJsonSerializable) JsonSerializationAbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			value.FromJson(json, serializer);
			return (T) value;
		}
	}
}