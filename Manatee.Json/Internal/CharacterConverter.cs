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
 
	File Name:		CharacterConverter.cs
	Namespace:		Manatee.Json.Internal
	Class Name:		CharacterConverter
	Purpose:		Maps ASCII characters to JsonInput values.

***************************************************************************************/

using System.Collections.Generic;

namespace Manatee.Json.Internal
{
	internal static class CharacterConverter
	{
		private static readonly Dictionary<char, JsonInput> Converter =
			new Dictionary<char, JsonInput>()
			{
				{'{', JsonInput.OpenBrace},
				{'}', JsonInput.CloseBrace},
				{'"', JsonInput.Quote},
				{':', JsonInput.Colon},
				{'-', JsonInput.Number},
				{'.', JsonInput.Number},
				{'0', JsonInput.Number},
				{'1', JsonInput.Number},
				{'2', JsonInput.Number},
				{'3', JsonInput.Number},
				{'4', JsonInput.Number},
				{'5', JsonInput.Number},
				{'6', JsonInput.Number},
				{'7', JsonInput.Number},
				{'8', JsonInput.Number},
				{'9', JsonInput.Number},
				{'t', JsonInput.Boolean},
				{'T', JsonInput.Boolean},
				{'f', JsonInput.Boolean},
				{'F', JsonInput.Boolean},
				{'n', JsonInput.Null},
				{'N', JsonInput.Null},
				{'[', JsonInput.OpenBracket},
				{']', JsonInput.CloseBracket},
				{',', JsonInput.Comma},
			};
		private static readonly Dictionary<char, JsonPathInput> PathConverter =
			new Dictionary<char, JsonPathInput>
			{
				{'$', JsonPathInput.Root},
				{':', JsonPathInput.Colon},
				{'-', JsonPathInput.Number},
				{'.', JsonPathInput.Number},
				{'0', JsonPathInput.Number},
				{'1', JsonPathInput.Number},
				{'2', JsonPathInput.Number},
				{'3', JsonPathInput.Number},
				{'4', JsonPathInput.Number},
				{'5', JsonPathInput.Number},
				{'6', JsonPathInput.Number},
				{'7', JsonPathInput.Number},
				{'8', JsonPathInput.Number},
				{'9', JsonPathInput.Number},
				{'.', JsonPathInput.Period},
				{'[', JsonPathInput.OpenBracket},
				{']', JsonPathInput.CloseBracket},
				{'(', JsonPathInput.OpenParenth},
				{')', JsonPathInput.CloseParenth},
				{',', JsonPathInput.Comma},
				{'*', JsonPathInput.Wildcard},
				{'@', JsonPathInput.Current},
				{'?', JsonPathInput.Filter},
				{'\'', JsonPathInput.Name},
			};

		public static JsonInput Item(char key)
		{
			return Converter[key];
		}
		public static JsonPathInput PathItem(char key)
		{
			return char.IsLetter(key) ? JsonPathInput.Name : PathConverter[key];
		}
	}
}