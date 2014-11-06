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
 
	File Name:		SchemaSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		SchemaSerializer
	Purpose:		Converts ISchema implementations to and from JsonValues.

***************************************************************************************/

using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : ISerializer
	{
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var schema = (IJsonSchema) obj;
			return schema.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = JsonSchemaFactory.FromJson(json);
			return (T)value;
		}
		public bool ShouldMaintainReferences { get { return false; } }
	}
}