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
			var reference = new JsonSchemaReference(JsonSchema.Draft04.Id);
			reference.Validate(new JsonObject());

			var onlineValidation = reference.Validate(JsonSchema.Draft04.ToJson(null));
			var localValidation = JsonSchema.Draft04.Validate(reference.Resolved.ToJson(null));

			Assert.IsTrue(onlineValidation.Valid);
			Assert.IsTrue(localValidation.Valid);
		}
	}
}
