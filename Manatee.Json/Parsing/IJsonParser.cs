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
 
	File Name:		IJsonParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		IJsonParser
	Purpose:		Defines a parser for a single JSON type.

***************************************************************************************/

using System.IO;

namespace Manatee.Json.Parsing
{
	internal interface IJsonParser
	{
		bool Handles(char c);
		// returns error message, if any.  Null return implies success.
		string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars);
		string TryParse(StreamReader stream, out JsonValue value);
	}
}
