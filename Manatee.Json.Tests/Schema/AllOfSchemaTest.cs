using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class AllOfSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnAnyInvalid()
		{
			var schema = new JsonSchema04
				{
					AllOf = new List<IJsonSchema>
						{
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array},
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number}
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnAllValid()
		{
			var schema = new JsonSchema04
			{
					AllOf = new List<IJsonSchema>
						{
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number,Minimum = 10},
							new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number,Maximum = 20}
						}
				};
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}