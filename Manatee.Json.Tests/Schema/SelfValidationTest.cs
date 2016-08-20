/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		SelfValidationTest.cs
	Namespace:		Manatee.Json.Tests.Schema
	Class Name:		SelfValidationTest
	Purpose:		Tests to verify that Draft-04 is self-validating.

***************************************************************************************/
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
