using System;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class MultiSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNoMatch()
		{
			var schema = new MultiSchema(new StringSchema(), new IntegerSchema());
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}

		[TestMethod]
		public void ValidateReturnsErrorOnMultipleMatches()
		{
			var schema = new MultiSchema(new NumberSchema {Minimum = 5}, new NumberSchema {Minimum = 10});
			var json = (JsonValue)20;

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}

		[TestMethod]
		public void ValidateReturnsValidOnSingleMatch()
		{
			var schema = new MultiSchema(new StringSchema(), new IntegerSchema());
			JsonValue json = 1;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreNotEqual(false, results.Valid);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FromJsonThrowsWhenNonPrimitiveSchemaUsed()
		{
			var schema = new MultiSchema(new StringSchema(), new OneOfSchema());
		}
	}
}
