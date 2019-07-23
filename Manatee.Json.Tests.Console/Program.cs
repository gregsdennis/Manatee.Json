using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using JsonSchema = Manatee.Json.Schema.JsonSchema;

namespace Manatee.Json.Tests.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			//RunNewtonsoft();
			RunManatee();
		}

		private static void RunManatee()
		{
			var watch = Stopwatch.StartNew();
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Flag;

			var serializer = new JsonSerializer();

			var schemaFile = @"C:\Users\gregs\Downloads\Sample\fhir.schema.json";
			var jsonFile = @"C:\Users\gregs\Downloads\Sample\sample.json";

			var schemaText = File.ReadAllText(schemaFile);
			var schemaJson = JsonValue.Parse(schemaText);
			var schema = serializer.Deserialize<JsonSchema>(schemaJson);

			var jsonText = File.ReadAllText(jsonFile);
			var json = JsonValue.Parse(jsonText);

			var metaResults = schema.ValidateSchema();
			var results = schema.Validate(json);

			File.WriteAllText(@"C:\Users\gregs\Downloads\Sample\metaResults.json", serializer.Serialize(metaResults).GetIndentedString());
			File.WriteAllText(@"C:\Users\gregs\Downloads\Sample\results.json", serializer.Serialize(results).GetIndentedString());
			System.Console.WriteLine(watch.Elapsed);
		}

		private static void RunNewtonsoft()
		{
			var schemaFile = @"C:\Users\gregs\Downloads\Sample\fhir.schema.json";
			var jsonFile = @"C:\Users\gregs\Downloads\Sample\sample.json";

			var schemaText = File.ReadAllText(schemaFile);
			var schema = JSchema.Parse(schemaText);

			var jsonText = File.ReadAllText(jsonFile);
			var json = JObject.Parse(jsonText);

			var isValid = json.IsValid(schema, out IList<string> errors);

			File.WriteAllText(@"C:\Users\gregs\Downloads\Sample\newtonsoft.results.json", string.Join("\n", errors));
		}
	}
}
