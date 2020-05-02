﻿using System;
 using System.Collections.Concurrent;
 using System.Linq;

 namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines various formats used for <see cref="FormatKeyword"/> validation.
	/// </summary>
	/// <remarks>
	/// Any of these may be overridden by creating a new instance with the same key.
	/// </remarks>
	public static class Formats
	{
		private static readonly ConcurrentDictionary<string, IFormatValidator> _lookup;

		/// <summary>
		/// Gets the `date` format.
		/// </summary>
		public static IFormatValidator? Date => GetFormat(DateFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `date-time` format.
		/// </summary>
		public static IFormatValidator? DateTime => GetFormat(DateTimeFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `duration` format.
		/// </summary>
		public static IFormatValidator? Duration => GetFormat(DurationFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `email` format.
		/// </summary>
		public static IFormatValidator? Email => GetFormat(EmailFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `hostname` format.
		/// </summary>
		public static IFormatValidator? HostName => GetFormat(HostNameFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `ipv4` format.
		/// </summary>
		public static IFormatValidator? Ipv4 => GetFormat(Ipv4FormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `ipv6` format.
		/// </summary>
		public static IFormatValidator? Ipv6 => GetFormat(Ipv6FormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `iri` format.
		/// </summary>
		public static IFormatValidator? Iri => GetFormat(IriFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `iri-reference` format.
		/// </summary>
		public static IFormatValidator? IriReference => GetFormat(IriReferenceFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `json-pointer` format.
		/// </summary>
		public static IFormatValidator? JsonPointer => GetFormat(JsonPointerFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `regex` format.
		/// </summary>
		public static IFormatValidator? Regex => GetFormat(RegexFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `relative-json-pointer` format.
		/// </summary>
		public static IFormatValidator? RelativeJsonPointer => GetFormat(RelativeJsonPointerFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `uri` format.
		/// </summary>
		public static IFormatValidator? Uri => GetFormat(UriFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `uri-reference` format.
		/// </summary>
		public static IFormatValidator? UriReference => GetFormat(UriReferenceFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `uri-template` format.
		/// </summary>
		public static IFormatValidator? UriTemplate => GetFormat(UriTemplateFormatValidator.Instance.Format);
		/// <summary>
		/// Gets the `uuid` format.
		/// </summary>
		public static IFormatValidator? Uuid => GetFormat(UuidReferenceFormatValidator.Instance.Format);

		static Formats()
		{
			_lookup = new ConcurrentDictionary<string, IFormatValidator>
				{
					[DateFormatValidator.Instance.Format] = DateFormatValidator.Instance,
					[DateTimeFormatValidator.Instance.Format] = DateTimeFormatValidator.Instance,
					[DurationFormatValidator.Instance.Format] = DurationFormatValidator.Instance,
					[EmailFormatValidator.Instance.Format] = EmailFormatValidator.Instance,
					[HostNameFormatValidator.Instance.Format] = HostNameFormatValidator.Instance,
					[Ipv4FormatValidator.Instance.Format] = Ipv4FormatValidator.Instance,
					[Ipv6FormatValidator.Instance.Format] = Ipv6FormatValidator.Instance,
					[IriFormatValidator.Instance.Format] = IriFormatValidator.Instance,
					[IriReferenceFormatValidator.Instance.Format] = IriReferenceFormatValidator.Instance,
					[JsonPointerFormatValidator.Instance.Format] = JsonPointerFormatValidator.Instance,
					[RegexFormatValidator.Instance.Format] = RegexFormatValidator.Instance,
					[RelativeJsonPointerFormatValidator.Instance.Format] = RelativeJsonPointerFormatValidator.Instance,
					[UriFormatValidator.Instance.Format] = UriFormatValidator.Instance,
					[UriReferenceFormatValidator.Instance.Format] = UriReferenceFormatValidator.Instance,
					[UriTemplateFormatValidator.Instance.Format] = UriTemplateFormatValidator.Instance,
					[UuidReferenceFormatValidator.Instance.Format] = UuidReferenceFormatValidator.Instance
			};
		}

		/// <summary>
		/// Gets a <see cref="IFormatValidator"/> object based on a format key.
		/// </summary>
		/// <param name="formatKey">The predefined key for the format.</param>
		/// <returns>A <see cref="IFormatValidator"/> object, or null if none exists for the key.</returns>
		public static IFormatValidator? GetFormat(string formatKey)
		{
			if (formatKey == null)
				throw new ArgumentNullException(nameof(formatKey));

			_lookup.TryGetValue(formatKey, out var format);
			return format; 
			//: new Format(formatKey, JsonSchemaVersion.None, s => JsonSchemaOptions.AllowUnknownFormats) {IsKnown = false};
		}

		/// <summary>
		/// Adds a validator to the library.  This will override any existing validators for the same key.
		/// </summary>
		/// <param name="validator">The validator to add.</param>
		public static void RegisterFormat(IFormatValidator validator)
		{
			if (validator == null)
				throw new ArgumentNullException(nameof(validator));

			_lookup[validator.Format] = validator;
		}

		/// <summary>
		/// Removes support for the given format.
		/// </summary>
		/// <param name="formatKey">The format key.</param>
		public static void UnregisterFormat(string formatKey)
		{
			if (formatKey == null)
				throw new ArgumentNullException(nameof(formatKey));

			_lookup.TryRemove(formatKey, out _);
		}
	}
}