using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using Manatee.Json.Tests.Schema.TestSuite;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class DevTest
	{
		[Test]
		// TOOD: Add categories to exclude this test.
		[Ignore("This test for development purposes only.")]
		public void Test1()
		{
			JsonSchemaTestSuite.Setup();
			
			var text = "{\"$id\":\"http://localhost:1234/scope_change_defs2.json\",\"type\":\"object\",\"properties\":{\"list\":{\"$ref\":\"#/definitions/baz/definitions/bar\"}},\"definitions\":{\"baz\":{\"$id\":\"folder/\",\"definitions\":{\"bar\":{\"type\":\"array\",\"items\":{\"$ref\":\"folderInteger.json\"}}}}}}";
			var json = JsonValue.Parse(text);
			var schema = JsonSchemaFactory.FromJson(json);

			var array = new JsonObject {["list"] = new JsonArray {1}};

			var results = schema.Validate(array);

			Console.WriteLine(schema.ToJson(null));
		}
	}
}