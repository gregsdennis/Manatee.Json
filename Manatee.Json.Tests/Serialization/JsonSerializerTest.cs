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
					to contain all JsonSerializer Unit Tests

***************************************************************************************/

using System;
using System.Collections.Generic;
using Manatee.Json.Serialization;
using Manatee.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Serialization
{
	[TestClass]
	public class JsonSerializerTest
	{
		#region Deserialize Tests
		[TestMethod]
		public void Deserialize_RegisteredType_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = TimeSpan.FromDays(1).ToString();
			var expected = TimeSpan.FromDays(1);
			var actual = serializer.Deserialize<TimeSpan>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeDefaultOptions_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = DateTime.Today.ToString("");
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeJavaFormat_Successful()
		{
			var serializer = new JsonSerializer();
			serializer.Options = new JsonSerializerOptions
				{
					DateTimeSerializationFormat = DateTimeSerializationFormat.JavaConstructor
				};
			JsonValue json = string.Format("/Date({0})/", DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond);
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeMilliseconds_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.Milliseconds
						}
				};
			JsonValue json = DateTime.Today.Ticks / TimeSpan.TicksPerMillisecond;
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_DateTimeCustom_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.Custom,
							CustomDateTimeSerializationFormat = "yyyy.MM.dd"
						}
				};
			JsonValue json = DateTime.Today.ToString("yyyy.MM.dd");
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Basic_Successful()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", 1},
					{"MapToMe", 4}
				};
			var expected = new ObjectWithBasicProps
			{
				StringProp = "stringValue",
				IntProp = 42,
				DoubleProp = 6.0,
				BoolProp = true,
				EnumProp = TestEnum.BasicEnumValue,
				MappedProp = 4
			};
			var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_BasicWithNamedEnum_Successful()
		{
			var serializer = new JsonSerializer();
			serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;
			var json = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "BasicEnumValue"},
					{"MapToMe", 4}
				};
			var expected = new ObjectWithBasicProps
			{
				StringProp = "stringValue",
				IntProp = 42,
				DoubleProp = 6.0,
				BoolProp = true,
				EnumProp = TestEnum.BasicEnumValue,
				MappedProp = 4
			};
			var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_BasicWithNamedEnumWithDescription_Successful()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var json = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", "enum_value_with_description"},
					{"MapToMe", 4}
				};
			var expected = new ObjectWithBasicProps
			{
				StringProp = "stringValue",
				IntProp = 42,
				DoubleProp = 6.0,
				BoolProp = true,
				EnumProp = TestEnum.EnumValueWithDescription,
				MappedProp = 4
			};
			var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_AbstractAndInterfacePropsWithoutMap_Successful()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
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
								{"RequiredProp", "test"}
							}
					}
				};
			var expected = new ObjectWithAbstractAndInterfaceProps
			               	{
			               		AbstractProp = new DerivedClass {SomeProp = 42},
			               		InterfaceProp = new ImplementationClass {RequiredProp = "test"}
			               	};
			var actual = serializer.Deserialize<ObjectWithAbstractAndInterfaceProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_AbstractAndInterfacePropsWithMap_Successful()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
				{
					{"AbstractProp", new JsonObject {{"SomeProp", 42}}},
					{"InterfaceProp", new JsonObject {{"RequiredProp", "test"}}}
				};
			var expected = new ObjectWithAbstractAndInterfaceProps
				{
					AbstractProp = new DerivedClass {SomeProp = 42},
					InterfaceProp = new ImplementationClass {RequiredProp = "test"}
				};
			JsonSerializationAbstractionMap.Map<AbstractClass, DerivedClass>();
			JsonSerializationAbstractionMap.Map<IInterface, ImplementationClass>();
			var actual = serializer.Deserialize<ObjectWithAbstractAndInterfaceProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_AbstractClass_Successful()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
				{
					{"#Type", typeof (DerivedClass).AssemblyQualifiedName},
					{"SomeProp", 42},
					{"NewProp", "test"}
				};
			AbstractClass expected = new DerivedClass
				{
					SomeProp = 42,
					NewProp = "test"
				};

			var actual = serializer.Deserialize<AbstractClass>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_AbstractClassWithMap_Successful()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
				{
					//{"#Type", typeof (DerivedClass).AssemblyQualifiedName},
					{"SomeProp", 42},
					{"NewProp", "test"}
				};
			AbstractClass expected = new DerivedClass
			{
				SomeProp = 42,
				NewProp = "test"
			};
			JsonSerializationAbstractionMap.Map<AbstractClass, DerivedClass>();

			var actual = serializer.Deserialize<AbstractClass>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Interface_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"#Type", typeof (ImplementationClass).AssemblyQualifiedName},
					{"RequiredProp", "test"}
				};
			IInterface expected = new ImplementationClass { RequiredProp = "test" };

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_InterfaceWithoutMap_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"RequiredProp", "test"}
				};
			IInterface expected = new ImplementationClass { RequiredProp = "test" };

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected.RequiredProp, actual.RequiredProp);
			Assert.AreNotEqual(typeof (ImplementationClass), actual.GetType());
		}
		[TestMethod]
		public void Deserialize_InterfaceWithMap_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"RequiredProp", "test"}
				};
			IInterface expected = new ImplementationClass { RequiredProp = "test" };
			JsonSerializationAbstractionMap.Map<IInterface, ImplementationClass>();

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_InterfaceWithMapToIJsonSerializableImplementation_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"requiredProp", "test"}
				};
			IInterface expected = new JsonSerializableImplementationClass { RequiredProp = "test" };
			JsonSerializationAbstractionMap.Map<IInterface, JsonSerializableImplementationClass>();

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Nullable_Null_Successful()
		{
			var serializer = new JsonSerializer();
			int? expected = null;
			var json = JsonValue.Null;
			var actual = serializer.Deserialize<int?>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Nullable_NonNull_Successful()
		{
			var serializer = new JsonSerializer();
			int? expected = 42;
			JsonValue json = 42;
			var actual = serializer.Deserialize<int?>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_Array_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonArray { 4, 3, 5, 6 };
			var expected = new[] { 4, 3, 5, 6 };
			var actual = serializer.Deserialize<int[]>(json);
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
		[TestMethod]
		public void Deserialize_List_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonArray { 4, 3, 5, 6 };
			var expected = new List<int> { 4, 3, 5, 6 };
			var actual = serializer.Deserialize<List<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
		[TestMethod]
		public void Deserialize_Dictionary_Successful()
		{
			var serializer = new JsonSerializer();
			var expected = new Dictionary<string, double> {{"four", 4}, {"three", 3}, {"five", 5}, {"six", 6}};
			JsonValue json = new JsonArray
				{
					new JsonObject {{"Key", "four"}, {"Value", 4}},
					new JsonObject {{"Key", "three"}, {"Value", 3}},
					new JsonObject {{"Key", "five"}, {"Value", 5}},
					new JsonObject {{"Key", "six"}, {"Value", 6}}
				};
			var actual = serializer.Deserialize<Dictionary<string, double>>(json);
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
			var serializer = new JsonSerializer();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new Queue<int>();
			expected.Enqueue(4);
			expected.Enqueue(3);
			expected.Enqueue(5);
			expected.Enqueue(6);
			var actual = serializer.Deserialize<Queue<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected.ToArray()[i], actual.ToArray()[i]);
			}
		}
		[TestMethod]
		public void Deserialize_Stack_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new Stack<int>();
			expected.Push(4);
			expected.Push(3);
			expected.Push(5);
			expected.Push(6);
			var actual = serializer.Deserialize<Stack<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected.ToArray()[i], actual.ToArray()[i]);
			}
		}
		[TestMethod]
		public void DeserializeType_Successfull()
		{
			var serializer = new JsonSerializer();
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
			serializer.DeserializeType<ObjectWithBasicProps>(json);
			Assert.AreEqual(ObjectWithBasicProps.StaticStringProp, stringProp);
			Assert.AreEqual(ObjectWithBasicProps.StaticIntProp, intProp);
			Assert.AreEqual(ObjectWithBasicProps.StaticDoubleProp, doubleProp);
			Assert.AreEqual(ObjectWithBasicProps.StaticBoolProp, boolProp);
		}
		[TestMethod]
		public void Deserialize_DefaultOptions_IgnoresExtraProperties()
		{
			var serializer = new JsonSerializer();
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
			var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_CustomOptions_ThrowsTypeDoesNotContainPropertyException()
		{
			var serializer = new JsonSerializer();
			try
			{
				serializer.Options = new JsonSerializerOptions
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
				var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
				Assert.Fail("Did not throw exception.");
			}
			catch (TypeDoesNotContainPropertyException) { }
			catch (AssertFailedException)
			{
				throw;
			}
			catch (Exception e)
			{
				Assert.Fail("Threw {0}.", e.GetType());
			}
			finally
			{
				serializer.Options = null;
			}
		}
		[TestMethod]
		public void Deserialize_CircularStructure_MaintainsReferences()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
				{
					{
						"LoopProperty", new JsonObject
							{
								{"LoopProperty", new JsonObject {{"#Ref", "f3a2993b-9b0c-4296-872e-95a9210295f4"}}},
								{"StringProp", "stringValueB"},
								{"IntProp", 6},
								{"BoolProp", true}
							}
					},
					{"StringProp", "stringValueA"},
					{"IntProp", 42},
					{"#Def", "f3a2993b-9b0c-4296-872e-95a9210295f4"}
				};
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

			var actual = serializer.Deserialize<ObjectWithExtendedProps>(json);

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.StringProp, actual.StringProp);
			Assert.IsNotNull(actual.LoopProperty);
			Assert.AreEqual(expected.LoopProperty.StringProp, actual.LoopProperty.StringProp);
			Assert.IsNotNull(actual.LoopProperty.LoopProperty);
			Assert.AreSame(actual, actual.LoopProperty.LoopProperty);
			Assert.AreEqual(0, serializer.SerializationMap.Count);
		}
		// This test fails when all tests are run together because the JsonSerializationAbstractionMap
		// is static and one of the other tests has registered ImplementationClass to be used
		// for IInterface, and VS is running the tests concurrently.
		[TestMethod]
		public void Deserialize_UnimplementedInterface_ReturnsRunTimeImplementation()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject {{"RequiredProp", "test"}};
			IInterface expected = new ImplementationClass {RequiredProp = "test"};

			var actual = serializer.Deserialize<IInterface>(json);

			Assert.IsNotNull(actual);
			Assert.AreNotEqual(expected.GetType(), actual.GetType());
			Assert.AreEqual(expected.RequiredProp, actual.RequiredProp);
		}
		[TestMethod]
		public void Deserialize_Fields()
		{
			var serializer = new JsonSerializer {Options = {AutoSerializeFields = true}};
			var json = new JsonObject
				{
					{"StringProp", "stringValue"},
					{"IntProp", 42},
					{"DoubleProp", 6},
					{"BoolProp", true},
					{"EnumProp", 1},
					{"MapToMe", 4},
					{"Field", "test"}
				};
			var expected = new ObjectWithBasicProps
			{
				StringProp = "stringValue",
				IntProp = 42,
				DoubleProp = 6.0,
				BoolProp = true,
				EnumProp = TestEnum.BasicEnumValue,
				MappedProp = 4,
				Field = "test"
			};
			var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Deserialize_MapGenericAbstraction_Interface_Success()
		{
			JsonSerializationAbstractionMap.MapGeneric(typeof(IFace<>), typeof(Impl<>));

			var serializer = new JsonSerializer();
			var json = new JsonObject { { "Value", 1 } };
			var value = serializer.Deserialize<IFace<int>>(json);

			Assert.AreEqual(typeof (Impl<int>), value.GetType());
		}
		[TestMethod]
		public void Deserialize_MapGenericAbstraction_BaseClass_Success()
		{
			JsonSerializationAbstractionMap.MapGeneric(typeof(Impl<>), typeof(Derived<>));

			var serializer = new JsonSerializer();
			var json = new JsonObject { { "Value", 1 } };
			var value = serializer.Deserialize<Impl<int>>(json);

			Assert.AreEqual(typeof(Derived<int>), value.GetType());
		}
		[TestMethod]
		public void Deserialize_MapGenericAbstraction_WithOverride_Success()
		{
			JsonSerializationAbstractionMap.MapGeneric(typeof(Impl<>), typeof(Derived<>));
			JsonSerializationAbstractionMap.Map<Impl<string>, Derived<string>>();

			var serializer = new JsonSerializer();
			var json = new JsonObject { { "Value", 1 } };
			var value = serializer.Deserialize<Impl<int>>(json);

			Assert.AreEqual(typeof(Derived<int>), value.GetType());
		}
		#endregion

		#region Serialize Tests
		[TestMethod]
		public void Serialize_RegisteredType_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = TimeSpan.FromDays(1);
			JsonValue expected = TimeSpan.FromDays(1).ToString();
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeDefaultOptions_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.ToString("s");
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeJavaFormat_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.JavaConstructor
						}
				};
			var obj = DateTime.Today;
			JsonValue expected = string.Format("/Date({0})/", DateTime.Today.Ticks / TimeSpan.TicksPerMillisecond);
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeMilliseconds_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.Milliseconds
						}
				};
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.Ticks / TimeSpan.TicksPerMillisecond;
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_DateTimeCustom_Successful()
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
		public void Serialize_Basic_Successful()
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
		public void Serialize_BasicWithNamedEnum_Successful()
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
		public void Serialize_BasicWithNamedEnumWithDescription_Successful()
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
		public void Serialize_BasicWithNamedFlagsEnum_Successful()
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
		public void Serialize_BasicWithNamedFlagsEnumWithDescription_Successful()
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
		public void Serialize_BasicWithNamedMultivalueFlagsEnumWithDescription_Successful()
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
		public void Serialize_AbstractAndInterfaceProps_Successful()
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
		public void Serialize_AbstractClass_Successful()
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
		public void Serialize_Interface_Successful()
		{
			var serializer = new JsonSerializer();
			IInterface obj = new ImplementationClass { RequiredProp = "test" };
			JsonValue expected = new JsonObject
				{
					{"#Type", typeof (ImplementationClass).AssemblyQualifiedName},
					{"RequiredProp", "test"}
				};

			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Nullable_Null_Successful()
		{
			var serializer = new JsonSerializer();
			int? i = null;
			var expected = JsonValue.Null;
			var actual = serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Nullable_NonNull_Successful()
		{
			var serializer = new JsonSerializer();
			int? i = 42;
			JsonValue expected = 42;
			var actual = serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_IJsonSerializable_Successful()
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
		public void Serialize_Array_Successfull()
		{
			var serializer = new JsonSerializer();
			var list = new[] {4, 3, 5, 6};
			JsonValue expected = new JsonArray { 4, 3, 5, 6 };
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_List_Successfull()
		{
			var serializer = new JsonSerializer();
			var list = new List<int> { 4, 3, 5, 6 };
			JsonValue expected = new JsonArray { 4, 3, 5, 6 };
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Dictionary_Successful()
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
		public void Serialize_Queue_Successfull()
		{
			var serializer = new JsonSerializer();
			var queue = new Queue<int>();
			queue.Enqueue(4);
			queue.Enqueue(3);
			queue.Enqueue(5);
			queue.Enqueue(6);
			JsonValue expected = new JsonArray { 4, 3, 5, 6 };
			var actual = serializer.Serialize(queue);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Serialize_Stack_Successfull()
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
		public void Serialize_DefaultOptions_IgnoresDefaultValues()
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
		public void Serialize_CustomOptions_SerializesDefaultValues()
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
		public void Serialize_CircularStructure_SerializesWithReference()
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
		public void Serialize_Fields()
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
		public void Serialize_ObjectWithAllDefaultValues()
		{
			var obj = new ObjectWithExtendedProps();

			var serializer = new JsonSerializer {Options = {AutoSerializeFields = true, EncodeDefaultValues = true}};

			var json = serializer.Serialize(obj);

			Console.WriteLine(json);
		}
		[TestMethod]
		public void Serialize_NullableWithNonNullDefaultValue()
		{
			bool? obj = false;
			JsonValue expected = false;
			var serializer = new JsonSerializer();
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		#endregion
	}
}
