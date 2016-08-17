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
 
	File Name:		SchemaTestSet.cs
	Namespace:		Manatee.Json.Tests.Schema.TestSuite
	Class Name:		SchemaTestSet
	Purpose:		Models a set of the schema tests defined at
					https://github.com/json-schema-org/JSON-Schema-Test-Suite.

***************************************************************************************/
using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTestSet : IJsonSerializable
	{
		public string Description { get; set; }
		public IJsonSchema Schema { get; set; }
		public IEnumerable<SchemaTest> Tests { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Description = obj["description"].String;
			Schema = serializer.Deserialize<IJsonSchema>(obj["schema"]);
			Tests = serializer.Deserialize<List<SchemaTest>>(obj["tests"]);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}