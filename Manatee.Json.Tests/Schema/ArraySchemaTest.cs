/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ArraySchemaTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		ArraySchemaTest
	Purpose:		Tests for ArraySchema.

***************************************************************************************/

using System;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class ArraySchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonArray()
		{
			var schema = new ArraySchema();
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnTooFewItems()
		{
			var schema = new ArraySchema {MinItems = 5};
			var json = new JsonArray {1, "string"};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountEqualsMinItems()
		{
			var schema = new ArraySchema { MinItems = 2 };
			var json = new JsonArray { 1, "string" };

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountGreaterThanMinItems()
		{
			var schema = new ArraySchema { MinItems = 2 };
			var json = new JsonArray { 1, "string", false };

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnTooManyItems()
		{
			var schema = new ArraySchema {MaxItems = 5};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 2};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountEqualsMaxItems()
		{
			var schema = new ArraySchema { MaxItems = 5 };
			var json = new JsonArray { 1, "string", false, Math.PI, JsonValue.Null };

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnCountLessThanMaxItems()
		{
			var schema = new ArraySchema { MaxItems = 5 };
			var json = new JsonArray { 1, "string", false };

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnDuplicateItems()
		{
			var schema = new ArraySchema {UniqueItems = true};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null, 1};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnUniqueItems()
		{
			var schema = new ArraySchema {UniqueItems = true};
			var json = new JsonArray {1, "string", false, Math.PI, JsonValue.Null};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidItems()
		{
			var schema = new ArraySchema { Items = new StringSchema() };
			var json = new JsonArray { 1, "string" };

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidItems()
		{
			var schema = new ArraySchema { Items = new StringSchema() };
			var json = new JsonArray { "start", "string" };

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
