using System;
using System.Collections;
using System.Linq;
using System.Net;
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
				yield return new TestCaseData(MetaSchemas.Draft04);
				yield return new TestCaseData(MetaSchemas.Draft06);
				yield return new TestCaseData(MetaSchemas.Draft07);
				yield return new TestCaseData(MetaSchemas.Draft08);
			}
		}
		
		[TestCaseSource(nameof(TestData))]
		public void Hardcoded(JsonSchema schema)
		{
			var json = schema.ToJson(null);
			var validation = schema.Validate(json);

			validation.AssertValid();
		}

		[TestCaseSource(nameof(TestData))]
		public void Online(JsonSchema schema)
		{
			try
			{
				// TODO: Catch web exceptions and assert inconclusive.
				var localSchemaJson = schema.ToJson(null);

				var onlineSchemaText = JsonSchemaOptions.Download(schema.Id);
				var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
				var onlineSchema = new JsonSchema();
				onlineSchema.FromJson(onlineSchemaJson, null);

				var localValidation = schema.Validate(onlineSchemaJson);
				var onlineValidation = onlineSchema.Validate(localSchemaJson);

				Assert.AreEqual(onlineSchema, schema);

				onlineValidation.AssertValid();
				localValidation.AssertValid();

				Assert.AreEqual(onlineSchemaJson, localSchemaJson);

			}
			catch (WebException)
			{
				Assert.Inconclusive();
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.OfType<WebException>().Any())
					Assert.Inconclusive();
				throw;
			}
		}
	}
}
