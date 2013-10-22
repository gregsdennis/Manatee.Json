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
			var schema = JsonSchema.Draft04;
			var geoSchemaJson = new JsonObject
				{
					{"description", "A geographical coordinate"},
					{"type", "object"},
					{
						"properties", new JsonObject
							{
								{"latitude", new JsonObject {{"type", "number"}}},
								{"longitude", new JsonObject {{"type", "number"}}}
							}
					},
					{"required", new JsonArray {"latitude", "longitude"}}
				};
			Console.WriteLine("geo schema valid: {0}", schema.Validate(geoSchemaJson));
			var geoSchema = JsonSchemaFactory.FromJson(geoSchemaJson);
			var geoJson = new JsonObject
				{
					{"latitude", 95.4},
					{"longitude", 36.8}
				};
			Console.WriteLine("geo json valid: {0}", geoSchema.Validate(geoJson));
		}
		[TestMethod]
		//[Ignore]
		public void Test2()
		{
			var schema = JsonSchema.Draft04;
			var geoSchemaJson = new JsonObject
				{
					{"$ref", "http://json-schema.org/geo"}
				};
			Console.WriteLine("geo schema valid: {0}", schema.Validate(geoSchemaJson));
			var serializer = new JsonSerializer();
			var geoSchema = serializer.Deserialize<IJsonSchema>(geoSchemaJson);
			var geoJson = new JsonObject
				{
					{"latitude", 95.4},
					{"longitude", 36.8}
				};
			Console.WriteLine("geo json valid: {0}", geoSchema.Validate(geoJson));
		}
		[TestMethod]
		//[Ignore]
		public void Test3()
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
					{"longitude", 36.8}
				};
			Console.WriteLine("geo json valid: {0}", geoSchema.Validate(geoJson));
		}
	}
}