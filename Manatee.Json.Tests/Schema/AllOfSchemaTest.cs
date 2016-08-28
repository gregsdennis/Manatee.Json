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
 
	File Name:		AllOfSchemaTest.cs
	Namespace:		Manatee.Json.Tests.Schema
	Class Name:		AllOfSchemaTest
	Purpose:		Tests for AllOfSchema.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class AllOfSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnAnyInvalid()
		{
			var schema = new JsonSchema
				{
					AllOf = new List<IJsonSchema>
						{
							new JsonSchema {Type = JsonSchemaTypeDefinition.Array},
							new JsonSchema {Type = JsonSchemaTypeDefinition.Number}
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnAllValid()
		{
			var schema = new JsonSchema
			{
					AllOf = new List<IJsonSchema>
						{
							new JsonSchema {Type = JsonSchemaTypeDefinition.Number,Minimum = 10},
							new JsonSchema {Type = JsonSchemaTypeDefinition.Number,Maximum = 20}
						}
				};
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}