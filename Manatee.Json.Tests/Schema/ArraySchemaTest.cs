using System;
using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ArraySchemaTest
	{
		public static IEnumerable TypeData
		{
			get { yield return new TestCaseData(new JsonSchema().Type(JsonSchemaType.Array)); }
		}
		[TestCaseSource(nameof(TypeData))]
		public void ValidateReturnsErrorOnNonArray(JsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(TypeData))]
		public void ValidateReturnsErrorOnString(JsonSchema schema)
		{
			JsonValue json = "string";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		public static IEnumerable MinItemsData
		{
			get { yield return new TestCaseData(new JsonSchema().Type(JsonSchemaType.Array).MinItems(5)); }
		}
		[TestCaseSource(nameof(MinItemsData))]
		public void ValidateReturnsErrorOnTooFewItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(MinItemsData))]
		public void ValidateReturnsValidOnCountEqualsMinItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", null, 4.0, "test"};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MinItemsData))]
		public void ValidateReturnsValidOnCountGreaterThanMinItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", null, 4.0, "test", false};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		public static IEnumerable MaxItemsData
		{
			get { yield return new TestCaseData(new JsonSchema().Type(JsonSchemaType.Array).MaxItems(5)); }
		}
		[TestCaseSource(nameof(MaxItemsData))]
		public void ValidateReturnsErrorOnTooManyItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 2};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(MaxItemsData))]
		public void ValidateReturnsValidOnCountEqualsMaxItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MaxItemsData))]
		public void ValidateReturnsValidOnCountLessThanMaxItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", false};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		public static IEnumerable UniqueItemsData
		{
			get { yield return new TestCaseData(new JsonSchema().Type(JsonSchemaType.Array).UniqueItems(true)); }
		}
		[TestCaseSource(nameof(UniqueItemsData))]
		public void ValidateReturnsErrorOnDuplicateItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 1};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(UniqueItemsData))]
		public void ValidateReturnsValidOnUniqueItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		public static IEnumerable ItemsData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema()
					                              .Type(JsonSchemaType.Array)
					                              .Items(new JsonSchema().Type(JsonSchemaType.String)));
			}
		}
		[TestCaseSource(nameof(ItemsData))]
		public void ValidateReturnsErrorOnInvalidItems(JsonSchema schema)
		{
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(ItemsData))]
		public void ValidateReturnsValidOnValidItems(JsonSchema schema)
		{
			var json = new JsonArray {"start", "string"};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void ValidateReturnsValidOnValidUnevaluatedItem()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Array)
				.AllOf(new JsonSchema()
					       .Item(new JsonSchema().Type(JsonSchemaType.String))
						   .Item(new JsonSchema().Type(JsonSchemaType.Number)))
				.UnevaluatedItems(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = new JsonArray {"value", 5.5, false, true};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidAdditionalItemWithNestedItems()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Array)
				.Item(true)
				.AllOf(new JsonSchema()
					       .Item(new JsonSchema().Type(JsonSchemaType.String))
					       .Item(new JsonSchema().Type(JsonSchemaType.Number)))
				.AdditionalItems(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = new JsonArray {"value", 5.5, false, true};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidUnevaluatedItem()
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Array)
				.AllOf(new JsonSchema()
					       .Item(new JsonSchema().Type(JsonSchemaType.String))
					       .Item(new JsonSchema().Type(JsonSchemaType.Number)))
				.UnevaluatedItems(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = new JsonArray {"value", 5.5, false, "invalid"};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

	}
}
