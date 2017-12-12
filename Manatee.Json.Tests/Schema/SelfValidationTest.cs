using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
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
				yield return new TestCaseData(JsonSchema07.MetaSchema);
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
			try
			{
				// TODO: Catch web exceptions and assert inconclusive.
				var localSchemaJson = schema.ToJson(null);

				var onlineSchemaText = JsonSchemaOptions.Download(schema.Id);
				var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
				var onlineSchema = JsonSchemaFactory.FromJson(onlineSchemaJson);

				var localValidation = schema.Validate(onlineSchemaJson);
				var onlineValidation = onlineSchema.Validate(localSchemaJson);

				Assert.AreEqual(onlineSchema, schema);
			
				onlineValidation.AssertValid();
				localValidation.AssertValid();

				Assert.AreEqual(onlineSchemaJson, localSchemaJson);

			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.OfType<HttpRequestException>().Any())
					Assert.Inconclusive();
				throw;
			}
		}
	}
}
