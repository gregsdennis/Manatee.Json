﻿using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests.Serialization
{
	[TestFixture]
	public class JsonDeserializerTest
	{
		[Test]
		public void RegisteredType_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = TimeSpan.FromDays(1).ToString();
			var expected = TimeSpan.FromDays(1);
			var actual = serializer.Deserialize<TimeSpan>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DateTimeDefaultOptions_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = DateTime.Today.ToString("");
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DateTimeJavaFormat_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options = new JsonSerializerOptions
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.JavaConstructor
						}
				};
			JsonValue json = $"/Date({DateTime.Today.Ticks / TimeSpan.TicksPerMillisecond})/";
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DateTimeMilliseconds_Successful()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DateTimeSerializationFormat = DateTimeSerializationFormat.Milliseconds
						}
				};
			JsonValue json = DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond;
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[Test]
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
			JsonValue json = DateTime.Today.ToString("yyyy.MM.dd");
			var expected = DateTime.Today;
			var actual = serializer.Deserialize<DateTime>(json);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Basic_Successful()
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
		[Test]
		public void BasicWithNamedEnum_Successful()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
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
		[Test]
		public void BasicWithNamedEnumWithDescription_Successful()
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
		[Test]
		public void AbstractAndInterfacePropsWithoutMap_Successful()
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
		[Test]
		public void AbstractAndInterfacePropsWithMap_Successful()
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
			serializer.AbstractionMap.Map<AbstractClass, DerivedClass>();
			serializer.AbstractionMap.Map<IInterface, ImplementationClass>();
			var actual = serializer.Deserialize<ObjectWithAbstractAndInterfaceProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void AbstractClass_Successful()
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
		[Test]
		public void AbstractClassWithMap_Successful()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject
				{
					{"SomeProp", 42},
					{"NewProp", "test"}
				};
			AbstractClass expected = new DerivedClass
				{
					SomeProp = 42,
					NewProp = "test"
				};
			serializer.AbstractionMap.Map<AbstractClass, DerivedClass>();

			var actual = serializer.Deserialize<AbstractClass>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Interface_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"#Type", typeof (ImplementationClass).AssemblyQualifiedName},
					{"RequiredProp", "test"}
				};
			IInterface expected = new ImplementationClass {RequiredProp = "test"};

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void InterfaceWithoutMap_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"RequiredProp", "test"}
				};
			IInterface expected = new ImplementationClass {RequiredProp = "test"};

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected.RequiredProp, actual.RequiredProp);
			Assert.AreNotEqual(typeof (ImplementationClass), actual.GetType());
		}
		[Test]
		public void InterfaceWithMap_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"RequiredProp", "test"}
				};
			IInterface expected = new ImplementationClass {RequiredProp = "test"};
			serializer.AbstractionMap.Map<IInterface, ImplementationClass>();

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void InterfaceWithMapToIJsonSerializableImplementation_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonObject
				{
					{"requiredProp", "test"}
				};
			IInterface expected = new JsonSerializableImplementationClass {RequiredProp = "test"};
			serializer.AbstractionMap.Map<IInterface, JsonSerializableImplementationClass>();

			var actual = serializer.Deserialize<IInterface>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Nullable_Null_Successful()
		{
			var serializer = new JsonSerializer();
			var json = JsonValue.Null;
			var actual = serializer.Deserialize<int?>(json);
			Assert.AreEqual(null, actual);
		}
		[Test]
		public void Nullable_NonNull_Successful()
		{
			var serializer = new JsonSerializer();
			int? expected = 42;
			JsonValue json = 42;
			var actual = serializer.Deserialize<int?>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Array_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new[] {4, 3, 5, 6};
			var actual = serializer.Deserialize<int[]>(json);
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
		[Test]
		public void List_Successful()
		{
			var serializer = new JsonSerializer();
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new List<int> {4, 3, 5, 6};
			var actual = serializer.Deserialize<List<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
		[Test]
		public void IEnumerable_Successful()
		{
			var serializer = new JsonSerializer();
			serializer.AbstractionMap.MapGeneric(typeof (IEnumerable<>), typeof (List<>));
			JsonValue json = new JsonArray {4, 3, 5, 6};
			var expected = new List<int> {4, 3, 5, 6};
			var actual = serializer.Deserialize<IEnumerable<int>>(json);
			Assert.AreEqual(expected.Count, actual.Count());
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual.ElementAt(i));
			}
		}
		[Test]
		public void Dictionary_Successful()
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
		[Test]
		public void Queue_Successful()
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
		[Test]
		public void Stack_Successful()
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
		[Test]
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
		[Test]
		public void DefaultOptions_IgnoresExtraProperties()
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
		[Test]
		public void CustomOptions_ThrowsTypeDoesNotContainPropertyException()
		{
			Assert.Throws<TypeDoesNotContainPropertyException>(() =>
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
					finally
					{
						serializer.Options = null;
					}
				});
		}
		[Test]
		public void CircularStructure_MaintainsReferences()
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
#if !IOS
		[Test]
		public void UnimplementedInterface_ReturnsRunTimeImplementation()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject {{"RequiredProp", "test"}};
			serializer.AbstractionMap.RemoveMap<IInterface>();
			IInterface expected = new ImplementationClass {RequiredProp = "test"};

			var actual = serializer.Deserialize<IInterface>(json);

			Assert.IsNotNull(actual);
			Assert.AreNotEqual(expected.GetType(), actual.GetType());
			Assert.AreEqual(expected.RequiredProp, actual.RequiredProp);
		}
#endif
		[Test]
		public void Fields()
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
		[Test]
		public void MapGenericAbstraction_Interface_Success()
		{
			var serializer = new JsonSerializer();
			serializer.AbstractionMap.MapGeneric(typeof (IFace<>), typeof (Impl<>));

			var json = new JsonObject {{"Value", 1}};
			var value = serializer.Deserialize<IFace<int>>(json);

			Assert.AreEqual(typeof (Impl<int>), value.GetType());
		}
		[Test]
		public void MapGenericAbstraction_BaseClass_Success()
		{
			var serializer = new JsonSerializer();
			serializer.AbstractionMap.MapGeneric(typeof (Impl<>), typeof (Derived<>));

			var json = new JsonObject {{"Value", 1}};
			var value = serializer.Deserialize<Impl<int>>(json);

			Assert.AreEqual(typeof (Derived<int>), value.GetType());
		}
		[Test]
		public void MapGenericAbstraction_WithOverride_Success()
		{
			var serializer = new JsonSerializer();
			serializer.AbstractionMap.MapGeneric(typeof (Impl<>), typeof (Derived<>));
			serializer.AbstractionMap.Map<Impl<string>, Derived<string>>();

			var json = new JsonObject {{"Value", 1}};
			var value = serializer.Deserialize<Impl<int>>(json);

			Assert.AreEqual(typeof (Derived<int>), value.GetType());
		}
		[Test]
		public void NameTransformation()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							DeserializationNameTransform = s => new string(s.Reverse().ToArray())
						}
				};
			var json = new JsonObject
				{
					{"porPgnirtS", "stringValue"},
					{"porPtnI", 42},
					{"porPlooB", true},
						{"MapToMe", 4}
				};
			var expected = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					BoolProp = true,
					MappedProp = 4
				};
			var actual = serializer.Deserialize<ObjectWithBasicProps>(json);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DeserializeDynamic()
		{
			var json = new JsonObject
				{
					["StringProp"] = "string",
					["IntProp"] = 5,
					["NestProp"] = new JsonObject
						{
							["Value"] = false
						}
				};
			var serializer = new JsonSerializer();

			var dyn = serializer.Deserialize<dynamic>(json);

			Assert.AreEqual("string", dyn.StringProp);
			Assert.AreEqual(5, dyn.IntProp);
			Assert.AreEqual(false, dyn.NestProp.Value);
		}
		[Test]
		public void DeserializeListOfRandomStuff()
		{
			JsonValue json = new JsonArray { 1, false, "string", new JsonObject { ["DoubleProp"] = 5.5 } };

			var serializer = new JsonSerializer
				{
					Options =
						{
							TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions
						}
				};
			var actual = serializer.Deserialize<dynamic>(json);

			Assert.AreEqual(1, actual[0]);
			Assert.AreEqual(false, actual[1]);
			Assert.AreEqual("string", actual[2]);
			Assert.AreEqual(5.5, actual[3].DoubleProp);
		}
	}
}
