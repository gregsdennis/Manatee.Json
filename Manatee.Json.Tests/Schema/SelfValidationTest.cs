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
			var json = JsonSchema04.MetaSchema.ToJson(null);
			var validation = JsonSchema04.MetaSchema.Validate(json);

			Assert.IsTrue(validation.Valid);
		}

		[TestMethod]
		public void OnlineDraft04()
		{
			var reference = new JsonSchemaReference(JsonSchema04.MetaSchema.Id);
			reference.Validate(new JsonObject());

			var onlineValidation = reference.Validate(JsonSchema04.MetaSchema.ToJson(null));
			var localValidation = JsonSchema04.MetaSchema.Validate(reference.Resolved.ToJson(null));

			Assert.IsTrue(onlineValidation.Valid);
			Assert.IsTrue(localValidation.Valid);
		}
	}
}
