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
									Type = new NumberSchema {Minimum = -180, Maximum = 180},
									IsRequired = true
								},
							new JsonSchemaPropertyDefinition
								{
									Name = "longitude",
									Type = new NumberSchema {Minimum = -90, Maximum = 90},
									IsRequired = true
								},
							new JsonSchemaPropertyDefinition
								{
									Name = "nestedProperty",
									Type = new ObjectSchema
										{
											Properties = new JsonSchemaPropertyDefinitionCollection
												{
													new JsonSchemaPropertyDefinition
														{
															Name = "test",
															Type = new StringSchema()
														}
												}
										}
								}
						}
				};
			var geoJson = new JsonObject
				{
					{"latitude", 95},
					{"longitude", -36.8},
					{"nestedProperty", new JsonObject {{"test", false}}}
				};
			var validation = geoSchema.Validate(geoJson);
			Console.WriteLine("geo json valid: {0}", validation.Valid);
			foreach (var error in validation.Errors)
			{
				Console.WriteLine("    {0}", error);
			}
		}
	}
}