using System;
using System.Xml.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class ContentMediaTypeSchema07PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema07>
	{
		public bool Applies(JsonSchema07 schema, JsonValue json)
		{
			return schema.ContentMediaType != null;
		}
		public SchemaValidationResults Validate(JsonSchema07 schema, JsonValue json, JsonValue root)
		{
			// TODO: Need to validate that a string is (e.g.) of type "application/json" or "application/xml"
			throw new NotImplementedException();
		}

		private static bool _IsValidXml(string value)
		{
			var valid = true;
			try
			{
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				XDocument.Parse(value);
			}
			catch (Exception)
			{
				valid = false;
			}
			return valid;
		}
	}
}
