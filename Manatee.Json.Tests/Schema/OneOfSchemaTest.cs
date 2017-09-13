using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class OneOfSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNoneValid()
		{
			var schema = new JsonSchema04
				{
					OneOf = new List<IJsonSchema>
						{
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5},
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 10}
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnMoreThanOneValid()
		{
			var schema = new JsonSchema04
				{
					OneOf = new List<IJsonSchema>
						{
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5},
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 10}
						}
				};
			var json = (JsonValue) 20;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnSingleValid()
		{
			var schema = new JsonSchema04
				{
					OneOf = new List<IJsonSchema>
						{
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5},
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 10}
						}
				};
			var json = (JsonValue) 7;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}