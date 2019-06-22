using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ContainsSchemaTests
	{
		[Test]
		public void Contains_Valid()
		{
			var schema = new JsonSchema()
				.Contains(new JsonSchema().Type(JsonSchemaType.Integer));

			JsonValue json = new JsonArray {"string", true, 5, 10.6};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Contains_NonArray_Valid()
		{
			var schema = new JsonSchema()
				.Contains(new JsonSchema().Type(JsonSchemaType.Integer));

			JsonValue json = new JsonObject {["string"] = 5};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void MinContains_Valid()
		{
			var schema = new JsonSchema()
				.Contains(new JsonSchema().Type(JsonSchemaType.Integer))
				.MinContains(2);

			JsonValue json = new JsonArray { "string", true, 5, 10.6, 8 };

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void MinContains_Invalid()
		{
			var schema = new JsonSchema()
				.Contains(new JsonSchema().Type(JsonSchemaType.Integer))
				.MinContains(2);

			JsonValue json = new JsonArray { "string", true, 5, 10.6 };

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void MaxContains_Valid()
		{
			var schema = new JsonSchema()
				.Contains(new JsonSchema().Type(JsonSchemaType.Integer))
				.MaxContains(1);

			JsonValue json = new JsonArray { "string", true, 5, 10.6 };

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void MaxContains_Invalid()
		{
			var schema = new JsonSchema()
				.Contains(new JsonSchema().Type(JsonSchemaType.Integer))
				.MaxContains(1);

			JsonValue json = new JsonArray { "string", true, 5, 10.6, 8 };

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
	}
}
