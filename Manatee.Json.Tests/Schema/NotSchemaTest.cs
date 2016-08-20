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
 
	File Name:		NotSchemaTest.cs
	Namespace:		Manatee.Json.Tests.Schema
	Class Name:		NotSchemaTest
	Purpose:		Tests for NotSchema.

***************************************************************************************/

using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class NotSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnInvalid()
		{
			var schema = new JsonSchema
				{
					Not = new JsonSchema {Type = JsonSchemaTypeDefinition.Array}
				};
			var json = new JsonArray();

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValid()
		{
			var schema = new JsonSchema
			{
					Not = new JsonSchema { Type = JsonSchemaTypeDefinition.Number,Maximum = 10}
				};
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}