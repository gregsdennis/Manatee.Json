using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ArraySchemaTest
	{
		public static IEnumerable TypeData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaType.Array});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaType.Array});
				yield return new TestCaseData(new JsonSchema07 { Type = JsonSchemaType.Array});
			}
		}
		[TestCaseSource(nameof(TypeData))]
		public void ValidateReturnsErrorOnNonArray(IJsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(TypeData))]
		public void ValidateReturnsErrorOnString(IJsonSchema schema)
		{
			JsonValue json = "string";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		public static IEnumerable MinItemsData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaType.Array, MinItems = 5});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaType.Array, MinItems = 5});
				yield return new TestCaseData(new JsonSchema07 { Type = JsonSchemaType.Array, MinItems = 5});
			}
		}
		[TestCaseSource(nameof(MinItemsData))]
		public void ValidateReturnsErrorOnTooFewItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(MinItemsData))]
		public void ValidateReturnsValidOnCountEqualsMinItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", null, 4.0, "test"};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MinItemsData))]
		public void ValidateReturnsValidOnCountGreaterThanMinItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", null, 4.0, "test", false};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		public static IEnumerable MaxItemsData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaType.Array, MaxItems = 5});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaType.Array, MaxItems = 5});
				yield return new TestCaseData(new JsonSchema07 { Type = JsonSchemaType.Array, MaxItems = 5});
			}
		}
		[TestCaseSource(nameof(MaxItemsData))]
		public void ValidateReturnsErrorOnTooManyItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 2};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(MaxItemsData))]
		public void ValidateReturnsValidOnCountEqualsMaxItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MaxItemsData))]
		public void ValidateReturnsValidOnCountLessThanMaxItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", false};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		public static IEnumerable UniqueItemsData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaType.Array, UniqueItems = true});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaType.Array, UniqueItems = true});
				yield return new TestCaseData(new JsonSchema07 { Type = JsonSchemaType.Array, UniqueItems = true});
			}
		}
		[TestCaseSource(nameof(UniqueItemsData))]
		public void ValidateReturnsErrorOnDuplicateItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 1};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(UniqueItemsData))]
		public void ValidateReturnsValidOnUniqueItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		public static IEnumerable ItemsData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04
					{
						Type = JsonSchemaType.Array,
						Items = new JsonSchema04 {Type = JsonSchemaType.String}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						Type = JsonSchemaType.Array,
						Items = new JsonSchema06 {Type = JsonSchemaType.String}
					});
				yield return new TestCaseData(new JsonSchema07
				{
						Type = JsonSchemaType.Array,
						Items = new JsonSchema07 { Type = JsonSchemaType.String}
					});
			}
		}
		[TestCaseSource(nameof(ItemsData))]
		public void ValidateReturnsErrorOnInvalidItems(IJsonSchema schema)
		{
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(ItemsData))]
		public void ValidateReturnsValidOnValidItems(IJsonSchema schema)
		{
			var json = new JsonArray {"start", "string"};

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}
