using System;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ArraySchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonArray()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsErrorOnString()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array};
			JsonValue json = "string";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsErrorOnTooFewItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, MinItems = 5};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnCountEqualsMinItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, MinItems = 2};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnCountGreaterThanMinItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, MinItems = 2};
			var json = new JsonArray {1, "string", false};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[Test]
		public void ValidateReturnsErrorOnTooManyItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, MaxItems = 5};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 2};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnCountEqualsMaxItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, MaxItems = 5};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnCountLessThanMaxItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, MaxItems = 5};
			var json = new JsonArray {1, "string", false};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[Test]
		public void ValidateReturnsErrorOnDuplicateItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, UniqueItems = true};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 1};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnUniqueItems()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array, UniqueItems = true};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidItems()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaTypeDefinition.Array,
					Items = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
				};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnValidItems()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaTypeDefinition.Array,
					Items = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
				};
			var json = new JsonArray {"start", "string"};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
