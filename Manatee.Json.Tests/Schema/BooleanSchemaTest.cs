﻿using System.Collections;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class BooleanSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04 {Type = JsonSchemaTypeDefinition.Boolean});
				yield return new TestCaseData(new JsonSchema06 {Type = JsonSchemaTypeDefinition.Boolean});
			}
		} 
		
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnNonBoolean(IJsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}

		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValidOnBoolean(IJsonSchema schema)
		{
			var json = (JsonValue) false;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
