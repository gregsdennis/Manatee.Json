﻿using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class NumberSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonNumber()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number};
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnLessThanMinimum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5};
			var json = (JsonValue) 4;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnMoreThanMinimum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnEqualsExclusiveMinimum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5, ExclusiveMinimum = true};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnMoreThanExclusiveMinimum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Minimum = 5, ExclusiveMinimum = true};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnMoreThanMaximum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Maximum = 5};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnLessThanMaximum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Maximum = 5};
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnEqualsExclusiveMaximum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Maximum = 5, ExclusiveMaximum = true};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnLessThanExclusiveMaximum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Maximum = 5, ExclusiveMaximum = true};
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnMultipleOf_Positive()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) 7.5;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnMultipleOf_Negative()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) (-7.5);

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnMultipleOf_Zero()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) 0;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsInvalicOnMultipleOf()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) 16;

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
	}
}
