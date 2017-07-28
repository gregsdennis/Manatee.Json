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

			Assert.IsTrue(validation.Valid);
		}

		[TestMethod]
		public void OnlineDraft04()
		{
			var localSchemaJson = JsonSchema.Draft04.ToJson(null);

			var onlineSchemaText = JsonSchemaOptions.Download(JsonSchema.Draft04.Id);
			var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
			var onlineSchema = JsonSchemaFactory.FromJson(onlineSchemaJson);

			var localValidation = JsonSchema.Draft04.Validate(onlineSchemaJson);
			var onlineValidation = onlineSchema.Validate(localSchemaJson);

			Assert.IsTrue(onlineValidation.Valid);
			Assert.IsTrue(localValidation.Valid);

			Assert.AreEqual(onlineSchemaJson, localSchemaJson);
		}
	}
}
