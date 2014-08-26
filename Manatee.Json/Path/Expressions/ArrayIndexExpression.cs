/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ArrayIndexExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ArrayIndexExpression
	Purpose:		Expresses the intent to retrieve a value by providing a
					numeric array index.

***************************************************************************************/
using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class ArrayIndexExpression<T> : PathExpression<T>
	{
		public override int Priority { get { return 5; } }
		public int Index { get; set; }

		public override object Evaluate(T json, JsonValue root)
		{
			var value = IsLocal ? json as JsonValue : root;
			if (value == null)
				throw new NotSupportedException("ArrayIndex requires a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException(string.Format("Path '{0}' returned more than one result on value '{1}'", Path, value));
			var result = results.FirstOrDefault();
			return result != null && result.Type == JsonValueType.Array && Index >= 0 && Index < result.Array.Count
					   ? result.Array[Index]
				       : null;
		}
		public override string ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			return string.Format(IsLocal ? "@{0}[{1}]" : "${0}[{1}]", path, Index);
		}
	}
}