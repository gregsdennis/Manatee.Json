using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test()
		{
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Flag;
			SchemaValidationResults.IncludeAdditionalInfo = true;

			var serializer = new JsonSerializer();

			var schemaText = File.ReadAllText("C:\\Users\\gregs\\Desktop\\Manatee.Json.Schema.OpenApi\\Reference\\schema-2.0.json");
			var schemaJson = JsonValue.Parse(schemaText);
			var schema = serializer.Deserialize<JsonSchema>(schemaJson);

			var address = new Uri("https://raw.githubusercontent.com/warehouseman/trello-swagger-generator/master/TrelloAPI.json");
			var content = new WebClient().DownloadString(address);
			var json = JsonValue.Parse(content);

			var results = schema.Validate(json);

			Console.WriteLine(json);

			results.AssertValid();
		}
	}
}