using System.Collections;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class IntegerSchemaTest
	{
		public static IEnumerable TypeData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer});
			}
		}
		[TestCaseSource(nameof(TypeData))]
		public void ValidateReturnsErrorOnNonNumber(IJsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(TypeData))]
		public void ValidateReturnsErrorOnNonInteger(IJsonSchema schema)
		{
			var json = (JsonValue) 1.2;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		
		public static IEnumerable MinimumData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, Minimum = 5});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, Minimum = 5});
			}
		}
		[TestCaseSource(nameof(MinimumData))]
		public void ValidateReturnsErrorOnLessThanMinimum(IJsonSchema schema)
		{
			var json = (JsonValue) 4;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(MinimumData))]
		public void ValidateReturnsValidOnMoreThanMinimum(IJsonSchema schema)
		{
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		
		[Test]
		public void Draft04_ValidateReturnsErrorOnEqualsExclusiveMinimum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, Minimum = 5, ExclusiveMinimum = true};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void Draft04_ValidateReturnsValidOnMoreThanExclusiveMinimum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, Minimum = 5, ExclusiveMinimum = true};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void Draft06_ValidateReturnsErrorOnEqualsExclusiveMinimum()
		{
			var schema = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, ExclusiveMinimum = 5};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void Draft06_ValidateReturnsValidOnMoreThanExclusiveMinimum()
		{
			var schema = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, ExclusiveMinimum = 5};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertValid();
		}
			
		public static IEnumerable MaximumData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, Maximum = 5});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, Maximum = 5});
			}
		}
		[TestCaseSource(nameof(MaximumData))]
		public void ValidateReturnsErrorOnMoreThanMaximum(IJsonSchema schema)
		{
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(MaximumData))]
		public void ValidateReturnsValidOnLessThanMaximum(IJsonSchema schema)
		{
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		
		[Test]
		public void Draft04_ValidateReturnsErrorOnEqualsExclusiveMaximum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, Maximum = 5, ExclusiveMaximum = true};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void Draft04_ValidateReturnsValidOnLessThanExclusiveMaximum()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, Maximum = 5, ExclusiveMaximum = true};
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void Draft06_ValidateReturnsErrorOnEqualsExclusiveMaximum()
		{
			var schema = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, ExclusiveMaximum = 5};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void Draft06_ValidateReturnsValidOnLessThanExclusiveMaximum()
		{
			var schema = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, ExclusiveMaximum = 5};
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		
		public static IEnumerable MultipleOfData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer, MultipleOf = 5});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaTypeDefinition.Integer, MultipleOf = 5});
			}
		}
		[TestCaseSource(nameof(MultipleOfData))]
		public void ValidateReturnsValidOnMultipleOf_Positive(IJsonSchema schema)
		{
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MultipleOfData))]
		public void ValidateReturnsValidOnMultipleOf_Negative(IJsonSchema schema)
		{
			var json = (JsonValue) (-15);

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MultipleOfData))]
		public void ValidateReturnsValidOnMultipleOf_Zero(IJsonSchema schema)
		{
			var json = (JsonValue) 0;

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[TestCaseSource(nameof(MultipleOfData))]
		public void ValidateReturnsInvalicOnMultipleOf(IJsonSchema schema)
		{
			var json = (JsonValue) 16;

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
	}
}
