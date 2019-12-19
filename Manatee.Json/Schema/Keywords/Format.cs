﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines various formats used for <see cref="FormatKeyword"/> validation.
	/// </summary>
	/// <remarks>
	/// Any of these may be overridden by creating a new instance with the same key.
	/// </remarks>
	public class Format
	{
		/// <summary>
		/// Defines an ISO 8601 date format.
		/// </summary>
		public static Format Date { get; set; }
		/// <summary>
		/// Defines an ISO 8601 date/time format.
		/// </summary>
		public static Format DateTime { get; set; }
		/// <summary>
		/// Defines an ISO 8601 duration format.
		/// </summary>
		public static Format Duration { get; set; }
		/// <summary>
		/// Defines an email address format.
		/// </summary>
		/// <remarks>
		/// From http://www.regular-expressions.info/email.html
		/// </remarks>
		public static Format Email { get; set; }
		// from [lost the link, sorry]
		/// <summary>
		/// Defines a host name format.
		/// </summary>
		public static Format HostName { get; set; }
		// from [lost the link, sorry]
		/// <summary>
		/// Defines an IPV4 address format.
		/// </summary>
		public static Format Ipv4 { get; set; }
		// from [lost the link, sorry]
		/// <summary>
		/// Defines an IPV6 format.
		/// </summary>
		public static Format Ipv6 { get; set; }
		/// <summary>
		/// Defines an IRI format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static Format Iri { get; set; }
		/// <summary>
		/// Defines an IRI Reference format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static Format IriReference { get; set; }
		/// <summary>
		/// Defines a JSON Pointer format.
		/// </summary>
		public static Format JsonPointer { get; set; }
		/// <summary>
		/// Defines a regular expression format.
		/// </summary>
		public static Format Regex { get; set; }
		/// <summary>
		/// Defines a Relative JSON Pointer format.
		/// </summary>
		public static Format RelativeJsonPointer { get; set; }
		/// <summary>
		/// Defines a URI format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		/// <remarks>For draft-06 schema and later, only use this for absolute URIs.</remarks>
		public static Format Uri { get; set; }
		/// <summary>
		/// Defines a URI Reference format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static Format UriReference { get; set; }
		/// <summary>
		/// Defines a URI Template format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static Format UriTemplate { get; set; }
		/// <summary>
		/// Defines a UUID format.
		/// </summary>
		public static Format Uuid { get; set; }

		private static readonly Dictionary<string, Format> _lookup;

		/// <summary>
		/// A string key which specifies this string format.
		/// </summary>
		public string Key { get; }
		
		/// <summary>
		/// Gets
		/// </summary>
		public JsonSchemaVersion SupportedBy { get; }

		/// <summary>
		/// If created with a regular expression, this gets the regular expression.
		/// </summary>
		public Regex? ValidationRegex { get; }

		/// <summary>
		/// If created with a validation function, this gets the function.
		/// </summary>
		public Func<JsonValue, bool>? ValidationFunction { get; }

		internal bool IsKnown { get; private set; } = true;

		private static readonly string[] _dateTimeFormats =
			{
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss",
			};

		static Format()
		{
			_lookup = new Dictionary<string, Format>();

			Date = new Format("date", JsonSchemaVersion.Draft2019_09,
									@"^(\d{4})-(\d{2})-(\d{2})$");
			DateTime = new Format("date-time", JsonSchemaVersion.All,
										s => s.Type != JsonValueType.String || DateTimeOffset.TryParseExact(s.String, _dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
			Duration = new Format("duration", JsonSchemaVersion.Draft2019_09,
										@"^(-?)P(?=\d|T\d)(?:(\d+)Y)?(?:(\d+)M)?(?:(\d+)([DW]))?(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?)?$");
			Email = new Format("email", JsonSchemaVersion.All,
									 @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$");
			HostName = new Format("hostname", JsonSchemaVersion.All,
										@"^(?!.{255,})([a-zA-Z0-9-]{0,63}\.)*([a-zA-Z0-9-]{0,63})$");
			Ipv4 = new Format("ipv4", JsonSchemaVersion.All,
									@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
			Ipv6 = new Format("ipv6", JsonSchemaVersion.All,
									@"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$");
			IriReference = new Format("iri-reference", JsonSchemaVersion.Draft2019_09,
											@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");
			Iri = new Format("iri", JsonSchemaVersion.Draft2019_09, s => s.Type != JsonValueType.String || System.Uri.IsWellFormedUriString(s.String, UriKind.RelativeOrAbsolute));
			JsonPointer = new Format("json-pointer", JsonSchemaVersion.Draft2019_09, @"^(/(([^/~])|(~[01]))+)*/?$");
			Regex = new Format("regex", JsonSchemaVersion.All, string.Empty, true);
			RelativeJsonPointer = new Format("relative-json-pointer", JsonSchemaVersion.Draft2019_09, @"^[0-9]+#/(([^/~])|(~[01]))*$");
			UriReference = new Format("uri-reference", JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_09,
											@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");
			UriTemplate = new Format("uri-template", JsonSchemaVersion.Draft2019_09,
											@"^$");
			Uri = new Format("uri", JsonSchemaVersion.All, s => s.Type != JsonValueType.String || System.Uri.IsWellFormedUriString(s.String, UriKind.RelativeOrAbsolute));
			Uuid = new Format("uuid", JsonSchemaVersion.Draft2019_09,
									@"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}");
		}
		/// <summary>
		/// Creates a new <see cref="Format"/> instance using a regular expression.
		/// </summary>
		/// <param name="key">The name of the format.  This is the value that will appear in the schema.</param>
		/// <param name="supportedBy">The schema drafts supported by this keyword.</param>
		/// <param name="regex">The validation regular expression.</param>
		/// <param name="isCaseSensitive">(optional) Whether the regular expression is to be processed as case-sensitive; the default is `false`.</param>
		public Format(string key, JsonSchemaVersion supportedBy, [RegexPattern] string regex, bool isCaseSensitive = false)
		{
			Key = key;
			SupportedBy = supportedBy;
			if (regex != null)
				ValidationRegex = new Regex(regex, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);

			_lookup[key] = this;
		}
		/// <summary>
		/// Creates a new <see cref="Format"/> instance using a validation function.
		/// </summary>
		/// <param name="key">The name of the format.  This is the value that will appear in the schema.</param>
		/// <param name="supportedBy">The schema drafts supported by this keyword.</param>
		/// <param name="validate">The function by which the value is validated.</param>
		public Format(string key, JsonSchemaVersion supportedBy, Func<JsonValue, bool> validate)
		{
			ValidationFunction = validate;
			Key = key;
			SupportedBy = supportedBy;

			_lookup[key] = this;
		}

		/// <summary>
		/// Validates a value to the specified format.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value is valid, otherwise false.</returns>
		public bool Validate(JsonValue value)
		{
			return ValidationRegex == null || value.Type != JsonValueType.String
				? ValidationFunction == null || ValidationFunction(value)
				: ValidationRegex.IsMatch(value.String);
		}

		/// <summary>
		/// Gets a <see cref="Format"/> object based on a format key.
		/// </summary>
		/// <param name="formatKey">The predefined key for the format.</param>
		/// <returns>A <see cref="Format"/> object, or null if none exists for the key.</returns>
		public static Format GetFormat(string formatKey)
		{
			if (formatKey == null)
				throw new ArgumentNullException(nameof(formatKey));

			return _lookup.TryGetValue(formatKey, out var format)
				? format
				: new Format(formatKey, JsonSchemaVersion.None, s => JsonSchemaOptions.AllowUnknownFormats) {IsKnown = false};
		}
	}
}