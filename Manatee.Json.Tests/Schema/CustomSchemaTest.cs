using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class CustomSchemaTest
	{
		private class MySchema04 : JsonSchema04
		{
			public JsonValue Equals { get; set; }
		}

		private class MySchemaEqualsValidator : IJsonSchemaPropertyValidator
		{
			public bool Applies(IJsonSchema schema, JsonValue json)
			{
				return schema is MySchema04 typed && json.Type == JsonValueType.Number;
			}
			public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
			{
				var typed = (MySchema04) schema;
				return typed.Equals == json
						   ? new SchemaValidationResults()
						   : new SchemaValidationResults(null, "The values aren't equal.");
			}
		}

		[Test]
		public void MySchema04EqualsValid()
		{
			JsonSchemaPropertyValidatorFactory.RegisterValidator(new MySchemaEqualsValidator());

			JsonValue json = 4;
			var target = new MySchema04 {Equals = 4};

			var results = target.Validate(json);

			Assert.IsTrue(results.Valid);
		}

		[Test]
		public void MySchema04EqualsInvalid()
		{
			JsonSchemaPropertyValidatorFactory.RegisterValidator(new MySchemaEqualsValidator());

			JsonValue json = 4;
			var target = new MySchema04 {Equals = 5};

			var results = target.Validate(json);

			Assert.IsFalse(results.Valid);
			Assert.AreEqual(results.Errors.First().Message, "The values aren't equal.");
		}
	}
}
