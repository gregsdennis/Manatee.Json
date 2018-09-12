using System;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	public static class SchemaValidationResultsExtensions
	{
		private static readonly JsonSerializer _serializer = new JsonSerializer();

		public static void AssertInvalid(this SchemaValidationResults results, SchemaValidationResults expected = null)
		{
			Console.WriteLine("expected:\n{0}", _serializer.Serialize(expected).GetIndentedString());
			Console.WriteLine("actual:\n{0}", _serializer.Serialize(results).GetIndentedString());

			if (expected == null)
				Assert.IsFalse(results.IsValid);
			else
				Assert.AreEqual(expected, results);
		}
		public static void AssertValid(this SchemaValidationResults results, SchemaValidationResults expected = null)
		{
			Console.WriteLine("expected:\n{0}", _serializer.Serialize(expected).GetIndentedString());
			Console.WriteLine("actual:\n{0}", _serializer.Serialize(results).GetIndentedString());

			if (expected == null)
				Assert.IsTrue(results.IsValid);
			else
				Assert.AreEqual(expected, results);
		}
	}
}