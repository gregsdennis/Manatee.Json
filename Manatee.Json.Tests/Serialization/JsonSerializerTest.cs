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
 
	File Name:		JsonSerializerTest.cs
	Namespace:		Manatee.Json.Tests.Serialization
	Class Name:		JsonSerializerTest
	Purpose:		This is a test class for JsonSerializer and is intended
					to contain all JsonSerializer serialization unit tests.

***************************************************************************************/

using System;
using System.Collections.Generic;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Serialization
{
	[TestClass]
	public class JsonSerializerTest
	{
		[TestMethod]
		public void RegisteredType_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = TimeSpan.FromDays(1);
			JsonValue expected = TimeSpan.FromDays(1).ToString();
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void DateTimeDefaultOptions_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.ToString("s");
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void DateTimeJavaFormat_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.JavaConstructor
						}
				};
			var obj = DateTime.Today;
			JsonValue expected = string.Format("/Date({0})/", DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond);
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void DateTimeMilliseconds_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.Milliseconds
						}
				};
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond;
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void DateTimeCustom_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.Custom,
							CustomDateTimeSerializationFormat = "yyyy.MM.dd"
						}
				};
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.ToString("yyyy.MM.dd");
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Basic_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.BasicEnumValue,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", 1},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void BasicWithNamedEnum_Successful()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.BasicEnumValue,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "BasicEnumValue"},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void BasicWithNamedEnumWithDescription_Successful()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.EnumValueWithDescription,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "enum_value_with_description"},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void BasicWithNamedFlagsEnum_Successful()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.BasicEnumValue,
					FlagsEnumProp = FlagsEnum.BasicEnumValue,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "BasicEnumValue"},
					{"FlagsEnumProp", "BasicEnumValue"},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void BasicWithNamedFlagsEnumWithDescription_Successful()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.EnumValueWithDescription,
					FlagsEnumProp = FlagsEnum.EnumValueWithDescription,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "enum_value_with_description"},
					{"FlagsEnumProp", "enum_value_with_description"},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void BasicWithNamedMultivalueFlagsEnumWithDescription_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							EnumSerializationFormat = EnumSerializationFormat.AsName,
							FlagsEnumSeparator = " | "
						}
				};
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.EnumValueWithDescription,
					FlagsEnumProp = (FlagsEnum) 3,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "enum_value_with_description"},
					{"FlagsEnumProp", "BasicEnumValue | enum_value_with_description"},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		// This test fails when all tests are run together because the JsonSerializationAbstractionMap
		// is static and one of the other tests has registered ImplementationClass to be used
		// for IInterface, and VS is running the tests concurrently.
		[TestMethod]
		public void AbstractAndInterfaceProps_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = new ObjectWithAbstractAndInterfaceProps
				{
					AbstractProp = new DerivedClass {SomeProp = 42},
					InterfaceProp = new ImplementationClass {RequiredProp = "test comparable"}
				};
			JsonValue expected = new JsonObject
				{
					{
						"AbstractProp", new JsonObject
							{
								{"#Type", typeof (DerivedClass).AssemblyQualifiedName},
								{"SomeProp", 42}
							}
					},
					{
						"InterfaceProp", new JsonObject
							{
								{"#Type", typeof (ImplementationClass).AssemblyQualifiedName},
								{"RequiredProp", "test comparable"}
							}
					}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void AbstractClass_Successful()
		{
			var serializer = new JsonSerializer();
			AbstractClass obj = new DerivedClass
				{
					SomeProp = 42,
					NewProp = "test"
				};
			JsonValue expected = new JsonObject
				{
					{"#Type", typeof (DerivedClass).AssemblyQualifiedName},
					{"SomeProp", 42},
					{"NewProp", "test"}
				};

			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		// This test fails when all tests are run together because the JsonSerializationAbstractionMap
		// is static and one of the other tests has registered ImplementationClass to be used
		// for IInterface, and VS is running the tests concurrently.
		[TestMethod]
		public void Interface_Successful()
		{
			var serializer = new JsonSerializer();
			IInterface obj = new ImplementationClass {RequiredProp = "test"};
			JsonValue expected = new JsonObject
				{
					{"#Type", typeof (ImplementationClass).AssemblyQualifiedName},
					{"RequiredProp", "test"}
				};

			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Nullable_Null_Successful()
		{
			var serializer = new JsonSerializer();
			int? i = null;
			var expected = JsonValue.Null;
			var actual = serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Nullable_NonNull_Successful()
		{
			var serializer = new JsonSerializer();
			int? i = 42;
			JsonValue expected = 42;
			var actual = serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void IJsonSerializable_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = new JsonSerializableClass("test string", 42);
			var expected = new JsonObject
				{
					{"StringProp", "test string"},
					{"IntProp", 42}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Array_Successfull()
		{
			var serializer = new JsonSerializer();
			var list = new[] {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void List_Successfull()
		{
			var serializer = new JsonSerializer();
			var list = new List<int> {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void IEnumerable_Successfull()
		{
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof (IEnumerable<>), typeof (List<>));
			var list = (IEnumerable<int>) new List<int> {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Dictionary_Successful()
		{
			var serializer = new JsonSerializer();
			var dict = new Dictionary<string, double> {{"four", 4}, {"three", 3}, {"five", 5}, {"six", 6}};
			JsonValue expected = new JsonArray
				{
					new JsonObject {{"Key", "four"}, {"Value", 4}},
					new JsonObject {{"Key", "three"}, {"Value", 3}},
					new JsonObject {{"Key", "five"}, {"Value", 5}},
					new JsonObject {{"Key", "six"}, {"Value", 6}}
				};
			var actual = serializer.Serialize(dict);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Queue_Successfull()
		{
			var serializer = new JsonSerializer();
			var queue = new Queue<int>();
			queue.Enqueue(4);
			queue.Enqueue(3);
			queue.Enqueue(5);
			queue.Enqueue(6);
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(queue);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Stack_Successfull()
		{
			var serializer = new JsonSerializer();
			var stack = new Stack<int>();
			stack.Push(4);
			stack.Push(3);
			stack.Push(5);
			stack.Push(6);
			JsonValue expected = new JsonArray {6, 5, 3, 4};
			var actual = serializer.Serialize(stack);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void SerializeType_Successfull()
		{
			var serializer = new JsonSerializer();
			ObjectWithBasicProps.StaticStringProp = "staticStringValue";
			ObjectWithBasicProps.StaticIntProp = 42;
			ObjectWithBasicProps.StaticDoubleProp = 6.0;
			ObjectWithBasicProps.StaticBoolProp = true;
			JsonValue expected = new JsonObject
				{
					{"StaticStringProp", "staticStringValue"},
					{"StaticIntProp", 42},
					{"StaticDoubleProp", 6},
					{"StaticBoolProp", true}
				};
			var actual = serializer.SerializeType<ObjectWithBasicProps>();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void DefaultOptions_IgnoresDefaultValues()
		{
			var serializer = new JsonSerializer();
			// DoubleProp remains default
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					BoolProp = true
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"BoolProp", true}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void CustomOptions_SerializesDefaultValues()
		{
			var serializer = new JsonSerializer {Options = {EncodeDefaultValues = true}};
			// DoubleProp remains default
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					BoolProp = true
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 0},
					{"BoolProp", true},
					{"EnumProp", 0},
					{"FlagsEnumProp", 0},
					{"MapToMe", 0}
				};
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void CircularStructure_SerializesWithReference()
		{
			var serializer = new JsonSerializer();
			var obj = new ObjectWithExtendedProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					BoolProp = true
				};
			var obj2 = new ObjectWithExtendedProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					BoolProp = true,
					LoopProperty = obj
				};
			obj.LoopProperty = obj2;

			var actual = serializer.Serialize(obj);

			Assert.IsNotNull(actual);
			Assert.AreEqual(JsonValueType.Object, actual.Type);
			Assert.IsTrue(actual.Object.ContainsKey("#Def"));
			Assert.IsTrue(actual.Object.ContainsKey("LoopProperty"));
			Assert.AreEqual(JsonValueType.Object, actual.Object["LoopProperty"].Type);
			Assert.IsTrue(actual.Object["LoopProperty"].Object.ContainsKey("LoopProperty"));
			Assert.AreEqual(JsonValueType.Object, actual.Object["LoopProperty"].Object["LoopProperty"].Type);
			Assert.IsTrue(actual.Object["LoopProperty"].Object["LoopProperty"].Object.ContainsKey("#Ref"));
			Assert.AreEqual(actual.Object["#Def"], actual.Object["LoopProperty"].Object["LoopProperty"].Object["#Ref"]);
			Assert.AreEqual(0, serializer.SerializationMap.Count);
		}
		[TestMethod]
		public void Fields()
		{
			var serializer = new JsonSerializer();
			serializer.Options.AutoSerializeFields = true;
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true,
					EnumProp = TestEnum.BasicEnumValue,
					MappedProp = 4,
					Field = "test"
				};
			JsonValue expected = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", 1},
					{"MapToMe", 4},
					{"Field", "test"}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ObjectWithAllDefaultValues()
		{
			var obj = new ObjectWithExtendedProps();

			var serializer = new JsonSerializer {Options = {AutoSerializeFields = true, EncodeDefaultValues = true}};

			var json = serializer.Serialize(obj);

			Console.WriteLine(json);
		}
		[TestMethod]
		public void NullableWithNonNullDefaultValue()
		{
			bool? obj = false;
			JsonValue expected = false;
			var serializer = new JsonSerializer();
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
	}
}
