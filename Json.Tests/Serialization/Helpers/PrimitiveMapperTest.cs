using Manatee.Json.Serialization.Exceptions;
using Manatee.Json.Serialization.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Manatee.Json;

namespace Json.Tests.Serialization.Helpers
{
	
	
	/// <summary>
	///This is a test class for PrimitiveMapperTest and is intended
	///to contain all PrimitiveMapperTest Unit Tests
	///</summary>
	[TestClass()]
	public class PrimitiveMapperTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		#region MapFromJson Tests
		[TestMethod]
		public void MapFromJson_ParameterIsNotPrimitive_ThrowsNotPrimitiveTypeException()
		{
			JsonValue json = new JsonObject();
			try
			{
				PrimitiveMapper.MapFromJson<Uri>(json);
				Assert.Fail("Expected NotPrimitiveTypeException");
			}
			catch (NotPrimitiveTypeException e) { }
			catch (Exception e)
			{
				Assert.Fail(string.Format("Incorrect exception thrown: {0}", e));
			}
		}
		[TestMethod]
		public void MapFromJson_ParameterIsString_ReturnsCorrectString()
		{
			var json = new JsonValue("test");
			var expected = "test";
			var actual = PrimitiveMapper.MapFromJson<string>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsInt_ReturnsCorrectInt()
		{
			var json = new JsonValue(42);
			var expected = 42;
			var actual = PrimitiveMapper.MapFromJson<int>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsDouble_ReturnsCorrectDouble()
		{
			var json = new JsonValue(42.0);
			var expected = 42.0;
			var actual = PrimitiveMapper.MapFromJson<double>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsBool_ReturnsCorrectBool()
		{
			var json = new JsonValue(true);
			var expected = true;
			var actual = PrimitiveMapper.MapFromJson<bool>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsNotConvertibleToString_ValueIsString_ReturnsDefaultString()
		{
			var json = JsonValue.Null;
			var expected = default(string);
			var actual = PrimitiveMapper.MapFromJson<string>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsNotConvertibleToInt_ValueIsInt_ReturnsDefaultInt()
		{
			var json = JsonValue.Null;
			var expected = default(int);
			var actual = PrimitiveMapper.MapFromJson<int>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsNotConvertibleToDouble_ValueIsDouble_ReturnsDefaultDouble()
		{
			var json = JsonValue.Null;
			var expected = default(double);
			var actual = PrimitiveMapper.MapFromJson<double>(json);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapFromJson_ParameterIsNotConvertibleToBool_ValueIsBool_ReturnsDefaultBool()
		{
			var json = JsonValue.Null;
			var expected = default(bool);
			var actual = PrimitiveMapper.MapFromJson<bool>(json);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region MapToJson Tests
		[TestMethod]
		public void MapToJson_MapsStringCorrectly()
		{
			var obj = "test";
			var expected = new JsonValue(obj);
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapToJson_MapsIntCorrectly()
		{
			var obj = 42;
			var expected = new JsonValue(obj);
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapToJson_MapsDoubleCorrectly()
		{
			var obj = 42.0;
			var expected = new JsonValue(obj);
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapToJson_MapsBoolCorrectly()
		{
			var obj = true;
			var expected = new JsonValue(obj);
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapToJson_MapsEnumCorrectly()
		{
			var obj = UriKind.Absolute;
			var expected = new JsonValue((int)obj);
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapToJson_MapsOtherValueTypeToDefaultValue()
		{
			var obj = DateTime.Today;
			var expected = JsonValue.Null;
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void MapToJson_MapsOtherReferenceTypeToNull()
		{
			var obj = new UriBuilder();
			var expected = JsonValue.Null;
			var actual = PrimitiveMapper.MapToJson(obj);
			Assert.AreEqual(expected, actual);
		}
		#endregion
	}
}
