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
 
	File Name:		JsonInput.cs
	Namespace:		Manatee.Json.Internal
	Class Name:		JsonInput
	Purpose:		Enumerates input types for JSON structures.

***************************************************************************************/
namespace Manatee.Json.Internal
{
	internal enum JsonInput
	{
		Unknown,
		OpenBrace,
		Quote,
		Colon,
		Number,
		Boolean,
		Null,
		OpenBracket,
		Comma,
		CloseBracket,
		CloseBrace
	}
}
