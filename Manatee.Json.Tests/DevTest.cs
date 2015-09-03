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
 
	File Name:		DevTest.cs
	Namespace:		Manatee.Tests
	Class Name:		DevTest
	Purpose:		Provides a single method through which one can test any specific
					scenario.  Should be marked [Ignore] when committing to the
					repository.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manatee.Json.Path;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Transform;
using Manatee.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class DevTest
	{
		[TestMethod]
		public void Test1()
		{
			JsonValue source = new JsonObject
				{
					{"a", new JsonObject {{"example", new JsonObject {{"demo", "baz"}}}}},
					{"b", new JsonObject {{"example", new JsonObject {{"demo", "qux"}}}}},
				};
			JsonValue template = new JsonObject
				{
					{
						"foo", new JsonArray {"$..example", new JsonObject {{"bar", "$.demo"}}}
					}
				};
			JsonValue expected = new JsonObject
				{
					{
						"foo", new JsonArray
							{
								new JsonObject {{"bar", "baz"}},
								new JsonObject {{"bar", "qux"}}
							}
					}
				};
			var result = source.Transform(template);

			Console.WriteLine(expected);
			Console.WriteLine(result);
			Assert.AreEqual(expected, result);
		}
		[TestMethod]
		public void Test2()
		{
			var json = JsonSchema.Draft04.ToJson(null);
			var validation = JsonSchema.Draft04.Validate(json);
			Console.WriteLine(validation.Valid);
		}

		[TestMethod]
		public void SchemaGenerationTest()
		{
			var schema = JsonSchemaFactory.FromType<List<Dictionary<JsonValueType, JsonValue>>>();
			Console.WriteLine(schema.ToJson(null).GetIndentedString());
		}
	}
}