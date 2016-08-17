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
 
	File Name:		PathExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		PathExpression
	Purpose:		Provides a base class for expressions which are part of paths.

***************************************************************************************/
using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class PathExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority => 6;
		public JsonPath Path { get; set; }
		public bool IsLocal { get; set; }

		public override object Evaluate(T json, JsonValue root)
		{
			var value = IsLocal ? json as JsonValue : root;
			if (value == null)
				throw new InvalidOperationException($"Path must evaluate to a JsonValue. Returned value is {json.GetType().Name}.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			return results.FirstOrDefault();
		}
		public override string ToString()
		{
			return (IsLocal ? "@" : "$") + Path.GetRawString();
		}
	}
}