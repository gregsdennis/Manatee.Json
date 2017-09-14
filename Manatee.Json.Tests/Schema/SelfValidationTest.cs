using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class SelfValidationTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(JsonSchema04.MetaSchema);
				yield return new TestCaseData(JsonSchema06.MetaSchema);
			}
		}
		
		[TestCaseSource(nameof(TestData))]
		public void Hardcoded(IJsonSchema schema)
		{
			var json = schema.ToJson(null);
			var validation = schema.Validate(json);

			validation.AssertValid();
		}

		[TestCaseSource(nameof(TestData))]
		public void Online(IJsonSchema schema)
		{
			var localSchemaJson = schema.ToJson(null);

			var onlineSchemaText = JsonSchemaOptions.Download(schema.Id);
			var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
			var onlineSchema = JsonSchemaFactory.FromJson(onlineSchemaJson);

			var localValidation = schema.Validate(onlineSchemaJson);
			var onlineValidation = onlineSchema.Validate(localSchemaJson);

			onlineValidation.AssertValid();
			localValidation.AssertValid();

			Assert.AreEqual(onlineSchemaJson, localSchemaJson);
		}
	}
}
