﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines various string formatting types used for StringSchema validation.
	/// </summary>
	public class StringFormat
	{
		/// <summary>
		/// Defines a date/time format via <see cref="DateTime.TryParse(string, out DateTime)"/>
		/// </summary>
		public static readonly StringFormat DateTime = new StringFormat("date-time", s =>
			{
				DateTime date;
				return System.DateTime.TryParse(s, out date);
			})
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		/// <summary>
		/// Defines an email address format.
		/// </summary>
		/// <remarks>
		/// From http://www.regular-expressions.info/email.html
		/// </remarks>
		public static readonly StringFormat Email = new StringFormat("email", "^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])$")
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		// from [lost the link, sorry]
		/// <summary>
		/// Defines a host name format.
		/// </summary>
		public static readonly StringFormat HostName = new StringFormat("hostname", "^(?!.{255,})([a-zA-Z0-9-]{0,63}\\.)*([a-zA-Z0-9-]{0,63})$")
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		// from [lost the link, sorry]
		/// <summary>
		/// Defines an IPV4 address format.
		/// </summary>
		public static readonly StringFormat Ipv4 = new StringFormat("ipv4", "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		// from [lost the link, sorry]
		/// <summary>
		/// Defines an IPV6 format.
		/// </summary>
		public static readonly StringFormat Ipv6 = new StringFormat("ipv6", "^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$")
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		/// <summary>
		/// Defines a regular expression format.
		/// </summary>
		public static readonly StringFormat Regex = new StringFormat("regex", null, true)
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		/// <summary>
		/// Defines a URI format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		/// <remarks>For draft-06 schema, only use this for absolute URIs.</remarks>
		public static readonly StringFormat Uri = new StringFormat("uri", s => System.Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute))
			{
				SupportedBy = new[]{typeof(JsonSchema04), typeof(JsonSchema06)}
			};
		/// <summary>
		/// Defines a URI format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>
		/// </summary>
		public static readonly StringFormat UriReference = new StringFormat("uri-reference", s => System.Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute))
			{
				SupportedBy = new[]{typeof(JsonSchema06)}
			};

		private static readonly Dictionary<string, StringFormat> _lookup = new Dictionary<string, StringFormat>
				{
					{DateTime.Key, DateTime},
					{Email.Key, Email},
					{HostName.Key, HostName},
					{Ipv4.Key, Ipv4},
					{Ipv6.Key, Ipv6},
					{Regex.Key, Regex},
					{Uri.Key, Uri},
					{UriReference.Key, UriReference}
				};

		private readonly Regex _validationRule;
		private readonly Func<string, bool> _validate;

		/// <summary>
		/// A string key which specifies this string format.
		/// </summary>
		public string Key { get; }
		
		private IList<Type> SupportedBy { get; set; }

		private StringFormat(string key, string regex, bool isCaseSensitive = false)
		{
			Key = key;
			if (regex != null)
				_validationRule = new Regex(regex, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
		}
		private StringFormat(string key, Func<string, bool> validate)
		{
			_validate = validate;
			Key = key;
		}

		/// <summary>
		/// Validates a value to the specified format.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value is valid, otherwise false.</returns>
		public bool Validate(string value)
		{
			if (_validationRule == null)
			{
				return _validate == null || _validate(value);
			}
			return _validationRule.IsMatch(value);
		}

		/// <summary>
		/// Gets a <see cref="StringFormat"/> object based on a format key.
		/// </summary>
		/// <param name="formatKey">The predefined key for the format.</param>
		/// <returns>A <see cref="StringFormat"/> object, or null if none exists for the key.</returns>
		public static StringFormat GetFormat(string formatKey)
		{
			return formatKey != null && _lookup.ContainsKey(formatKey)
				       ? _lookup[formatKey]
				       : null;
		}

		internal void ValidateForDraft<T>()
			where T : IJsonSchema
		{
			if (!((IList) SupportedBy).Contains(typeof(T)))
				throw new InvalidOperationException($"Format '{Key}' is not supported by {typeof(T).Name}");
		}
	}
}