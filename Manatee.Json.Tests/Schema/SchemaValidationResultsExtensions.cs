using System;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	public static class SchemaValidationResultsExtensions
	{
		private static readonly JsonSerializer _serializer = new JsonSerializer();

		public static void AssertInvalid(this SchemaValidationResults results)
		{
			Assert.AreEqual(false, results.IsValid);

			Console.WriteLine(string.Join(Environment.NewLine, _serializer.Serialize(results)));
		}
		public static void AssertValid(this SchemaValidationResults results)
		{
			if (!results.IsValid)
				Console.WriteLine(string.Join(Environment.NewLine, _serializer.Serialize(results)));
			
			Assert.AreEqual(true, results.IsValid);
		}
	}
}