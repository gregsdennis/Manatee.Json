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
 
	File Name:		StringFormat.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		StringFormat
	Purpose:		Defines various string formatting types used for StringSchema
					validation.

***************************************************************************************/

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Manatee.Json.Schema
{
	public class StringFormat
	{
		// from https://bugzilla.mozilla.org/show_bug.cgi?id=468020
		public static readonly StringFormat DateTime = new StringFormat("date-time", "^([0-9]{4})-([0-9]{2})-([0-9]{2})([Tt]([0-9]{2}):([0-9]{2}):([0-9]{2})(\\.[0-9]+)?)?(([Zz]|([+-])([0-9]{2}):([0-9]{2})))?");
		// from http://www.regular-expressions.info/email.html
		public static readonly StringFormat Email = new StringFormat("email", "^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])$");
		public static readonly StringFormat HostName = new StringFormat("hostname", "^(?!.{255,})([a-zA-Z0-9-]{0,63}\\.)*([a-zA-Z0-9-]{0,63})$");
		// from [lost the link, sorry]
		public static readonly StringFormat Ipv4 = new StringFormat("ipv4", "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
		// from [lost the link, sorry]
		public static readonly StringFormat Ipv6 = new StringFormat("ipv6", "^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$");
		public static readonly StringFormat Regex = new StringFormat("regex", null);
		// from http://mathiasbynens.be/demo/url-regex
		public static readonly StringFormat Uri = new StringFormat("uri", "^([a-zA-Z]{3,})://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?$");

		private static readonly Dictionary<string, StringFormat> _lookup = new Dictionary<string, StringFormat>
				{
					{DateTime.Key, DateTime},
					{Email.Key, Email},
					{HostName.Key, HostName},
					{Ipv4.Key, Ipv4},
					{Ipv6.Key, Ipv6},
					{Regex.Key, Regex},
					{Uri.Key, Uri}
				};

		private readonly Regex _validationRule;
		private readonly string _key;

		public string Key { get { return _key; } }

		private StringFormat(string key, string regex)
		{
			_key = key;
			if (regex != null)
				_validationRule = new Regex(regex);
		}

		public bool Validate(string value)
		{
			return _validationRule == null || _validationRule.IsMatch(value);
		}

		public static StringFormat GetFormat(string formatKey)
		{
			return _lookup.ContainsKey(formatKey)
				? _lookup[formatKey]
				: null;
		}
	}
}