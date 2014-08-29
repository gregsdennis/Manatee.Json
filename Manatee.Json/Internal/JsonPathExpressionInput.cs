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
 
	File Name:		JsonPathExpressionInput.cs
	Namespace:		Manatee.Json.Internal
	Class Name:		JsonPathExpressionInput
	Purpose:		Enumerates input types for JSON Path Expressions.

***************************************************************************************/
namespace Manatee.Json.Internal
{
	internal enum JsonPathExpressionInput
	{
		OpenParenth,
		CloseParenth,
		Number,
		Plus,
		Minus,
		Star,
		Slash,
		Caret,
		Root,
		Current,
		LessThan,
		Equal,
		GreaterThan,
		Bang,
		Quote,
		Letter,
		And,
		Or,
		End
	}
}