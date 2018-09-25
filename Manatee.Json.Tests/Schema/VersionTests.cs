using System;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class VersionTests
	{
		[Test]
		public void IfIsNotADraft4Keyword()
		{
			var schema = new JsonSchema()
				.Schema(MetaSchemas.Draft04.Id)
				.If(true)
				.Then(true)
				.Else(false);

			var results = schema.ValidateSchema();

			Assert.IsFalse(results.IsValid);
		}

		[Test]
		public void UsingDraft4ExclusiveMaxWithContainsIsNotValid()
		{
			var schema = new JsonSchema()
				.ExclusiveMaximumDraft04(false)
				.Contains(new JsonSchema());

			var results = schema.ValidateSchema();

			Assert.IsFalse(results.IsValid);
		}

		[Test]
		public void Draft4ExclusiveMaxWithMissingMaxIsNotValid()
		{
			var schema = new JsonSchema()
				.ExclusiveMaximumDraft04(true);

			var results = schema.ValidateSchema();

			Console.WriteLine(results.MetaSchemaValidations[MetaSchemas.Draft04.Id].ToJson(new JsonSerializer()).GetIndentedString());
			Assert.IsFalse(results.IsValid);
		}
	}
}
