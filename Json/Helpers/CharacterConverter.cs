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
	Namespace:		Manatee.Json.Helpers
	Class Name:		CharacterConverter
	Purpose:		Maps ASCII characters to JsonInput values.

***************************************************************************************/
using System.Collections.Generic;

namespace Manatee.Json.Helpers
{
	internal static class CharacterConverter
	{
	    private static readonly Dictionary<char, JsonInput> Converter = new Dictionary<char, JsonInput>()
	                                                                        {
	                                                                            {'{', JsonInput.OpenBrace},
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
	                                                                            {',', JsonInput.Comma},
	                                                                            {']', JsonInput.CloseBracket},
	                                                                            {'}', JsonInput.CloseBrace}
	                                                                        };
		public static JsonInput Item(char key) { return Converter[key]; }
	}
}