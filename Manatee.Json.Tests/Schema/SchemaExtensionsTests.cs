using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class SchemaExtensionsTests
	{
		private class QuasiSchema7 : JsonSchema06
		{
			public static QuasiSchema7 MetaQuasi = new QuasiSchema7
				{
					Id = "http://schema.org/quasi7",
					Schema = "http://schema.org/quasi7",
					AllOf = new List<IJsonSchema>{new JsonSchemaReference(MetaSchema.Id, typeof(JsonSchema06))},
					Properties = new Dictionary<string, IJsonSchema>
						{
							["if"] = True,
							["then"] = True,
							["else"] = True
						}
				};

			public IJsonSchema If { get; set; }
			public IJsonSchema Then { get; set; }
			public IJsonSchema Else { get; set; }
		}

		private class QuasiSchema7IfThenElseValidator : IJsonSchemaPropertyValidator
		{
			public bool Applies(IJsonSchema schema, JsonValue json)
			{
				return schema is QuasiSchema7 typed && typed.If != null;
			}
			public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
			{
				var typed = (QuasiSchema7)schema;
				if (typed.If == null) return new SchemaValidationResults();

				var ifResults = _ValidateSubSchema(typed.If, json, root);
				if (ifResults.Valid)
				{
					var thenResults = _ValidateSubSchema(typed.Then, json, root);
					if (thenResults.Valid) return new SchemaValidationResults();

					return new SchemaValidationResults("then", "Validation of `if` succeeded, but validation of `then` failed.");
				}

				var elseResults = _ValidateSubSchema(typed.Else, json, root);
				if (elseResults.Valid) return new SchemaValidationResults();

				return new SchemaValidationResults("else", "Validation of `if` failed, but validation of `else` also failed.");
			}

			private static SchemaValidationResults _ValidateSubSchema(IJsonSchema schema, JsonValue json, JsonValue root)
			{
				return schema == null
					       ? new SchemaValidationResults()
					       : schema.Validate(json, root);
			}
		}

		[Test]
		public void QuasiSchema7IfTrueThenTrue()
		{
			JsonSchemaFactory.RegisterExtendedSchema(QuasiSchema7.MetaQuasi.Id, () => new QuasiSchema7(), QuasiSchema7.MetaQuasi);
			JsonSchemaPropertyValidatorFactory.RegisterValidator(new QuasiSchema7IfThenElseValidator());

			var schema = new QuasiSchema7
				{
					Schema = QuasiSchema7.MetaQuasi.Id,
					If = new QuasiSchema7 {Type = JsonSchemaType.Integer},
					Then = new QuasiSchema7 {Minimum = 10},
					Else = JsonSchema06.False
				};

			JsonValue json = 15;

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void QuasiSchema7IfTrueThenFalse()
		{
			JsonSchemaFactory.RegisterExtendedSchema(QuasiSchema7.MetaQuasi.Id, () => new QuasiSchema7(), QuasiSchema7.MetaQuasi);
			JsonSchemaPropertyValidatorFactory.RegisterValidator(new QuasiSchema7IfThenElseValidator());

			var schema = new QuasiSchema7
				{
					Schema = QuasiSchema7.MetaQuasi.Id,
					If = new QuasiSchema7 {Type = JsonSchemaType.Integer},
					Then = new QuasiSchema7 {Minimum = 10},
					Else = JsonSchema06.False
				};

			JsonValue json = 5;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void QuasiSchema7IfFalseElseTrue()
		{
			JsonSchemaFactory.RegisterExtendedSchema(QuasiSchema7.MetaQuasi.Id, () => new QuasiSchema7(), QuasiSchema7.MetaQuasi);
			JsonSchemaPropertyValidatorFactory.RegisterValidator(new QuasiSchema7IfThenElseValidator());

			var schema = new QuasiSchema7
				{
					Schema = QuasiSchema7.MetaQuasi.Id,
					If = new QuasiSchema7 {Type = JsonSchemaType.Integer},
					Then = JsonSchema06.False,
					Else = new QuasiSchema7 {Type = JsonSchemaType.String}
				};

			JsonValue json = "hello";

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void QuasiSchema7IfFalseElseFalse()
		{
			JsonSchemaFactory.RegisterExtendedSchema(QuasiSchema7.MetaQuasi.Id, () => new QuasiSchema7(), QuasiSchema7.MetaQuasi);
			JsonSchemaPropertyValidatorFactory.RegisterValidator(new QuasiSchema7IfThenElseValidator());

			var schema = new QuasiSchema7
				{
					Schema = QuasiSchema7.MetaQuasi.Id,
					If = new QuasiSchema7 {Type = JsonSchemaType.Integer},
					Then = JsonSchema06.False,
					Else = new QuasiSchema7 {Type = JsonSchemaType.String}
				};

			JsonValue json = false;

			var results = schema.Validate(json);

			results.AssertInvalid();

			json = QuasiSchema7.MetaQuasi.ToJson(new JsonSerializer());

			Console.WriteLine(json.GetIndentedString());
		}
	}
}