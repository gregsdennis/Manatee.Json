using System;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class ArraySchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonArray()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnTooFewItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, MinItems = 5};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountEqualsMinItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, MinItems = 2};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountGreaterThanMinItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, MinItems = 2};
			var json = new JsonArray {1, "string", false};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnTooManyItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, MaxItems = 5};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 2};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountEqualsMaxItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, MaxItems = 5};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountLessThanMaxItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, MaxItems = 5};
			var json = new JsonArray {1, "string", false};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnDuplicateItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, UniqueItems = true};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 1};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnUniqueItems()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array, UniqueItems = true};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidItems()
		{
			var schema = new JsonSchema
				{
					Type = JsonSchemaTypeDefinition.Array,
					Items = new JsonSchema {Type = JsonSchemaTypeDefinition.String}
				};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidItems()
		{
			var schema = new JsonSchema
				{
					Type = JsonSchemaTypeDefinition.Array,
					Items = new JsonSchema {Type = JsonSchemaTypeDefinition.String}
				};
			var json = new JsonArray {"start", "string"};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
