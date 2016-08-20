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
 
	File Name:		NumberSchemaTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		NumberSchemaTest
	Purpose:		Tests for NumberSchema.

***************************************************************************************/

using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class NumberSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonNumber()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnLessThanMinimum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Minimum = 5};
			var json = (JsonValue) 4;

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnMoreThanMinimum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Minimum = 5};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnEqualsExclusiveMinimum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Minimum = 5, ExclusiveMinimum = true};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnMoreThanExclusiveMinimum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Minimum = 5, ExclusiveMinimum = true};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnMoreThanMaximum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Maximum = 5};
			var json = (JsonValue) 10;

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnLessThanMaximum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Maximum = 5};
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnEqualsExclusiveMaximum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Maximum = 5, ExclusiveMaximum = true};
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnLessThanExclusiveMaximum()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, Maximum = 5, ExclusiveMaximum = true};
			var json = (JsonValue) 3;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnMultipleOf_Positive()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) 7.5;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnMultipleOf_Negative()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) (-7.5);

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnMultipleOf_Zero()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) 0;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsInvalicOnMultipleOf()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number, MultipleOf = 2.5};
			var json = (JsonValue) 16;

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
	}
}
