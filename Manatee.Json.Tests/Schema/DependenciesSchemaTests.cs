using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class DependenciesSchemaTests
	{
		[Test]
		public void Dependencies_Properties_Valid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.Dependency("prop1", "test");

			var json = new JsonObject
				{
					["prop1"] = 5,
					["test"] = null
				};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Dependencies_Properties_Invalid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.Dependency("prop1", "test");

			var json = new JsonObject
				{
					["prop1"] = 5
				};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void DependentRequired_Valid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.DependentRequired("prop1", "test");

			var json = new JsonObject
				{
					["prop1"] = 5,
					["test"] = null
				};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void DependentRequired_Invalid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.DependentRequired("prop1", "test");

			var json = new JsonObject
				{
					["prop1"] = 5
				};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void Dependencies_Schema_Valid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.Dependency("prop1", new JsonSchema().MinProperties(2));

			var json = new JsonObject
				{
					["prop1"] = 5,
					["test"] = null
				};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Dependencies_Schema_Invalid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.Dependency("prop1", new JsonSchema().MinProperties(2));

			var json = new JsonObject
				{
					["prop1"] = 5
				};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void DependentSchema_Schema_Valid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.DependentSchema("prop1", new JsonSchema().MinProperties(2));

			var json = new JsonObject
				{
					["prop1"] = 5,
					["test"] = null
				};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void DependentSchema_Schema_Invalid()
		{
			var schema = new JsonSchema()
				.Property("prop1", new JsonSchema().Type(JsonSchemaType.Integer))
				.DependentSchema("prop1", new JsonSchema().MinProperties(2));

			var json = new JsonObject
				{
					["prop1"] = 5
				};

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
	}
}
