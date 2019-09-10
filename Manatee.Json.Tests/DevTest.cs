using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Verbose;

			var serializer = new JsonSerializer();

			var schemaFile = @"C:\Users\gregs\Downloads\dozor-schema.json";

			var schemaText = File.ReadAllText(schemaFile);
			var schemaJson = JsonValue.Parse(schemaText);
			var schema = serializer.Deserialize<JsonSchema>(schemaJson);

			JsonValue json = new JsonObject
				{
					["events"] = new JsonArray
						{
							new JsonObject
								{
									["data"] = new JsonObject {["screen_name"] = "example screen 4"},
									["event_type"] = "screen_view"
								},
							new JsonObject
								{
									["data"] = new JsonObject
										{
											["custom_attributes"] = new JsonObject {["foo"] = "literally anything"},
											["custom_event_type"] = "navigation",
											["event_name"] = "example custom 40"
										},
									["event_type"] = "custom_event"
								}
						},
					["user_attributes"] = new JsonObject
							{
								["foo-string6"] = "a string",
								["foo-boolean4"] = false,
								["custom_property"] = new JsonArray {5, false, "string"}
							}
						["user_identities"] = new JsonObject
						{
							["customer_id"] = "name",
							["email"] = "email",
							["other"] = Math.PI,
							["other_property"] = null
						}
				};

			var watch = Stopwatch.StartNew();
			var results = schema.Validate(json);
			watch.Stop();

			Console.WriteLine(json);
			Console.WriteLine(watch.ElapsedMilliseconds);

			results.AssertValid();
		}
	}
}