﻿using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class StringSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonString()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnTooShort()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, MinLength = 5};
			var json = (JsonValue) "test";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnLengthEqualsMinLength()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, MinLength = 5};
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnLengthGreaterThanMinLength()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, MinLength = 5};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnTooLong()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, MaxLength = 5};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnLengthEqualsMaxLength()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, MaxLength = 5};
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnLengthLessThanMaxLength()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, MaxLength = 5};
			var json = (JsonValue) "test";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidDateTimeFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.DateTime};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidDateTimeFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.DateTime};
			var json = (JsonValue) "2016-01-25T10:32:02Z";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidEmailFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Email};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidEmailFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Email};
			var json = (JsonValue) "me@you.com";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidEmailFormat2()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Email};
			var json = (JsonValue) "Me@You.net";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidHostNameFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.HostName};
			var json = (JsonValue) "$test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidHostNameFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.HostName};
			var json = (JsonValue) "me.you.com";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidIpv4Format()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Ipv4};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidIpv4Format()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Ipv4};
			var json = (JsonValue) "255.255.1.1";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidIpv6Format()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Ipv6};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidIpv6Format()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Ipv6};
			var json = (JsonValue) "2001:0db8:85a3:0042:1000:8a2e:0370:7334";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnInvalidUriFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Uri};
			var json = (JsonValue) "test123^%";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnValidUriFormat()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Format = StringFormat.Uri};
			var json = (JsonValue) "http://en.wikipedia.org/wiki/ISO_8601";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsErrorOnPatternNonMatch()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Pattern = "^[0-9_]+$"};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValidOnPatternMatch()
		{
			var schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String, Pattern = "^[0-9_]+$"};
			var json = (JsonValue) "81681_1868";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
