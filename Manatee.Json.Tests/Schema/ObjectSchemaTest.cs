using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ObjectSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonObject()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.Object);
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnRequiredPropertyMissing()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String))
				.Required("test1");
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnOptionalPropertyMissing()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String));
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String));
			var json = new JsonObject {{"test1", 1}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnAllValidProperties()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String));
			var json = new JsonObject {{"test1", "value"}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidPatternProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String))
				.AdditionalProperties(false)
				.PatternProperty("[0-9]", new JsonSchema().Type(JsonSchemaType.String));
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnUnmatchedPatternProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String))
				.AdditionalProperties(false)
				.PatternProperty("[0-9]", new JsonSchema().Type(JsonSchemaType.String));
			var json = new JsonObject {{"test1", "value"}, {"test", "value"}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidAdditionalProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String))
				.AdditionalProperties(false);
			var json = new JsonObject {{"test1", "value"}, {"test", 1}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidAdditionalProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test1", new JsonSchema().Type(JsonSchemaType.String))
				.AdditionalProperties(new JsonSchema().Type(JsonSchemaType.String));
			var json = new JsonObject {{"test1", "value"}, {"test", "value"}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnValidUnevaluatedProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.AllOf(new JsonSchema()
					       .Property("test1", new JsonSchema().Type(JsonSchemaType.String)),
				       new JsonSchema()
					       .Property("test2", new JsonSchema().Type(JsonSchemaType.Number)))
				.UnevaluatedProperties(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = new JsonObject {{"test1", "value"}, {"test2", 5.5}, {"test", false}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidAdditionalPropertyWithNestedProperties()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.AllOf(new JsonSchema()
					       .Property("test1", new JsonSchema().Type(JsonSchemaType.String)),
				       new JsonSchema()
					       .Property("test2", new JsonSchema().Type(JsonSchemaType.Number)))
				.AdditionalProperties(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = new JsonObject {{"test1", "value"}, {"test2", 5.5}, {"test", false}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidUnevaluatedProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.AllOf(new JsonSchema()
					       .Property("test1", new JsonSchema().Type(JsonSchemaType.String)),
				       new JsonSchema()
					       .Property("test2", new JsonSchema().Type(JsonSchemaType.Number)))
				.UnevaluatedProperties(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = new JsonObject {{"test1", "value"}, {"test2", 5.5}, {"test", "invalid"}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidPatternProperty()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("test", new JsonSchema().Type(JsonSchemaType.String))
				.AdditionalProperties(false)
				.PatternProperty("[0-9]", new JsonSchema().Type(JsonSchemaType.Integer));
			var json = new JsonObject {{"test", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnNotTooManyProperties()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.MaxProperties(5);
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsInvalidOnTooManyProperties()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.MaxProperties(1);
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(false, results.IsValid);
		}
		[Test]
		public void ValidateReturnsValidOnNotTooFewProperties()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.MinProperties(1);
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsInvalidOnTooFewProperties()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.MinProperties(5);
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(false, results.IsValid);
		}
	}
}
