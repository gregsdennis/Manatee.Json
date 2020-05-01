using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	//[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test()
		{
			Regex p = new Regex("^🐲*$");
			Console.WriteLine(p.IsMatch("🐲"));
			Console.WriteLine(p.IsMatch("🐲🐲"));
		}

		[Test]
		public void Test2()
		{
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Verbose;

			var schema = new JsonSchema
				{
					new ItemsKeyword{IsArray = true}
				};
			var json = new JsonArray{"1", "2", "3"};

			var result = schema.Validate(json);

			Console.WriteLine(result.ToJson(null).GetIndentedString());
		}
	}
}