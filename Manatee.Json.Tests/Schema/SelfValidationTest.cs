using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class SelfValidationTest
	{
		[Test]
		public void Draft04()
		{
			var json = JsonSchema04.MetaSchema.ToJson(null);
			var validation = JsonSchema04.MetaSchema.Validate(json);

			Assert.IsTrue(validation.Valid);
		}

		[Test]
		public void OnlineDraft04()
		{
			var localSchemaJson = JsonSchema04.MetaSchema.ToJson(null);

			var onlineSchemaText = JsonSchemaOptions.Download(JsonSchema04.MetaSchema.Id);
			var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
			var onlineSchema = JsonSchemaFactory.FromJson(onlineSchemaJson);

			var localValidation = JsonSchema04.MetaSchema.Validate(onlineSchemaJson);
			var onlineValidation = onlineSchema.Validate(localSchemaJson);

			Assert.IsTrue(onlineValidation.Valid);
			Assert.IsTrue(localValidation.Valid);

			Assert.AreEqual(onlineSchemaJson, localSchemaJson);
		}
	}
}
