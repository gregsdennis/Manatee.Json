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
 
	File Name:		JsonPathParser.cs
	Namespace:		Manatee.Json.Path.Parsing
	Class Name:		JsonPathParser
	Purpose:		Parses JSON Path.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class JsonPathParser
	{
		private static readonly List<IJsonPathParser> Parsers;

		static JsonPathParser()
		{
			Parsers = typeof(JsonPathParser).GetTypeInfo().Assembly.GetTypes()
			                                .Where(t => typeof(IJsonPathParser).IsAssignableFrom(t) && t.GetTypeInfo().IsClass)
			                                .Select(Activator.CreateInstance)
			                                .Cast<IJsonPathParser>()
			                                .ToList();
		}

		public static JsonPath Parse(string source)
		{
			var index = 0;
			JsonPath path;
			var errorMessage = Parse(source, ref index, out path);
			if (errorMessage != null)
				throw new JsonPathSyntaxException(path, errorMessage);
			return path;
		}
		public static string Parse(string source, ref int index, out JsonPath path)
		{
			var length = source.Length;
			path = null;
			while(index < length)
			{
				char c;
				var errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return errorMessage;
				var substring = source.Substring(index);
				var parser = Parsers.FirstOrDefault(p => p.Handles(substring));
				if (parser == null) return "Unrecognized JSON Path element.";
				errorMessage = parser.TryParse(source, ref index, ref path);
				if (errorMessage != null) return errorMessage;
			}
			return null;
		}
	}
}
