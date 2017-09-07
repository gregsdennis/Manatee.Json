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

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
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

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
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

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}