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
 
	File Name:		DefaultValueSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		DefaultValueSerializer
	Purpose:		Shortcuts serialization and returns JsonValue.Null if the
					object equals the default value for its type.

***************************************************************************************/

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DefaultValueSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences { get { return _innerSerializer.ShouldMaintainReferences; } }

		public DefaultValueSerializer(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			if (Equals(obj, default(T)) && !serializer.Options.EncodeDefaultValues) return JsonValue.Null;
			return _innerSerializer.Serialize(obj, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			if (json == JsonValue.Null) return default(T);
			return _innerSerializer.Deserialize<T>(json, serializer);
		}
	}
}