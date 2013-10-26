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
 
	File Name:		DevTest.cs
	Namespace:		Manatee.Tests
	Class Name:		DevTest
	Purpose:		Provides a single method through which one can test any specific
					scenario.  Should be marked [Ignore] when committing to the
					repository.

***************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class DevTest
	{
		[TestMethod]
		//[Ignore]
		public void Test1()
		{
			var geoSchema = new ObjectSchema
			{
				Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition
								{
									Name = "latitude",
									Type = new NumberSchema(),
									IsRequired = true
								},
							new JsonSchemaPropertyDefinition
								{
									Name = "longitude",
									Type = new NumberSchema(),
									IsRequired = true
								}
						}
			};
			var geoJson = new JsonObject
				{
					{"latitude", 95.4},
					//{"longitude", 36.8}
				};
			var result = geoSchema.Validate(geoJson);
			Console.WriteLine("geo json valid: {0}", result.Valid);
			foreach (var error in result.Errors)
			{
				Console.WriteLine("    {0}", error);
			}
		}
		[TestMethod]
		//[Ignore]
		public void Test2()
		{
			// Having some problems with generating schema from complex or immutable types.
			// For example, the system can't generate for KeyValuePair<,> since the properties aren't read/write.
			// Try Dictionary<string, int> or JsonObject (Dictionary<string, JsonValue>).
			var schema = JsonSchemaFactory.FromType(typeof (Dictionary<string, int>));
			Console.WriteLine(schema.ToJson());
		}
	}
}