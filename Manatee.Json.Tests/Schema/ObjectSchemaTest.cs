﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Object};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnRequiredPropertyMissing()
		{
			var schema = new JsonSchema04
				{
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String},
									IsRequired = true
								}
						}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, IJsonSchema>
						{
							{new Regex("[0-9]"), new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, IJsonSchema>
						{
							{new Regex("[0-9]"), new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
						},
					AdditionalProperties =
						new AdditionalProperties {Definition = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
						},
					AdditionalProperties =
						new AdditionalProperties {Definition = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}}
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
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["test1"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, IJsonSchema>
						{
							{new Regex("[0-9]"), new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer}}
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
				Type = JsonSchemaTypeDefinition.Object,
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
				Type = JsonSchemaTypeDefinition.Object,
				MaxProperties = 1
			};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnNotTooFewProperties()
		{
			var schema = new JsonSchema04
			{
				Type = JsonSchemaTypeDefinition.Object,
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
				Type = JsonSchemaTypeDefinition.Object,
				MinProperties = 5
			};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
	}
}
