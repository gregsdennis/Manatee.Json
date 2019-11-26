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
			SchemaValidationResults.IncludeAdditionalInfo = true;

			var schema = new JsonSchema()
				.UnevaluatedProperties(false)
				.AllOf(new JsonSchema()
					       .Property("foo", new JsonSchema().Type(JsonSchemaType.String | JsonSchemaType.Null))
					       .Property("bar", new JsonSchema().Type(JsonSchemaType.String | JsonSchemaType.Null)),
				       new JsonSchema()
					       .AdditionalProperties(new JsonSchema().Not(new JsonSchema().Enum(JsonValue.Null))));
			var json = new JsonObject
				{
					["bar"] = "foo",
					["bob"] = "who?"
				};

			var results = schema.Validate(json);

			Console.WriteLine(json);

			results.AssertValid();
		}
	}
}