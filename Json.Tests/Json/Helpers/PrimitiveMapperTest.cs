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
 
	File Name:		PrimitiveMapperTest.cs
	Namespace:		Manatee.Tests.Json.Helpers
	Class Name:		PrimitiveMapperTest
	Purpose:		This is a test class for PrimitiveMapper and is intended
					to contain all PrimitiveMapper Unit Tests

***************************************************************************************/
using System;
using Manatee.Json;
using Manatee.Json.Exceptions;
using Manatee.Json.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Tests.Json.Helpers
{
	[TestClass]
	public class PrimitiveMapperTest
	{
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
			catch (NotPrimitiveTypeException) { }
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
