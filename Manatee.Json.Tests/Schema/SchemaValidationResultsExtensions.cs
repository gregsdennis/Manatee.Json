using System;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	public static class SchemaValidationResultsExtensions
	{
		public static void AssertInvalid(this SchemaValidationResults results)
		{
			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);

			Console.WriteLine(string.Join(Environment.NewLine, results.Errors));
		}
		public static void AssertValid(this SchemaValidationResults results)
		{
			if (!results.Valid)
				Console.WriteLine(string.Join(Environment.NewLine, results.Errors));
			
			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}