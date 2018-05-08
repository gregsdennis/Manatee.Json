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
	public class ConstSchemaTests
	{
		[Test]
		public void ValidationFails()
		{
			var schema = new JsonSchema07
				{
					Const = 5
				};

			JsonValue json = 6;

			var results = schema.Validate(json);

			Assert.IsFalse(results.Valid);
			Assert.AreEqual("Expected: 5; Actual: 6.", results.Errors.First().Message);
		}

		[Test]
		public void ValidationPasses()
		{
			var schema = new JsonSchema07
				{
					Const = 5
				};

			JsonValue json = 5;

			var results = schema.Validate(json);

			Assert.IsTrue(results.Valid);
		}
	}
}
