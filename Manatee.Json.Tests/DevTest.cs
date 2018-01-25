using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema.TestSuite;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		private class MySchema : JsonSchema04 { }

		private class MyValidator : IJsonSchemaPropertyValidator<MySchema>
		{
			public bool Applies(MySchema schema, JsonValue json)
			{
				throw new NotImplementedException();
			}
			public SchemaValidationResults Validate(MySchema schema, JsonValue json, JsonValue root)
			{
				throw new NotImplementedException();
			}
		}

		private class SchemaValidator : IJsonSchemaPropertyValidator<JsonSchema04>
		{
			public bool Applies(JsonSchema04 schema, JsonValue json)
			{
				throw new NotImplementedException();
			}
			public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
			{
				throw new NotImplementedException();
			}
		}

		[Test]
		public void Test()
		{
			var list = new IJsonSchemaPropertyValidator[] {new SchemaValidator(), new MyValidator()};
			var typed = list.OfType<IJsonSchemaPropertyValidator<MySchema>>().ToList();
			Assert.AreEqual(2, typed.Count);
		}
	}
}