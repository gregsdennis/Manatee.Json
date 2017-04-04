using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class XmlExtensionsTest
	{
		#region Number
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToXElement_NumberNullKey_ThrowsArgumentException()
		{
			JsonValue json = 42;
			var actual = json.ToXElement(null);
		}
		[TestMethod]
		public void ToXElement_NumberWithKey_MapsCorrectly()
		{
			var value = 42;
			var key = "aKey";
			JsonValue json = value;
			var expected = new XElement(key, value);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToJson_ElementWithNumberValue_MapsCorrectly()
		{
			var value = 42;
			var key = "aKey";
			var xml = new XElement(key, value);
			JsonValue expected = new JsonObject {{key, value}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		#endregion
		#region String
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToXElement_StringNullKey_ThrowsArgumentException()
		{
			JsonValue json = "a string";
			var actual = json.ToXElement(null);
		}
		[TestMethod]
		public void ToXElement_StringWithKey_MapsCorrectly()
		{
			var value = "a string";
			var key = "aKey";
			JsonValue json = value;
			var expected = new XElement(key, value);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToXElement_NumericStringWithKey_MapsCorrectly()
		{
			var value = "42";
			var key = "aKey";
			JsonValue json = value;
			var expected = new XElement(key, value);
			expected.SetAttributeValue("type", "String");

			var actual = json.ToXElement(key);

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}
		[TestMethod]
		public void ToXElement_BooleanStringWithKey_MapsCorrectly()
		{
			var value = "true";
			var key = "aKey";
			JsonValue json = value;
			var expected = new XElement(key, value);
			expected.SetAttributeValue("type", "String");

			var actual = json.ToXElement(key);

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}
		[TestMethod]
		public void ToJson_ElementWithStringValue_MapsCorrectly()
		{
			var value = "a string";
			var key = "aKey";
			var xml = new XElement(key, value);
			JsonValue expected = new JsonObject {{key, value}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_ElementWithNumericStringValue_MapsCorrectly()
		{
			var value = "42";
			var key = "aKey";
			var xml = new XElement(key, value);
			xml.SetAttributeValue("type", "String");
			JsonValue expected = new JsonObject {{key, value}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_ElementWithBooleanStringValue_MapsCorrectly()
		{
			var value = "true";
			var key = "aKey";
			var xml = new XElement(key, value);
			xml.SetAttributeValue("type", "String");
			JsonValue expected = new JsonObject {{key, value}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		#endregion
		#region Boolean
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToXElement_BooleanNullKey_ThrowsArgumentException()
		{
			JsonValue json = true;
			var actual = json.ToXElement(null);
		}
		[TestMethod]
		public void ToXElement_BooleanWithKey_MapsCorrectly()
		{
			var value = true;
			var key = "aKey";
			JsonValue json = value;
			var expected = new XElement(key, value);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToJson_ElementWithBooleanValue_MapsCorrectly()
		{
			var value = true;
			var key = "aKey";
			var xml = new XElement(key, value);
			JsonValue expected = new JsonObject {{key, value}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		#endregion
		#region Object
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToXElement_ObjectNullKeyMultiplePairs_ThrowsArgumentException()
		{
			JsonValue json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = json.ToXElement(null);
		}
		[TestMethod]
		public void ToXElement_ObjectNullKeySinglePair_MapsCorrectly()
		{
			JsonValue json = new JsonObject {{"root", new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}}}};
			var key = "root";
			var expected = new XElement(key);
			var xml = new XElement("bool", false);
			expected.Add(xml);
			xml = new XElement("int", 42);
			expected.Add(xml);
			xml = new XElement("string", "a string");
			expected.Add(xml);

			var actual = json.ToXElement(null);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToXElement_ObjectWithKey_MapsCorrectly()
		{
			JsonValue json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var key = "aKey";
			var expected = new XElement(key);
			var xml = new XElement("bool", false);
			expected.Add(xml);
			xml = new XElement("int", 42);
			expected.Add(xml);
			xml = new XElement("string", "a string");
			expected.Add(xml);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToJson_SingleRootElementSimpleContents_MapsCorrectly()
		{
			var key = "aKey";
			var xml = new XElement(key);
			var element = new XElement("bool", false);
			xml.Add(element);
			element = new XElement("int", 42);
			xml.Add(element);
			element = new XElement("string", "a string");
			xml.Add(element);
			JsonValue expected = new JsonObject {{"aKey", new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}}}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_MultipleRootElementsSimpleContents_MapsCorrectly()
		{
			var list = new List<XElement>();
			var element = new XElement("bool", false);
			list.Add(element);
			element = new XElement("int", 42);
			list.Add(element);
			element = new XElement("string", "a string");
			list.Add(element);
			JsonValue expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};

			var actual = list.ToJson();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_SingleRootElementComplexContents_MapsCorrectly()
		{
			var key = "aKey";
			var xml = new XElement(key);
			var element = new XElement("bool", false);
			xml.Add(element);
			element = new XElement("int", 42);
			xml.Add(element);
			element = new XElement("object");
			var inner = new XElement("string", "a string");
			element.Add(inner);
			xml.Add(element);
			JsonValue expected = new JsonObject
			                     	{
			                     		{
			                     			"aKey",
			                     			new JsonObject
			                     				{
			                     					{"bool", false},
			                     					{"int", 42},
			                     					{"object", new JsonObject {{"string", "a string"}}}
			                     				}
			                     			}
			                     	};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		#endregion
		#region Array
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToXElement_ArrayNullKey_ThrowsArgumentException()
		{
			JsonValue json = new JsonArray {false, 42, "a string"};
			var actual = json.ToXElement(null);
		}
		[TestMethod]
		public void ToXElement_ArrayWithKey_MapsCorrectly()
		{
			var key = "aKey";
			JsonValue json = new JsonArray {false, 42, "a string"};
			var expected = new XElement(key);
			var xml = new XElement(key, false);
			expected.Add(xml);
			xml = new XElement(key, 42);
			expected.Add(xml);
			xml = new XElement(key, "a string");
			expected.Add(xml);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToXElement_ArrayWithNestedArray_MapsCorrectly()
		{
			var key = "aKey";
			JsonValue json = new JsonArray {false, 42, new JsonArray {"a string"}};
			var expected = new XElement(key);
			var xml = new XElement(key, false);
			expected.Add(xml);
			xml = new XElement(key, 42);
			expected.Add(xml);
			xml = new XElement(key);
			var inner = new XElement(key, "a string");
			xml.Add(inner);
			xml.SetAttributeValue("nest", true);
			expected.Add(xml);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToXElement_ArrayWithNestedArrayWithNestedObjectContainingSameKey_MapsCorrectly()
		{
			var key = "aKey";
			JsonValue json = new JsonArray {false, 42, new JsonArray {"a string", new JsonObject {{key, 6}}}};
			var expected = new XElement(key);
			var xml = new XElement(key, false);
			expected.Add(xml);
			xml = new XElement(key, 42);
			expected.Add(xml);
			xml = new XElement(key);
			var inner = new XElement(key, "a string");
			xml.Add(inner);
			inner = new XElement(key);
			var obj = new XElement(key, 6);
			inner.Add(obj);
			xml.Add(inner);
			xml.SetAttributeValue("nest", true);
			expected.Add(xml);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		#endregion
		#region Null
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ToXElement_NullNullKey_ThrowsArgumentException()
		{
			JsonValue json = true;
			var actual = json.ToXElement(null);
		}
		[TestMethod]
		public void ToXElement_NullWithKey_MapsCorrectly()
		{
			var key = "aKey";
			JsonValue json = JsonValue.Null;
			var expected = new XElement(key);

			var actual = json.ToXElement(key);

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToJson_ElementWithNullValue_MapsCorrectly()
		{
			var key = "aKey";
			var xml = new XElement(key);
			JsonValue expected = new JsonObject {{key, JsonValue.Null}};

			var actual = xml.ToJson();

			Assert.AreEqual(expected, actual);
		}
		#endregion
		#region Complex
		[TestMethod]
		public void ToXElement_ComplexObject_MapsCorrectly()
		{
			JsonValue json = new JsonObject
			           			{
			           				{"string","a string"},
									{"number", 42},
									{"array", new JsonArray{-6, JsonValue.Null, true, new JsonArray{Math.PI, false, "another string"}}},
									{"object", new JsonObject
							           			{
							           				{"string", "yet another string"},
													{"object", new JsonObject{{"number", 263.5},{"array",new JsonArray{1,2,new JsonObject{{"single",1}}}}}}
							           			}}
			           			};
			var expected = new XElement("root");
			var element1 = new XElement("string", "a string");
			expected.Add(element1);
			element1 = new XElement("number", 42);
			expected.Add(element1);
			element1 = new XElement("array", -6);
			expected.Add(element1);
			element1 = new XElement("array");
			expected.Add(element1);
			element1 = new XElement("array", true);
			expected.Add(element1);
			element1 = new XElement("array");
			element1.SetAttributeValue("nest", true);
			var element2 = new XElement("array", Math.PI);
			element1.Add(element2);
			element2 = new XElement("array", false);
			element1.Add(element2);
			element2 = new XElement("array", "another string");
			element1.Add(element2);
			expected.Add(element1);
			element1 = new XElement("object");
			element2 = new XElement("string", "yet another string");
			element1.Add(element2);
			element2 = new XElement("object");
			var element3 = new XElement("number", 263.5);
			element2.Add(element3);
			element3 = new XElement("array", 1);
			element2.Add(element3);
			element3 = new XElement("array", 2);
			element2.Add(element3);
			element3 = new XElement("array");
			var element4 = new XElement("single", 1);
			element3.Add(element4);
			element2.Add(element3);
			element1.Add(element2);
			expected.Add(element1);

			var actual = json.ToXElement("root");

			Assert.IsTrue(XNode.DeepEquals(expected, actual));
		}
		[TestMethod]
		public void ToJson_ComplexElement_MapsCorrectly()
		{
			var list = new List<XElement>();
			var element1 = new XElement("string", "a string");
			list.Add(element1);
			element1 = new XElement("number", 42);
			list.Add(element1);
			element1 = new XElement("array", -6);
			list.Add(element1);
			element1 = new XElement("array");
			list.Add(element1);
			element1 = new XElement("array", true);
			list.Add(element1);
			element1 = new XElement("array");
			element1.SetAttributeValue("nest", true);
			var element2 = new XElement("array", Math.PI);
			element1.Add(element2);
			element2 = new XElement("array", false);
			element1.Add(element2);
			element2 = new XElement("array", "another string");
			element1.Add(element2);
			list.Add(element1);
			element1 = new XElement("object");
			element2 = new XElement("string", "yet another string");
			element1.Add(element2);
			element2 = new XElement("object");
			var element3 = new XElement("number", 263.5);
			element2.Add(element3);
			element3 = new XElement("array", 1);
			element2.Add(element3);
			element3 = new XElement("array", 2);
			element2.Add(element3);
			element3 = new XElement("array");
			var element4 = new XElement("single", 1);
			element3.Add(element4);
			element2.Add(element3);
			element1.Add(element2);
			list.Add(element1);
			JsonValue expected = new JsonObject
			           				{
			           					{"string","a string"},
										{"number", 42},
										{"array", new JsonArray{-6, JsonValue.Null, true, new JsonArray{Math.PI, false, "another string"}}},
										{"object", new JsonObject
							           				{
							           					{"string", "yet another string"},
														{"object", new JsonObject{{"number", 263.5},{"array",new JsonArray{1,2,new JsonObject{{"single",1}}}}}}
							           				}}
			           				};

			var actual = list.ToJson();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RoundTrip_StartingWithJson()
		{
			JsonValue json = new JsonObject
			           			{
			           				{"string","a string"},
									{"number", 42},
									{"array", new JsonArray{-6, JsonValue.Null, true, new JsonArray{Math.PI, false, "another string"}}},
									{"object", new JsonObject
							           			{
							           				{"string", "yet another string"},
													{"object", new JsonObject{{"number", 263.5},{"array",new JsonArray{1,2,new JsonObject{{"single",1}}}}}}
							           			}}
			           			};
			var expected = new JsonObject {{"root", json}};

			var toXElement = json.ToXElement("root");
			var actual = toXElement.ToJson();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RoundTrip_StartingWithXml()
		{
			var expected = XElement.Parse(@"<Requests><Request><SearchCustomerRequest xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://www.manatee.com/services/""><MachineName xmlns=""http://www.manatee.com/services/sub/"">USA02415-2</MachineName><SearchContext xmlns:d2p1=""http://www.manatee.com/services/""><d2p1:CustomerId i:nil=""true"" /><d2p1:EdgeId /><d2p1:EmailAddress /><d2p1:LastName /><d2p1:LoyaltyCardNumber>1234567890123</d2p1:LoyaltyCardNumber><d2p1:MaxRows i:nil=""true"" /><d2p1:PhoneNumber /><d2p1:ZipCode /></SearchContext></SearchCustomerRequest></Request></Requests>");
			var expectedJson = JsonValue.Parse(@"{""Requests"":{""Request"":{""SearchCustomerRequest"":[{""-xmlns:i"":""http:\/\/www.w3.org\/2001\/XMLSchema-instance"",""-xmlns"":""http:\/\/www.manatee.com\/services\/""},{""MachineName"":[{""-xmlns"":""http:\/\/www.manatee.com\/services\/sub\/""},""USA02415-2""],""d2p1:SearchContext"":[{""-xmlns:d2p1"":""http:\/\/www.manatee.com\/services\/""},{""d2p1:CustomerId"":[{""-i:nil"":true},null],""d2p1:EdgeId"":null,""d2p1:EmailAddress"":null,""d2p1:LastName"":null,""d2p1:LoyaltyCardNumber"":1234567890123,""d2p1:MaxRows"":[{""-i:nil"":true},null],""d2p1:PhoneNumber"":null,""d2p1:ZipCode"":null}]}]}}}");

			var toJson = expected.ToJson();
			Assert.AreEqual(expectedJson, toJson);

			var actual = toJson.ToXElement(null);

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}
		#endregion
	}
}
