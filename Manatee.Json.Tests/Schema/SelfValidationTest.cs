using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class SelfValidationTest
	{
		[TestMethod]
		public void Draft04()
		{
			var json = JsonSchema.Draft04.ToJson(null);
			var validation = JsonSchema.Draft04.Validate(json);
			Console.WriteLine(validation.Valid);
		}
	}
}
