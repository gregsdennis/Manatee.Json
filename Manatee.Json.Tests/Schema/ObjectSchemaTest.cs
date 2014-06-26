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
 
	File Name:		ObjectSchemaTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		ObjectSchemaTest
	Purpose:		Tests for ObjectSchema.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class ObjectSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonObject()
		{
			var schema = new ObjectSchema();
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnRequiredPropertyMissing()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema(),
									IsRequired = true
								}
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnOptionalPropertyMissing()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						}
				};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidProperty()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						}
				};
			var json = new JsonObject {{"test1", 1}};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnAllValidProperties()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						}
				};
			var json = new JsonObject {{"test1", "value"}};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidPatternProperty()
		{
			var schema = new ObjectSchema
			{
				Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						},
				AdditionalProperties = AdditionalProperties.False,
				PatternProperties = new Dictionary<Regex, IJsonSchema>
						{
							{new Regex("[0-9]"), new StringSchema()}
						}
			};
			var json = new JsonObject { { "test1", "value" }, { "test2", 2 } };

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnUnmatchedPatternProperty()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, IJsonSchema>
						{
							{new Regex("[0-9]"), new StringSchema()}
						}
				};
			var json = new JsonObject {{"test1", "value"}, {"test", "value"}};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidAdditionalProperty()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						},
					AdditionalProperties = new AdditionalProperties {Definition = new StringSchema()}
				};
			var json = new JsonObject {{"test1", "value"}, {"test", 1}};

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidAdditionalProperty()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						},
					AdditionalProperties = new AdditionalProperties {Definition = new StringSchema()}
				};
			var json = new JsonObject {{"test1", "value"}, {"test", "value"}};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidPatternProperty()
		{
			var schema = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("test1")
								{
									Type = new StringSchema()
								}
						},
					AdditionalProperties = AdditionalProperties.False,
					PatternProperties = new Dictionary<Regex, IJsonSchema>
						{
							{new Regex("[0-9]"), new IntegerSchema()}
						}
				};
			var json = new JsonObject {{"test1", "value"}, {"test2", 2}};

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
