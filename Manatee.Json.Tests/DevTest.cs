using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	//[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test()
		{
			//JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Flag;

			var serializer = new JsonSerializer();

			var schemaFile = @"C:\Users\gregs\Downloads\Sample\fhir.schema.json";
			var jsonFile = @"C:\Users\gregs\Downloads\Sample\sample.json";

			var schemaText = File.ReadAllText(schemaFile);
			var schemaJson = JsonValue.Parse(schemaText);
			var schema = serializer.Deserialize<JsonSchema>(schemaJson);

			var jsonText = File.ReadAllText(jsonFile);
			var json = JsonValue.Parse(jsonText);

			//var metaResults = schema.ValidateSchema();
			var results = schema.Validate(json);

			//File.WriteAllText(@"C:\Users\gregs\Downloads\Sample\metaResults.json", serializer.Serialize(metaResults).GetIndentedString());
			File.WriteAllText(@"C:\Users\gregs\Downloads\Sample\results.json", serializer.Serialize(results).GetIndentedString());
		}
	}
}