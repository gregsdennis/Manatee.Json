using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	// TODO: Convert this to test all of the schema generations
	[TestFixture]
	public class StringSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonString()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String};
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnTooShort()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, MinLength = 5};
			var json = (JsonValue) "test";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthEqualsMinLength()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, MinLength = 5};
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthGreaterThanMinLength()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, MinLength = 5};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnTooLong()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, MaxLength = 5};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthEqualsMaxLength()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, MaxLength = 5};
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthLessThanMaxLength()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, MaxLength = 5};
			var json = (JsonValue) "test";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidDateTimeFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.DateTime};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidDateTimeFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.DateTime};
			var json = (JsonValue) "2016-01-25T10:32:02Z";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidEmailFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Email};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidEmailFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Email};
			var json = (JsonValue) "me@you.com";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnValidEmailFormat2()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Email};
			var json = (JsonValue) "Me@You.net";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidHostNameFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.HostName};
			var json = (JsonValue) "$test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidHostNameFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.HostName};
			var json = (JsonValue) "me.you.com";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidIpv4Format()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Ipv4};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidIpv4Format()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Ipv4};
			var json = (JsonValue) "255.255.1.1";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidIpv6Format()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Ipv6};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidIpv6Format()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Ipv6};
			var json = (JsonValue) "2001:0db8:85a3:0042:1000:8a2e:0370:7334";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidUriFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Uri};
			var json = (JsonValue) "test123^%";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidUriFormat()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Format = StringFormat.Uri};
			var json = (JsonValue) "http://en.wikipedia.org/wiki/ISO_8601";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnPatternNonMatch()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Pattern = "^[0-9_]+$"};
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnPatternMatch()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaType.String, Pattern = "^[0-9_]+$"};
			var json = (JsonValue) "81681_1868";

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}
