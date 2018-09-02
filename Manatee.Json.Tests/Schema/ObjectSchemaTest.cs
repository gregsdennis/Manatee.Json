using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	// TODO: Convert this to test all of the schema generations
	[TestFixture]
	public class ObjectSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonObject()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.Object};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnRequiredPropertyMissing()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						},
					Required = new List<string> {"test1"}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnOptionalPropertyMissing()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidProperty()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						}
				};
			var json = new JsonObject {{"test1", 1}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnAllValidProperties()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						}
				};
			var json = new JsonObject {{"test1", "value"}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidPatternProperty()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, JsonSchema>
						{
							{new Regex("[0-9]"), new JsonSchema04 {Type = JsonSchemaType.String}}
						}
				};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnUnmatchedPatternProperty()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, JsonSchema>
						{
							{new Regex("[0-9]"), new JsonSchema04 {Type = JsonSchemaType.String}}
						}
				};
			var json = new JsonObject {{"test1", "value"}, {"test", "value"}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidAdditionalProperty()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						},
					AdditionalProperties =
						new AdditionalProperties {Definition = new JsonSchema04 {Type = JsonSchemaType.String}}
				};
			var json = new JsonObject {{"test1", "value"}, {"test", 1}};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidAdditionalProperty()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						},
					AdditionalProperties =
						new AdditionalProperties {Definition = new JsonSchema04 {Type = JsonSchemaType.String}}
				};
			var json = new JsonObject {{"test1", "value"}, {"test", "value"}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnValidPatternProperty()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, JsonSchema>
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaType.String}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, JsonSchema>
						{
							{new Regex("[0-9]"), new JsonSchema04 {Type = JsonSchemaType.Integer}}
						}
				};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnNotTooManyProperties()
		{
			var schema = new JsonSchema04
			{
				Type = JsonSchemaType.Object,
				MaxProperties = 5
			};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsInvalidOnTooManyProperties()
		{
			var schema = new JsonSchema04
			{
				Type = JsonSchemaType.Object,
				MaxProperties = 1
			};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.IsValid);
		}
		[Test]
		public void ValidateReturnsValidOnNotTooFewProperties()
		{
			var schema = new JsonSchema04
			{
				Type = JsonSchemaType.Object,
				MinProperties = 1
			};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsInvalidOnTooFewProperties()
		{
			var schema = new JsonSchema04
			{
				Type = JsonSchemaType.Object,
				MinProperties = 5
			};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.IsValid);
		}
	}
}
