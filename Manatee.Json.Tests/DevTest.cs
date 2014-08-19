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
using System.Collections.Generic;
using Manatee.Json.Path;
using Manatee.Json.Schema;
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
			var json = new JsonObject
				{
					{
						"store", new JsonObject
							{
								{
									"book", new JsonArray
										{
											new JsonObject
												{
													{"category", "reference"},
													{"author", "Nigel Rees"},
													{"title", "Sayings of the Century"},
													{"price", 8.95},
													{"inStock", false}
												},
											new JsonObject
												{
													{"category", "fiction"},
													{"author", "Evelyn Waugh"},
													{"title", "Sword of Honour"},
													{"price", 12.99},
												},
											new JsonObject
												{
													{"category", "fiction"},
													{"author", "Herman Melville"},
													{"title", "Moby Dick"},
													{"isbn", "0-553-21311-3"},
													{"price", 8.99},
												},
											new JsonObject
												{
													{"category", "fiction"},
													{"author", "J. R. R. Tolkien"},
													{"title", "The Lord of the Rings"},
													{"isbn", "0-395-19395-8"},
													{"price", 22.99},
												},
										}
								},
								{
									"bicycle", new JsonObject
										{
											{"color", "red"},
											{"price", 19.95}
										}
								}
							}
					}
				};

			var maxPrice = Math.Sqrt(100);
			var path = JsonPathWith.Name("store")
			                       .Name("book")
			                       .ArrayFilter(jv => jv.GetNumber("price") < maxPrice);

			var result = path.Evaluate(json);

			Console.WriteLine(path);
			Console.WriteLine(result.GetIndentedString());
		}
		[TestMethod]
		//[Ignore]
		public void SchemaGenerationTest()
		{
			// Having some problems with generating schema from complex or immutable types.
			// For example, the system can't generate for KeyValuePair<,> since the properties aren't read/write.
			// Try Dictionary<string, int> or JsonObject (Dictionary<string, JsonValue>).
			var schema = JsonSchemaFactory.FromTypeBeta(typeof (Dictionary<string, int>));
			Console.WriteLine(schema.ToJson(null));
		}
		[TestMethod]
		public void EscapingTest()
		{
			var str = "{\"string\":\"double\\n\\nspaced\"}";
			var json = JsonValue.Parse(str).Object;
			Console.WriteLine(json["string"].String);
		}
	}
}