using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class DevTest
	{
		[Test]
		// TOOD: Add categories to exclude this test.
		//[Ignore("This test for development purposes only.")]
		public void Test1()
		{
			var text = "{\"$id\":\"http://localhost:1234/\",\"items\":{\"$id\":\"folder/\",\"items\":{\"$ref\":\"folderInteger.json\"}}}";
			var json = JsonValue.Parse(text);
			var schema = JsonSchemaFactory.FromJson(json);

			var array = new JsonArray {new JsonArray {1}};

			var results = schema.Validate(array);

			Console.WriteLine(schema.ToJson(null));
		}
	}
}