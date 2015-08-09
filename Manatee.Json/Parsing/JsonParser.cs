/***************************************************************************************

	Copyright 2015 Greg Dennis

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
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal static class JsonParser
	{
		private static readonly List<IJsonParser> Parsers;

		static JsonParser()
		{
			Parsers = typeof (JsonParser).Assembly.GetTypes()
										 .Where(t => typeof (IJsonParser).IsAssignableFrom(t) &&
													 t.IsClass)
										 .Select(Activator.CreateInstance)
										 .Cast<IJsonParser>()
										 .ToList();
		}

		public static JsonValue Parse(string source)
		{
			int index = 0;
			return Parse(source, ref index);
		}
		public static JsonValue Parse(string source, ref int index)
		{
			var length = source.Length;
			var c = source.SkipWhiteSpace(ref index, length);
			var parser = Parsers.FirstOrDefault(p => p.Handles(c));
			if (parser == null)
				throw new JsonSyntaxException("Unrecognized token");
			return parser.Parse(source, ref index);
		}
	}
}