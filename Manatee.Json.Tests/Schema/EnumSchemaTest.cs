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
 
	File Name:		EnumSchemaTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		EnumSchemaTest
	Purpose:		Tests for EnumSchema.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class EnumSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnValueOutOfRange()
		{
			var schema = new JsonSchema
				{
					Enum = new List<EnumSchemaValue>
						{
							new EnumSchemaValue("test1"),
							new EnumSchemaValue("test2")
						}
				};
			var json = (JsonValue) "string";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValueInRange()
		{
			var schema = new JsonSchema
			{
					Enum = new List<EnumSchemaValue>
						{
							new EnumSchemaValue("test1"),
							new EnumSchemaValue("test2")
						}
				};
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
