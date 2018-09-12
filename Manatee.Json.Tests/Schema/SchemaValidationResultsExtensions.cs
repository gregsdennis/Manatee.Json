using System;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	public static class SchemaValidationResultsExtensions
	{
		public static void AssertInvalid(this SchemaValidationResults results)
		{
			Assert.AreEqual(false, results.IsValid);

			Console.WriteLine(string.Join(Environment.NewLine, results.NestedResults));
		}
		public static void AssertValid(this SchemaValidationResults results)
		{
			if (!results.IsValid)
				Console.WriteLine(string.Join(Environment.NewLine, results.NestedResults));
			
			Assert.AreEqual(true, results.IsValid);
		}
	}
}