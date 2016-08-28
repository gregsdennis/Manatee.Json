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
 
	File Name:		JsonSchemaTypeTest.cs
	Namespace:		Manatee.Json.Tests.Schema
	Class Name:		JsonSchemaTypeTest
	Purpose:		Tests to validate the "type" property.

***************************************************************************************/
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class JsonSchemaTypeTest
	{
		[TestMethod]
		public void PrimitiveSchemaSucceeds()
		{
			var json = new JsonObject {{"type", "integer"}};

			var results = JsonSchema.Draft04.Validate(json);

			Assert.IsTrue(results.Valid);
		}
		[TestMethod]
		public void NonPrimitiveStringSchemaFails()
		{
			var json = new JsonObject {{"type", "other"}};

			var results = JsonSchema.Draft04.Validate(json);

			Assert.IsFalse(results.Valid);
		}
		[TestMethod]
		public void ConcoctedExampleFails()
		{
			// This test is intended to demontrate that it's not possible to create
			// a primitive definition; you must use the built-in definitions.
			// The type definition equality logic relies on this fact.
			var schema = new JsonSchema
				{
					Type = new JsonSchemaTypeDefinition("number")
						{
							Definition = new JsonSchema
								{
									Type = JsonSchemaTypeDefinition.Number
								}
						}
				};
			var json = new JsonObject { { "type", "number" } };

			var results = schema.Validate(json);

			Assert.IsFalse(results.Valid);
		}
	}
}
