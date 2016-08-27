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
 
	File Name:		GoessnerExamplesTest.cs
	Namespace:		Manatee.Json.Tests.Path
	Class Name:		GoessnerExamplesTest
	Purpose:		Tests for the examples of JSONPath presented on Stefan Goessner's
					site, http://goessner.net/articles/JsonPath/.

***************************************************************************************/
using System;
using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class GoessnerExamplesTest
	{
		public static readonly JsonValue GoessnerData =
			new JsonObject
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
													{"price", 8.95}
												},
											new JsonObject
												{
													{"category", "fiction"},
													{"author", "Evelyn Waugh"},
													{"title", "Sword of Honour"},
													{"price", 12.99}
												},
											new JsonObject
												{
													{"category", "fiction"},
													{"author", "Herman Melville"},
													{"title", "Moby Dick"},
													{"isbn", "0-553-21311-3"},
													{"price", 8.99}
												},
											new JsonObject
												{
													{"category", "fiction"},
													{"author", "J. R. R. Tolkien"},
													{"title", "The Lord of the Rings"},
													{"isbn", "0-395-19395-8"},
													{"price", 22.99}
												}
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

		[TestMethod]
		public void GoessnerExample1Parsed()
		{
			var path = JsonPath.Parse("$.store.book[*].author");
			var expected = new JsonArray {"Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien"};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample1Constructed()
		{
			var path = JsonPathWith.Name("store").Name("book").Array().Name("author");
			var expected = new JsonArray {"Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien"};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample2Parsed()
		{
			var path = JsonPath.Parse("$..author");
			var expected = new JsonArray {"Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien"};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample2Constructed()
		{
			var path = JsonPathWith.Search("author");
			var expected = new JsonArray {"Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien"};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample3Parsed()
		{
			var path = JsonPath.Parse("$.store.*");
			var expected = new JsonArray
				{
					new JsonArray
						{
							new JsonObject
								{
									{"category", "reference"},
									{"author", "Nigel Rees"},
									{"title", "Sayings of the Century"},
									{"price", 8.95}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Evelyn Waugh"},
									{"title", "Sword of Honour"},
									{"price", 12.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Herman Melville"},
									{"title", "Moby Dick"},
									{"isbn", "0-553-21311-3"},
									{"price", 8.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "J. R. R. Tolkien"},
									{"title", "The Lord of the Rings"},
									{"isbn", "0-395-19395-8"},
									{"price", 22.99}
								}
						},
					new JsonObject
						{
							{"color", "red"},
							{"price", 19.95}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample3Constructed()
		{
			var path = JsonPathWith.Name("store").Wildcard();
			var expected = new JsonArray
				{
					new JsonArray
						{
							new JsonObject
								{
									{"category", "reference"},
									{"author", "Nigel Rees"},
									{"title", "Sayings of the Century"},
									{"price", 8.95}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Evelyn Waugh"},
									{"title", "Sword of Honour"},
									{"price", 12.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Herman Melville"},
									{"title", "Moby Dick"},
									{"isbn", "0-553-21311-3"},
									{"price", 8.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "J. R. R. Tolkien"},
									{"title", "The Lord of the Rings"},
									{"isbn", "0-395-19395-8"},
									{"price", 22.99}
								}
						},
					new JsonObject
						{
							{"color", "red"},
							{"price", 19.95}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample4Parsed()
		{
			var path = JsonPath.Parse("$.store..price");
			var expected = new JsonArray {8.95, 12.99, 8.99, 22.99, 19.95};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample4Constructed()
		{
			var path = JsonPathWith.Name("store").Search("price");
			var expected = new JsonArray {8.95, 12.99, 8.99, 22.99, 19.95};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample5Parsed()
		{
			var path = JsonPath.Parse("$..book[2]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample5Constructed()
		{
			var path = JsonPathWith.Search("book").Array(2);
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample6AParsed()
		{
			var path = JsonPath.Parse("$..book[(@.length-1)]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample6AConstructed()
		{
			var path = JsonPathWith.Search("book").Array(ja => ja.Length() - 1);
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample6BParsed()
		{
			var path = JsonPath.Parse("$..book[-1:]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample6BConstructed()
		{
			var path = JsonPathWith.Search("book").ArraySlice(-1, null);
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample7AParsed()
		{
			var path = JsonPath.Parse("$..book[0,1]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Evelyn Waugh"},
							{"title", "Sword of Honour"},
							{"price", 12.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample7AConstructed()
		{
			var path = JsonPathWith.Search("book").Array(0, 1);
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Evelyn Waugh"},
							{"title", "Sword of Honour"},
							{"price", 12.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample7BParsed()
		{
			var path = JsonPath.Parse("$..book[:2]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Evelyn Waugh"},
							{"title", "Sword of Honour"},
							{"price", 12.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample7BConstructed()
		{
			var path = JsonPathWith.Search("book").ArraySlice(null, 2);
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Evelyn Waugh"},
							{"title", "Sword of Honour"},
							{"price", 12.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample8Parsed()
		{
			var path = JsonPath.Parse("$..book[?(@.isbn)]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample8Constructed()
		{
			var path = JsonPathWith.Search("book").Array(jv => jv.HasProperty("isbn"));
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample9Parsed()
		{
			var path = JsonPath.Parse("$..book[?(@.price<10)]");
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample9Constructed()
		{
			var path = JsonPathWith.Search("book").Array(jv => jv.Name("price") < 10);
			Console.WriteLine(path);
			var expected = new JsonArray
				{
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						}
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample10Parsed()
		{
			var path = JsonPath.Parse("$..*");
			var expected = new JsonArray
				{
					new JsonObject
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
															{"price", 8.95}
														},
													new JsonObject
														{
															{"category", "fiction"},
															{"author", "Evelyn Waugh"},
															{"title", "Sword of Honour"},
															{"price", 12.99}
														},
													new JsonObject
														{
															{"category", "fiction"},
															{"author", "Herman Melville"},
															{"title", "Moby Dick"},
															{"isbn", "0-553-21311-3"},
															{"price", 8.99}
														},
													new JsonObject
														{
															{"category", "fiction"},
															{"author", "J. R. R. Tolkien"},
															{"title", "The Lord of the Rings"},
															{"isbn", "0-395-19395-8"},
															{"price", 22.99}
														}
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
						},
					new JsonObject
						{
							{
								"book", new JsonArray
									{
										new JsonObject
											{
												{"category", "reference"},
												{"author", "Nigel Rees"},
												{"title", "Sayings of the Century"},
												{"price", 8.95}
											},
										new JsonObject
											{
												{"category", "fiction"},
												{"author", "Evelyn Waugh"},
												{"title", "Sword of Honour"},
												{"price", 12.99}
											},
										new JsonObject
											{
												{"category", "fiction"},
												{"author", "Herman Melville"},
												{"title", "Moby Dick"},
												{"isbn", "0-553-21311-3"},
												{"price", 8.99}
											},
										new JsonObject
											{
												{"category", "fiction"},
												{"author", "J. R. R. Tolkien"},
												{"title", "The Lord of the Rings"},
												{"isbn", "0-395-19395-8"},
												{"price", 22.99}
											}
									}
							},
							{
								"bicycle", new JsonObject
									{
										{"color", "red"},
										{"price", 19.95}
									}
							}
						},
					new JsonArray
						{
							new JsonObject
								{
									{"category", "reference"},
									{"author", "Nigel Rees"},
									{"title", "Sayings of the Century"},
									{"price", 8.95}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Evelyn Waugh"},
									{"title", "Sword of Honour"},
									{"price", 12.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Herman Melville"},
									{"title", "Moby Dick"},
									{"isbn", "0-553-21311-3"},
									{"price", 8.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "J. R. R. Tolkien"},
									{"title", "The Lord of the Rings"},
									{"isbn", "0-395-19395-8"},
									{"price", 22.99}
								}
						},
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					"reference",
					"Nigel Rees",
					"Sayings of the Century",
					8.95,
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Evelyn Waugh"},
							{"title", "Sword of Honour"},
							{"price", 12.99}
						},
					"fiction",
					"Evelyn Waugh",
					"Sword of Honour",
					12.99,
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						},
					"fiction",
					"Herman Melville",
					"Moby Dick",
					"0-553-21311-3",
					8.99,
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						},
					"fiction",
					"J. R. R. Tolkien",
					"The Lord of the Rings",
					"0-395-19395-8",
					22.99,
					new JsonObject
						{
							{"color", "red"},
							{"price", 19.95}
						},
					"red",
					19.95
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GoessnerExample10Constructed()
		{
			var path = JsonPathWith.Search();
			var expected = new JsonArray
				{
					new JsonObject
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
															{"price", 8.95}
														},
													new JsonObject
														{
															{"category", "fiction"},
															{"author", "Evelyn Waugh"},
															{"title", "Sword of Honour"},
															{"price", 12.99}
														},
													new JsonObject
														{
															{"category", "fiction"},
															{"author", "Herman Melville"},
															{"title", "Moby Dick"},
															{"isbn", "0-553-21311-3"},
															{"price", 8.99}
														},
													new JsonObject
														{
															{"category", "fiction"},
															{"author", "J. R. R. Tolkien"},
															{"title", "The Lord of the Rings"},
															{"isbn", "0-395-19395-8"},
															{"price", 22.99}
														}
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
						},
					new JsonObject
						{
							{
								"book", new JsonArray
									{
										new JsonObject
											{
												{"category", "reference"},
												{"author", "Nigel Rees"},
												{"title", "Sayings of the Century"},
												{"price", 8.95}
											},
										new JsonObject
											{
												{"category", "fiction"},
												{"author", "Evelyn Waugh"},
												{"title", "Sword of Honour"},
												{"price", 12.99}
											},
										new JsonObject
											{
												{"category", "fiction"},
												{"author", "Herman Melville"},
												{"title", "Moby Dick"},
												{"isbn", "0-553-21311-3"},
												{"price", 8.99}
											},
										new JsonObject
											{
												{"category", "fiction"},
												{"author", "J. R. R. Tolkien"},
												{"title", "The Lord of the Rings"},
												{"isbn", "0-395-19395-8"},
												{"price", 22.99}
											}
									}
							},
							{
								"bicycle", new JsonObject
									{
										{"color", "red"},
										{"price", 19.95}
									}
							}
						},
					new JsonArray
						{
							new JsonObject
								{
									{"category", "reference"},
									{"author", "Nigel Rees"},
									{"title", "Sayings of the Century"},
									{"price", 8.95}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Evelyn Waugh"},
									{"title", "Sword of Honour"},
									{"price", 12.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "Herman Melville"},
									{"title", "Moby Dick"},
									{"isbn", "0-553-21311-3"},
									{"price", 8.99}
								},
							new JsonObject
								{
									{"category", "fiction"},
									{"author", "J. R. R. Tolkien"},
									{"title", "The Lord of the Rings"},
									{"isbn", "0-395-19395-8"},
									{"price", 22.99}
								}
						},
					new JsonObject
						{
							{"category", "reference"},
							{"author", "Nigel Rees"},
							{"title", "Sayings of the Century"},
							{"price", 8.95}
						},
					"reference",
					"Nigel Rees",
					"Sayings of the Century",
					8.95,
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Evelyn Waugh"},
							{"title", "Sword of Honour"},
							{"price", 12.99}
						},
					"fiction",
					"Evelyn Waugh",
					"Sword of Honour",
					12.99,
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "Herman Melville"},
							{"title", "Moby Dick"},
							{"isbn", "0-553-21311-3"},
							{"price", 8.99}
						},
					"fiction",
					"Herman Melville",
					"Moby Dick",
					"0-553-21311-3",
					8.99,
					new JsonObject
						{
							{"category", "fiction"},
							{"author", "J. R. R. Tolkien"},
							{"title", "The Lord of the Rings"},
							{"isbn", "0-395-19395-8"},
							{"price", 22.99}
						},
					"fiction",
					"J. R. R. Tolkien",
					"The Lord of the Rings",
					"0-395-19395-8",
					22.99,
					new JsonObject
						{
							{"color", "red"},
							{"price", 19.95}
						},
					"red",
					19.95
				};
			var actual = path.Evaluate(GoessnerData);

			Assert.AreEqual(expected, actual);
		}
	}
}
