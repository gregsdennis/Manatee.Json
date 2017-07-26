using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class EnumSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnValueOutOfRange()
		{
			var schema = new JsonSchema04
				{
					Enum = new List<EnumSchemaValue>
						{
							new EnumSchemaValue("test1"),
							new EnumSchemaValue("test2")
						}
				};
			var json = (JsonValue) "string";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValueInRange()
		{
			var schema = new JsonSchema04
			{
					Enum = new List<EnumSchemaValue>
						{
							new EnumSchemaValue("test1"),
							new EnumSchemaValue("test2")
						}
				};
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
