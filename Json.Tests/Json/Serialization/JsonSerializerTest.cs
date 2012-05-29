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
	Namespace:		Manatee.Tests.Json.Serialization
	Class Name:		JsonSerializerTest
	Purpose:		This is a test class for JsonSerializer and is intended
					to contain all JsonSerializer Unit Tests

***************************************************************************************/
using System;
using System.Collections.Generic;
using Manatee.Json;
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;
using Manatee.Json.Serialization;
using Manatee.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Tests.Json.Serialization
{
	[TestClass]
	public class JsonSerializerTest
	{
		private JsonSerializer _serializer = new JsonSerializer();

		#region Deserialize Tests
		[TestMethod]
		public void Deserialize_RegisteredType_Successful()
		{
			JsonValue json = TimeSpan.FromDays(1).ToString();
			var expected = TimeSpan.FromDays(1);
			var actual = _serializer.Deserialize<TimeSpan>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeDefaultOptions_Successful()
		{
			JsonValue json = DateTime.Today.ToString("");
			var expected = DateTime.Today;
			var actual = _serializer.Deserialize<DateTime>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeJavaFormat_Successful()
		{
			_serializer.Options = new JsonSerializerOptions
			{
				DateTimeSerializationFormat = DateTimeSerializationFormat.JavaConstructor
			};
			JsonValue json = string.Format("/Date({0})/", DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond);
			var expected = DateTime.Today;
			var actual = _serializer.Deserialize<DateTime>(json);
			_serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeMilliseconds_Successful()
		{
			_serializer.Options = new JsonSerializerOptions
			{
				DateTimeSerializationFormat = DateTimeSerializationFormat.Milliseconds
			};
			JsonValue json = DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond;
			var expected = DateTime.Today;
			var actual = _serializer.Deserialize<DateTime>(json);
			_serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Basic_Successful()
		{
			var json = new JsonObject
			           	{
			           		{"StringProp", "stringValue"},
			           		{"IntProp", 42},
			           		{"DoubleProp", 6},
			           		{"BoolProp", true}
			           	};
			var expected = new ObjectWithBasicProps
							{
								StringProp = "stringValue",
								IntProp = 42,
								DoubleProp = 6.0,
								BoolProp = true
							};
			var actual = _serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_AbstractAndInterface_Successful()
		{
			var json = new JsonObject{
										{"AbstractProp", new JsonObject
															{
																{"#Type", typeof(DerivedClass).AssemblyQualifiedName},
																{"#Value", new JsonObject{{"SomeProp", 42}}}
															}},
										{"InterfaceProp", new JsonObject
															{
																{"#Type", typeof(string).AssemblyQualifiedName},
																{"#Value", "test comparable"}
															}}
									};
			var expected = new ObjectWithAbstractAndInterfaceProps
							{
								AbstractProp = new DerivedClass {SomeProp = 42},
								InterfaceProp = "test comparable"
							};
			var actual = _serializer.Deserialize<ObjectWithAbstractAndInterfaceProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Nullable_Null_Successful()
		{
			JsonSerializationTypeRegistry.RegisterNullableType<int>();
			int? expected = null;
			var json = JsonValue.Null;
			var actual = _serializer.Deserialize<int?>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Nullable_NonNull_Successful()
		{
			JsonSerializationTypeRegistry.RegisterNullableType<int>();
			int? expected = 42;
			JsonValue json = 42;
			var actual = _serializer.Deserialize<int?>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_IJsonCompatible_Successfull()
		{
			var expected = new JsonCompatibleClass("test string", 42);
			var json = new JsonObject
			               	{
			               		{"StringProp", "test string"},
			               		{"IntProp", 42}
			               	};
			var actual = _serializer.Deserialize<JsonCompatibleClass>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_List_Successful()
		{
			JsonSerializationTypeRegistry.RegisterListType<int>();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new List<int> {4, 3, 5, 6};
			var actual = _serializer.Deserialize<List<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
		[TestMethod]
		public void Deserialize_Dictionary_Successful()
		{
			JsonSerializationTypeRegistry.RegisterDictionaryType<string, double>();
			var expected = new Dictionary<string, double> {{"four", 4}, {"three", 3}, {"five", 5}, {"six", 6}};
			JsonValue json = new JsonArray
			                     	{
			                     		new JsonObject {{"Key", "four"}, {"Value", 4}},
			                     		new JsonObject {{"Key", "three"}, {"Value", 3}},
			                     		new JsonObject {{"Key", "five"}, {"Value", 5}},
			                     		new JsonObject {{"Key", "six"}, {"Value", 6}}
			                     	};
			var actual = _serializer.Deserialize<Dictionary<string, double>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			foreach (var key in actual.Keys)
			{
				Assert.IsTrue(expected.ContainsKey(key));
				Assert.AreEqual(expected[key], actual[key]);
			}
		}
		[TestMethod]
		public void Deserialize_Queue_Successful()
		{
			JsonSerializationTypeRegistry.RegisterQueueType<int>();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new Queue<int>();
			expected.Enqueue(4);
			expected.Enqueue(3);
			expected.Enqueue(5);
			expected.Enqueue(6);
			var actual = _serializer.Deserialize<Queue<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected.ToArray()[i], actual.ToArray()[i]);
			}
		}
		[TestMethod]
		public void Deserialize_Stack_Successful()
		{
			JsonSerializationTypeRegistry.RegisterStackType<int>();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new Stack<int>();
			expected.Push(4);
			expected.Push(3);
			expected.Push(5);
			expected.Push(6);
			var actual = _serializer.Deserialize<Stack<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected.ToArray()[i], actual.ToArray()[i]);
			}
		}
		[TestMethod]
		public void DeserializeType_Successfull()
		{
			var stringProp = "staticStringValue";
			var intProp = 42;
			var doubleProp = 6.0;
			var boolProp = true;
			var json = new JsonObject
			           	{
			           		{"StaticStringProp", "staticStringValue"},
			           		{"StaticIntProp", 42},
			           		{"StaticDoubleProp", 6},
			           		{"StaticBoolProp", true}
			           	};
			_serializer.DeserializeType<ObjectWithBasicProps>(json);
			Assert.AreEqual(ObjectWithBasicProps.StaticStringProp, stringProp);
			Assert.AreEqual(ObjectWithBasicProps.StaticIntProp, intProp);
			Assert.AreEqual(ObjectWithBasicProps.StaticDoubleProp, doubleProp);
			Assert.AreEqual(ObjectWithBasicProps.StaticBoolProp, boolProp);
		}
		[TestMethod]
		public void Deserialize_DefaultOptions_IgnoresExtraProperties()
		{
			var json = new JsonObject
			           	{
			           		{"StringProp", "stringValue"},
			           		{"IntProp", 42},
							{"UnknownProp", "string"},
			           		{"DoubleProp", 6},
			           		{"BoolProp", true},
							{"OtherProp", Math.PI}
			           	};
			var expected = new ObjectWithBasicProps
			               	{
			               		StringProp = "stringValue",
			               		IntProp = 42,
			               		DoubleProp = 6.0,
			               		BoolProp = true
			               	};
			var actual = _serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_CustomOptions_ThrowsTypeDoesNotContainPropertyException()
		{
			try
			{
				_serializer.Options = new JsonSerializerOptions
										{
											InvalidPropertyKeyBehavior = InvalidPropertyKeyBehavior.ThrowException
										};
				var json = new JsonObject
			           	{
			           		{"StringProp", "stringValue"},
			           		{"IntProp", 42},
							{"UnknownProp", "string"},
			           		{"DoubleProp", 6},
			           		{"BoolProp", true},
							{"OtherProp", Math.PI}
			           	};
				var expected = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					DoubleProp = 6.0,
					BoolProp = true
				};
				var actual = _serializer.Deserialize<ObjectWithBasicProps>(json);
				Assert.Fail("Did not throw exception.");
			}
			catch (TypeDoesNotContainPropertyException) { }
			catch (AssertFailedException)
			{
				throw;
			}
			catch (Exception e)
			{
				Assert.Fail(string.Format("Threw {0}.", e.GetType()));
			}
			finally
			{
				_serializer.Options = null;
			}
		}
		[TestMethod]
		public void Deserialize_CircularStructure_MaintainsReferences()
		{
			var json = JsonValue.Parse(@"{""LoopProperty"":{""LoopProperty"":{""#Ref"":""f3a2993b-9b0c-4296-872e-95a9210295f4""},""StringProp"":""stringValueB"",""IntProp"":6,""BoolProp"":true},""StringProp"":""stringValueA"",""IntProp"":42,""#Define"":""f3a2993b-9b0c-4296-872e-95a9210295f4""}");
			var expected = new ObjectWithExtendedProps
			{
				StringProp = "stringValueA",
				IntProp = 42
			};
			var obj2 = new ObjectWithExtendedProps
			{
				StringProp = "stringValueB",
				IntProp = 6,
				BoolProp = true,
				LoopProperty = expected
			};
			expected.LoopProperty = obj2;

			var actual = _serializer.Deserialize<ObjectWithExtendedProps>(json);

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.StringProp, actual.StringProp);
			Assert.IsNotNull(actual.LoopProperty);
			Assert.AreEqual(expected.LoopProperty.StringProp, actual.LoopProperty.StringProp);
			Assert.IsNotNull(actual.LoopProperty.LoopProperty);
			Assert.AreSame(actual, actual.LoopProperty.LoopProperty);

		}
		#endregion

		#region Serialize Tests
		[TestMethod]
		public void Serialize_RegisteredType_Successful()
		{
			var obj = TimeSpan.FromDays(1);
			JsonValue expected = TimeSpan.FromDays(1).ToString();
			var actual = _serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeDefaultOptions_Successful()
		{
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.ToString("s");
			var actual = _serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeJavaFormat_Successful()
		{
			_serializer.Options = new JsonSerializerOptions
			                      	{
			                      		DateTimeSerializationFormat = DateTimeSerializationFormat.JavaConstructor
			                      	};
			var obj = DateTime.Today;
			JsonValue expected = string.Format("/Date({0})/", DateTime.Today.Ticks / TimeSpan.TicksPerMillisecond);
			var actual = _serializer.Serialize(obj);
			_serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeMilliseconds_Successful()
		{
			_serializer.Options = new JsonSerializerOptions
			                      	{
			                      		DateTimeSerializationFormat = DateTimeSerializationFormat.Milliseconds
			                      	};
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.Ticks / TimeSpan.TicksPerMillisecond;
			var actual = _serializer.Serialize(obj);
			_serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Basic_Successful()
		{
			var obj = new ObjectWithBasicProps
						{
							StringProp = "stringValue",
							IntProp = 42,
							DoubleProp = 6.0,
							BoolProp = true
						};
			JsonValue expected = new JsonObject
									{
										{"StringProp", "stringValue"},
										{"IntProp", 42},
										{"DoubleProp", 6},
										{"BoolProp", true}
									};
			var actual = _serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_AbstractAndInterface_Successful()
		{
			var obj = new ObjectWithAbstractAndInterfaceProps
						{
							AbstractProp = new DerivedClass {SomeProp = 42},
							InterfaceProp = "test comparable"
						};
			JsonValue expected = new JsonObject
									{
										{"AbstractProp", new JsonObject
															{
																{"#Type", typeof(DerivedClass).AssemblyQualifiedName},
																{"#Value", new JsonObject{{"SomeProp", 42}}}
															}},
										{"InterfaceProp", new JsonObject
															{
																{"#Type", typeof(string).AssemblyQualifiedName},
																{"#Value", "test comparable"}
															}}
									};
			var actual = _serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Nullable_Null_Successful()
		{
			JsonSerializationTypeRegistry.RegisterNullableType<int>();
			int? i = null;
			var expected = JsonValue.Null;
			var actual = _serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Nullable_NonNull_Successful()
		{
			JsonSerializationTypeRegistry.RegisterNullableType<int>();
			int? i = 42;
			JsonValue expected = 42;
			var actual = _serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_IJsonCompatible_Successful()
		{
			var obj = new JsonCompatibleClass("test string", 42);
			var expected = new JsonObject
			               	{
			               		{"StringProp", "test string"},
			               		{"IntProp", 42}
			               	};
			var actual = _serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_List_Successfull()
		{
			JsonSerializationTypeRegistry.RegisterListType<int>();
			var list = new List<int> {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = _serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Dictionary_Successful()
		{
			JsonSerializationTypeRegistry.RegisterDictionaryType<string, double>();
			var dict = new Dictionary<string, double> {{"four", 4}, {"three", 3}, {"five", 5}, {"six", 6}};
			JsonValue expected = new JsonArray
			                     	{
			                     		new JsonObject {{"Key", "four"},{"Value", 4}},
			                     		new JsonObject {{"Key", "three"}, {"Value", 3}},
			                     		new JsonObject {{"Key", "five"}, {"Value", 5}},
			                     		new JsonObject {{"Key", "six"}, {"Value", 6}}
			                     	};
			var actual = _serializer.Serialize(dict);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Queue_Successfull()
		{
			JsonSerializationTypeRegistry.RegisterQueueType<int>();
			var queue = new Queue<int>();
			queue.Enqueue(4);
			queue.Enqueue(3);
			queue.Enqueue(5);
			queue.Enqueue(6);
			JsonValue expected = new JsonArray { 4, 3, 5, 6 };
			var actual = _serializer.Serialize(queue);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Stack_Successfull()
		{
			JsonSerializationTypeRegistry.RegisterStackType<int>();
			var stack = new Stack<int>();
			stack.Push(4);
			stack.Push(3);
			stack.Push(5);
			stack.Push(6);
			JsonValue expected = new JsonArray { 4, 3, 5, 6 };
			var actual = _serializer.Serialize(stack);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void SerializeType_Successfull()
		{
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
			var actual = _serializer.SerializeType<ObjectWithBasicProps>();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DefaultOptions_IgnoresDefaultValues()
		{
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
			var actual = _serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_CustomOptions_SerializesDefaultValues()
		{
			_serializer.Options = new JsonSerializerOptions
			                      	{
			                      		EncodeDefaultValues = true
			                      	};
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
										{"BoolProp", true}
									};
			var actual = _serializer.Serialize(obj);
			_serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_CircularStructure_SerializesWithReference()
		{
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

			var actual = _serializer.Serialize(obj);

			Assert.IsNotNull(actual);
			Assert.AreEqual(JsonValueType.Object, actual.Type);
			Assert.IsTrue(actual.Object.ContainsKey("#Define"));
			Assert.IsTrue(actual.Object.ContainsKey("LoopProperty"));
			Assert.AreEqual(JsonValueType.Object, actual.Object["LoopProperty"].Type);
			Assert.IsTrue(actual.Object["LoopProperty"].Object.ContainsKey("LoopProperty"));
			Assert.AreEqual(JsonValueType.Object, actual.Object["LoopProperty"].Object["LoopProperty"].Type);
			Assert.IsTrue(actual.Object["LoopProperty"].Object["LoopProperty"].Object.ContainsKey("#Ref"));
			Assert.AreEqual(actual.Object["#Define"], actual.Object["LoopProperty"].Object["LoopProperty"].Object["#Ref"]);
		}
		#endregion
	}
}
