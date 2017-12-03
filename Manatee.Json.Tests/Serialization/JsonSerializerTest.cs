﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests.Serialization
{
	[TestFixture]
	public class JsonSerializerTest
	{
		[Test]
		public void RegisteredType_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = TimeSpan.FromDays(1);
			JsonValue expected = TimeSpan.FromDays(1).ToString();
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DateTimeDefaultOptions_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.ToString("s");
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[Test]
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
			JsonValue expected = $"/Date({DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond})/";
			var actual = serializer.Serialize(obj);
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
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.Ticks/TimeSpan.TicksPerMillisecond;
			var actual = serializer.Serialize(obj);
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
			var obj = DateTime.Today;
			JsonValue expected = DateTime.Today.ToString("yyyy.MM.dd");
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
#if IOS
		[Ignore]
#endif
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
		[Test]
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
		[Test]
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
		[Test]
		public void Nullable_Null_Successful()
		{
			var serializer = new JsonSerializer();
			var expected = JsonValue.Null;
			var actual = serializer.Serialize((int?) null);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Nullable_NonNull_Successful()
		{
			var serializer = new JsonSerializer();
			int? i = 42;
			JsonValue expected = 42;
			var actual = serializer.Serialize(i);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void IJsonSerializable_Successful()
		{
			var serializer = new JsonSerializer();
			var obj = new JsonSerializableClass("test string", 42);
			JsonValue expected = new JsonObject
				{
					{"StringProp", "test string"},
					{"IntProp", 42}
				};
			var actual = serializer.Serialize(obj);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Array_Successful()
		{
			var serializer = new JsonSerializer();
			var list = new[] {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void List_Successful()
		{
			var serializer = new JsonSerializer();
			var list = new List<int> {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void IEnumerable_Successful()
		{
			var serializer = new JsonSerializer();
			var list = (IEnumerable<int>) new List<int> {4, 3, 5, 6};
			JsonValue expected = new JsonArray {4, 3, 5, 6};
			var actual = serializer.Serialize(list);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		[Ignore("Needs to be rewritten.")]
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
		[Test]
		public void Queue_Successful()
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
		[Test]
		public void Stack_Successful()
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
		[Test]
		public void SerializeType_Successful()
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
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
		public void Fields()
		{
			var serializer = new JsonSerializer {Options = {AutoSerializeFields = true}};
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
		[Test]
		public void ObjectWithAllDefaultValues()
		{
			var obj = new ObjectWithExtendedProps();

			var serializer = new JsonSerializer {Options = {AutoSerializeFields = true, EncodeDefaultValues = true}};

			var json = serializer.Serialize(obj);

			Console.WriteLine(json);
		}
		[Test]
		public void NullableWithNonNullDefaultValue()
		{
			JsonValue expected = false;
			var serializer = new JsonSerializer();
			var actual = serializer.Serialize((bool?) false);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GreedySerialization()
		{
			JsonValue expected = new JsonObject
				{
					{"#Type", typeof(ObjectWithBasicProps).AssemblyQualifiedName},
					{"StringProp", "string"},
					{"IntProp", 5},
					{"DoubleProp", 10},
					{"BoolProp", true},
					{"EnumProp", 2},
					{"MapToMe", 1}
				};
			var obj = new ObjectWithBasicProps
				{
					BoolProp = true,
					DoubleProp = 10,
					EnumProp = TestEnum.EnumValueWithDescription,
					IgnoreProp = "ignore",
					IntProp = 5,
					MappedProp = 1,
					StringProp = "string"
				};
			var serializer = new JsonSerializer {Options = {TypeNameSerializationBehavior = TypeNameSerializationBehavior.Auto}};
			var actual = serializer.Serialize<object>(obj);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GreedySerializationWithoutTypeName()
		{
			JsonValue expected = new JsonObject
				{
					{"StringProp", "string"},
					{"IntProp", 5},
					{"DoubleProp", 10},
					{"BoolProp", true},
					{"EnumProp", 2},
					{"MapToMe", 1}
				};
			var obj = new ObjectWithBasicProps
				{
					BoolProp = true,
					DoubleProp = 10,
					EnumProp = TestEnum.EnumValueWithDescription,
					IgnoreProp = "ignore",
					IntProp = 5,
					MappedProp = 1,
					StringProp = "string"
				};
			var serializer = new JsonSerializer();
			var actual = serializer.Serialize<object>(obj);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GreedySerializationDisabled()
		{
			var expected = JsonValue.Null;
			var obj = new ObjectWithBasicProps
				{
					BoolProp = true,
					DoubleProp = 10,
					EnumProp = TestEnum.EnumValueWithDescription,
					IgnoreProp = "ignore",
					IntProp = 5,
					MappedProp = 1,
					StringProp = "string"
				};
			var serializer = new JsonSerializer {Options = {OnlyExplicitProperties = true}};
			var actual = serializer.Serialize<object>(obj);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void NameTransformation()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							SerializationNameTransform = s => new string(s.Reverse().ToArray())
						}
				};
			var obj = new ObjectWithBasicProps
				{
					StringProp = "stringValue",
					IntProp = 42,
					BoolProp = true,
					MappedProp = 4
				};
			JsonValue expected = new JsonObject
				{
					{"porPgnirtS", "stringValue"},
					{"porPtnI", 42},
					{"porPlooB", true},
					{"MapToMe", 4}
				};
			var actual = serializer.Serialize(obj);
			serializer.Options = null;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SerializeDynamic()
		{
			dynamic dyn = new ExpandoObject();
			dyn.StringProp = "string";
			dyn.IntProp = 5;
			dyn.NestProp = new ExpandoObject();
			dyn.NestProp.Value = new ObjectWithBasicProps
				{
					BoolProp = true
				};

			JsonValue expected = new JsonObject
				{
					["StringProp"] = "string",
					["IntProp"] = 5,
					["NestProp"] = new JsonObject
						{
							["Value"] = new JsonObject
								{
									["BoolProp"] = true
								}
						}
				};

			var serializer = new JsonSerializer
				{
					Options =
						{
							TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions
						}
				};
			var json = serializer.Serialize<dynamic>(dyn);

			Assert.AreEqual(expected, json);
		}
		[Test]
		public void SerializeListOfRandomStuff()
		{
			var list = new List<object> { 1, false, "string", new ObjectWithBasicProps { DoubleProp = 5.5 } };

			JsonValue expected = new JsonArray { 1, false, "string", new JsonObject { ["DoubleProp"] = 5.5 } };

			var serializer = new JsonSerializer
				{
					Options =
						{
							TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions
						}
				};
			var json = serializer.Serialize<dynamic>(list);

			Assert.AreEqual(expected, json);
		}
		[Test]
		public void SerializeAnonymous()
		{
			var target = new
				{
					test = 1,
					fail = "no",
					nested = new
						{
							value = true
						}
				};

			JsonValue expected = new JsonObject
				{
					["test"] = 1,
					["fail"] = "no",
					["nested"] = new JsonObject
						{
							["value"] = true
						}
				};

			var seralizer = new JsonSerializer
				{
					Options =
						{
							PropertySelectionStrategy = PropertySelectionStrategy.ReadAndWrite
						}
				};
			var json = seralizer.Serialize(target);

			Assert.AreEqual(expected, json);
		}
		[Test]
		public void SerializeEnumKeyedDictionary()
		{
			var target = new Dictionary<JsonValueType, object>
				{
					[JsonValueType.String] = "yes",
					[JsonValueType.Array] = new List<object> {1},
					[JsonValueType.Number] = 1,
				};

			JsonValue expected = new JsonObject
				{
					["String"] = "yes",
					["Array"] = new JsonArray { 1},
					["Number"] = 1
				};

			var serializer = new JsonSerializer();
			var json = serializer.Serialize(target);

			Assert.AreEqual(expected, json);
		}
		[Test]
		public void SerializeEnumKeyedDictionaryWithTransform()
		{
			var target = new Dictionary<JsonValueType, object>
				{
					[JsonValueType.String] = "yes",
					[JsonValueType.Array] = new List<object> {1},
					[JsonValueType.Number] = 1,
				};

			JsonValue expected = new JsonObject
				{
					["string"] = "yes",
					["array"] = new JsonArray { 1},
					["number"] = 1
				};

			var serializer = new JsonSerializer
				{
					Options =
						{
							SerializationNameTransform = s => s.ToLower()
						}
				};
			var json = serializer.Serialize(target);

			Assert.AreEqual(expected, json);
		}
		[Test]
		public void SerializestringKeyedDictionary()
		{
			var target = new Dictionary<string, object>
				{
					["a string"] = "yes",
					["yes"] = new List<object> {1},
					["hello"] = 1,
				};

			JsonValue expected = new JsonObject
				{
					["a string"] = "yes",
					["yes"] = new JsonArray { 1},
					["hello"] = 1
				};

			var serializer = new JsonSerializer
				{
					Options =
						{
							SerializationNameTransform = s => s.ToLower()
						}
				};
			var json = serializer.Serialize(target);

			Assert.AreEqual(expected, json);
		}
	}
}
