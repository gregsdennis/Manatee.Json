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
 
	File Name:		RegisteredObjectSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		RegisteredObjectSerializer
	Purpose:		Converts an object whose type has registered serialize methods
					to and from JsonValues.

***************************************************************************************/

namespace Manatee.Json.Serialization.Internal
{
	internal class RegisteredObjectSerializer : ISerializer
	{
		public bool ShouldMaintainReferences { get { return true; } }

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			JsonValue json;
			serializer.Encode(obj, out json);
			return json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			T value;
			serializer.Decode(json, out value);
			return value;
		}
	}
}