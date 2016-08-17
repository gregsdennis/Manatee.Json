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
 
	File Name:		SchemaTest.cs
	Namespace:		Manatee.Json.Tests.Schema.TestSuite
	Class Name:		SchemaTest
	Purpose:		Models one of the schema tests defined at
					https://github.com/json-schema-org/JSON-Schema-Test-Suite.

***************************************************************************************/
using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTest : IJsonSerializable
	{
		public string Description { get; set; }
		public JsonValue Data { get; set; }
		public bool Valid { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Description = obj.TryGetString("description");
			Data = obj["data"];
			Valid = obj["valid"].Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}