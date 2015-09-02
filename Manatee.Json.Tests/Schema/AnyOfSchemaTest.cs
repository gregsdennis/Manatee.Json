/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		AnyOfSchemaTest.cs
	Namespace:		Manatee.Json.Tests.Schema
	Class Name:		AnyOfSchemaTest
	Purpose:		Tests for AnyOfSchema.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class AnyOfSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNoneValid()
		{
			var schema = new AnyOfSchema
				{
					Options = new List<IJsonSchema>
						{
							new ArraySchema(),
							new NumberSchema()
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnSingleValid()
		{
			var schema = new AnyOfSchema
				{
					Options = new List<IJsonSchema>
						{
							new NumberSchema {Minimum = 10},
							new NumberSchema {Maximum = 20}
						}
				};
			var json = (JsonValue) 25;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}