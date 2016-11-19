/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		JsonParser
	Purpose:		Manages parsing of all JSON values.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal static class JsonParser
	{
		private static readonly List<IJsonParser> Parsers;

		static JsonParser()
		{
#if CORE
			Parsers = typeof(JsonParser).GetTypeInfo().Assembly.GetTypes()
			                            .Where(t => typeof(IJsonParser).IsAssignableFrom(t) && t.GetTypeInfo().IsClass)
#else
			Parsers = typeof(JsonParser).Assembly.GetTypes()
										.Where(t => typeof(IJsonParser).IsAssignableFrom(t) && t.IsClass)
#endif
			                            .Select(Activator.CreateInstance)
			                            .Cast<IJsonParser>()
			                            .ToList();
		}

		public static JsonValue Parse(string source)
		{
			var index = 0;
			JsonValue value;
			var errorMessage = Parse(source, ref index, out value);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);
			return value;
		}
		public static JsonValue Parse(StreamReader stream)
		{
			JsonValue value;
			var errorMessage = Parse(stream, out value);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);
			return value;
		}
		public static string Parse(string source, ref int index, out JsonValue value, bool allowExtraChars = false)
		{
			var length = source.Length;
			char c;
			var errorMessage = source.SkipWhiteSpace(ref index, length, out  c);
			if (errorMessage != null)
			{
				value = null;
				return errorMessage;
			}
			var parser = Parsers.FirstOrDefault(p => p.Handles(c));
			if (parser == null)
			{
				value = null;
				return "Cannot determine type.";
			}
			errorMessage = parser.TryParse(source, ref index, out value, allowExtraChars);
			return errorMessage;
		}
		public static string Parse(StreamReader stream, out JsonValue value)
		{
			char c;
			var errorMessage = stream.SkipWhiteSpace(out  c);
			if (errorMessage != null)
			{
				value = null;
				return errorMessage;
			}
			var parser = Parsers.FirstOrDefault(p => p.Handles(c));
			if (parser == null)
			{
				value = null;
				return "Cannot determine type.";
			}
			errorMessage = parser.TryParse(stream, out value);
			return errorMessage;
		}
	}
}