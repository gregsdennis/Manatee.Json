using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class SelfValidationTest
	{
		private static readonly JsonSerializer _serializer = new JsonSerializer();

		public static IEnumerable<TestCaseData> TestData =>
			new[]
				{
					new TestCaseData(nameof(MetaSchemas.Draft04), MetaSchemas.Draft04),
					new TestCaseData(nameof(MetaSchemas.Draft06), MetaSchemas.Draft06),
					new TestCaseData(nameof(MetaSchemas.Draft07), MetaSchemas.Draft07),
					new TestCaseData(nameof(MetaSchemas.Draft2019_09), MetaSchemas.Draft2019_09)
				};

		[TestCaseSource(nameof(TestData))]
		public void Hardcoded(string testName, JsonSchema schema)
		{
			var json = schema.ToJson(_serializer);
			var validation = schema.Validate(json);

			validation.AssertValid();
		}

		[TestCaseSource(nameof(TestData))]
		public void Online(string testName, JsonSchema schema)
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

				try
				{
					Console.WriteLine("Asserting schema equality");
					Assert.AreEqual(onlineSchema, schema);

					Console.WriteLine("Validating local against online");
					onlineValidation.AssertValid();
					Console.WriteLine("Validating online against local");
					localValidation.AssertValid();

					Console.WriteLine("Asserting json equality");
					Assert.AreEqual(onlineSchemaJson, localSchemaJson);
				}
				catch (Exception)
				{
					Console.WriteLine("Online {0}", onlineSchemaJson);
					Console.WriteLine("Local {0}", localSchemaJson);
					throw;
				}
			}
			catch (WebException)
			{
				Assert.Inconclusive();
			}
			catch (SocketException)
			{
				Assert.Inconclusive();
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.OfType<WebException>().Any() || e.InnerExceptions.OfType<HttpRequestException>().Any())
					Assert.Inconclusive();
				throw;
			}
		}

		[TestCaseSource(nameof(TestData))]
		public void RoundTrip(string testName, JsonSchema schema)
		{
			var json = _serializer.Serialize(schema);
			Console.WriteLine(json.GetIndentedString());
			var returnTrip = _serializer.Deserialize<JsonSchema>(json);

			Assert.AreEqual(schema, returnTrip);
		}
	}
}
