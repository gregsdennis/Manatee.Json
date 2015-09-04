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
using System.IO;
using System.Linq;
using Manatee.Json.Path;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Transform;
using Manatee.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class DevTest
	{
		[TestMethod]
		public void Test1()
		{
			JsonValue source = JsonValue.Parse("[{\"Key1\":87,\"Key2\":99,\"Key3\":11},{\"Key1\":42,\"Key2\":-8,\"Key3\":12}]");
			JsonValue template = JsonValue.Parse("[[\"Key1\",\"Key2\",\"Key3\"],[\"$[*]\",\"@.Key1\"],[\"$[*]\",\"@.Key2\"],[\"$[*]\",\"@.Key3\"]]");
			JsonValue reverseTemplate = JsonValue.Parse("[\"$[1][*]\",{\"Key1\":\"@\",\"Key2\":\"$[2][*]\",\"Key3\":\"$[3][*]\"}]");
			JsonValue expected = JsonValue.Parse("[[\"Key1\",\"Key2\",\"Key3\"],[87,42],[99,-8],[11,12]]");
			var result = source.Transform(template);

			Console.WriteLine(expected);
			Console.WriteLine(result);
			Assert.AreEqual(expected, result);

			Console.WriteLine();

			result = expected.Transform(reverseTemplate);

			Console.WriteLine(source);
			Console.WriteLine(result);
			Assert.AreEqual(source, result);
		}
		[TestMethod]
		public void Test2()
		{
			var json = JsonSchema.Draft04.ToJson(null);
			var validation = JsonSchema.Draft04.Validate(json);
			Console.WriteLine(validation.Valid);
		}

		[TestMethod]
		public void SchemaGenerationTest()
		{
			var schema = JsonSchemaFactory.FromType<List<Dictionary<JsonValueType, JsonValue>>>();
			Console.WriteLine(schema.ToJson(null).GetIndentedString());
		}
	}
}