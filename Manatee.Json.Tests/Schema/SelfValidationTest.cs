using System;
using System.Collections;
using System.Linq;
using System.Net;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class SelfValidationTest
	{
		private static readonly JsonSerializer _serializer = new JsonSerializer();

		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(MetaSchemas.Draft04) {TestName = nameof(MetaSchemas.Draft04)};
				yield return new TestCaseData(MetaSchemas.Draft06) {TestName = nameof(MetaSchemas.Draft06)};
				yield return new TestCaseData(MetaSchemas.Draft07) {TestName = nameof(MetaSchemas.Draft07)};
				//yield return new TestCaseData(MetaSchemas.Draft2019_04) {TestName = nameof(MetaSchemas.Draft2019_04)};
			}
		}
		
		[TestCaseSource(nameof(TestData))]
		public void Hardcoded(JsonSchema schema)
		{
			var json = schema.ToJson(_serializer);
			var validation = schema.Validate(json);

			validation.AssertValid();
		}

		[TestCaseSource(nameof(TestData))]
		public void Online(JsonSchema schema)
		{
			try
			{
				var localSchemaJson = schema.ToJson(_serializer);

				var onlineSchemaText = JsonSchemaOptions.Download(schema.Id);
				var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
				var onlineSchema = new JsonSchema();
				onlineSchema.FromJson(onlineSchemaJson, _serializer);

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

		[TestCaseSource(nameof(TestData))]
		public void RoundTrip(JsonSchema schema)
		{
			var json = _serializer.Serialize(schema);
			Console.WriteLine(json.GetIndentedString());
			var returnTrip = _serializer.Deserialize<JsonSchema>(json);

			Assert.AreEqual(schema, returnTrip);
		}
	}
}
