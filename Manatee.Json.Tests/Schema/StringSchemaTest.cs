using System.Collections.Generic;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class StringSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonString()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String);
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsErrorOnTooShort()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).MinLength(5);
			var json = (JsonValue) "test";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthEqualsMinLength()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).MinLength(5);
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthGreaterThanMinLength()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).MinLength(5);
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnTooLong()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).MaxLength(5);
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthEqualsMaxLength()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).MaxLength(5);
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnLengthLessThanMaxLength()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).MaxLength(5);
			var json = (JsonValue) "test";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidDateTimeFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.DateTime);
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidDateTimeFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.DateTime);
			var json = (JsonValue) "2016-01-25T10:32:02Z";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidEmailFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Email);
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidEmailFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Email);
			var json = (JsonValue) "me@you.com";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnValidEmailFormat2()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Email);
			var json = (JsonValue) "Me@You.net";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidHostNameFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.HostName);
			var json = (JsonValue) "$test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidHostNameFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.HostName);
			var json = (JsonValue) "me.you.com";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidIpv4Format()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Ipv4);
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidIpv4Format()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Ipv4);
			var json = (JsonValue) "255.255.1.1";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidIpv6Format()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Ipv6);
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidIpv6Format()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Ipv6);
			var json = (JsonValue) "2001:0db8:85a3:0042:1000:8a2e:0370:7334";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsErrorOnInvalidUriFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Uri);
			var json = (JsonValue) "test123^%";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnValidUriFormat()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.Uri);
			var json = (JsonValue) "http://en.wikipedia.org/wiki/ISO_8601";

			var results = schema.Validate(json);

			results.AssertValid();
		}
		[Test]
		public void ValidateReturnsValidOnUnknownFormat()
		{
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Hierarchy;
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.GetFormat("Int32"));
			var json = (JsonValue) "32";
			var expected = new SchemaValidationResults
				{
					IsValid = true,
					RelativeLocation = JsonPointer.Parse("#/format"),
					InstanceLocation = JsonPointer.Parse("#"),
					Keyword = "format",
					AnnotationValue = "Int32"
				};

			var results = schema.Validate(json);

			results.AssertValid(expected);
		}
		[Test]
		public void DeserializeThrowsOnUnknownFormat()
		{
			JsonSchemaOptions.AllowUnknownFormats = false;
			var serializer = new JsonSerializer();
			var schemaJson = new JsonObject
				{
					["type"] = "string",
					["format"] = "Int32"
				};

			try
			{
				Assert.Throws<JsonSerializationException>(() => serializer.Deserialize<JsonSchema>(schemaJson));
			}
			finally
			{
				JsonSchemaOptions.AllowUnknownFormats = true;
			}
		}
		[Test]
		public void ValidateReturnsInvalidOnUnknownFormat()
		{
			JsonSchemaOptions.AllowUnknownFormats = false;
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Hierarchy;
			var schema = new JsonSchema().Type(JsonSchemaType.String).Format(StringFormat.GetFormat("Int32"));
			var json = (JsonValue)"32";
			var expected = new SchemaValidationResults
				{
					IsValid = false,
					RelativeLocation = JsonPointer.Parse("#/format"),
					InstanceLocation = JsonPointer.Parse("#"),
					Keyword = "format",
					AdditionalInfo = new JsonObject
						{
							["format"] = "Int32",
							["isKnownFormat"] = false
						}
				};

			try
			{
				var results = schema.Validate(json);

				results.AssertInvalid(expected);
			}
			finally
			{
				JsonSchemaOptions.AllowUnknownFormats = true;
			}
		}
		[Test]
		public void ValidateReturnsErrorOnPatternNonMatch()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Pattern("^[0-9_]+$");
			var json = (JsonValue) "test123";

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[Test]
		public void ValidateReturnsValidOnPatternMatch()
		{
			var schema = new JsonSchema().Type(JsonSchemaType.String).Pattern("^[0-9_]+$");
			var json = (JsonValue) "81681_1868";

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}
