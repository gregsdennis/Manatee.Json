﻿/***************************************************************************************

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
			JsonSerializationTypeRegistry.RegisterListType<int>();
			JsonSerializationTypeRegistry.RegisterArrayType<bool>();
			var serializer = new JsonSerializer {Options = {EncodeDefaultValues = true, AutoSerializeFields = true}};
			var json = serializer.GenerateTemplate<ObjectWithExtendedProps>();

			Console.WriteLine(json);
		}
		[TestMethod]
		public void Test2()
		{
			var text = "{\"key\" : \"value\", \"key2\" : [\"arrayValue1\", \"innerValue\", null, false]}";
			var json = JsonValue.Parse(text);

			var value = 2;
			var path = JsonPathWith.Name("key2").Array(ja => ja.Length() * (5 - value));

			Console.WriteLine(path);
			Console.WriteLine(path.Evaluate(json));
		}

		[TestMethod]
		[Ignore]
		public void SchemaGenerationTest()
		{
			// Having some problems with generating schema from complex or immutable types.
			// For example, the system can't generate for KeyValuePair<,> since the properties aren't read/write.
			// Try Dictionary<string, int>.
			var schema = JsonSchemaFactory.FromTypeBeta(typeof (Dictionary<string, int>));
			Console.WriteLine(schema.ToJson(null));
		}
	}
}