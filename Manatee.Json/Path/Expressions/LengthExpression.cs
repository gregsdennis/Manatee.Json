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
 
	File Name:		LengthExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		LengthExpression
	Purpose:		Expresses the intent to retrieve the length of an array.

***************************************************************************************/
using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class LengthExpression<T> : PathExpression<T>
	{
		public override int Priority { get { return 6; } }

		public override object Evaluate(T json, JsonValue root)
		{
			JsonArray array;
			if (Path == null || !Path.Operators.Any())
			{
				if (IsLocal)
					array = json as JsonArray;
				else
					array = root.Type == JsonValueType.Array ? root.Array : null;
				if (array == null)
				{
					var value = json as JsonValue;
					if (value.Type == JsonValueType.Array)
						array = value.Array;
				}
				if (array == null) return null;
			}
			else
			{
				var value = IsLocal
								? json is JsonArray ? json as JsonArray : json as JsonValue
								: root;
				var results = Path.Evaluate(value);
				if (results.Count > 1)
					throw new InvalidOperationException(string.Format("Path '{0}' returned more than one result on value '{1}'", Path, value));
				var result = results.FirstOrDefault();
				array = result != null && result.Type == JsonValueType.Array
					        ? result.Array
					        : null;
			}
			return array == null ? null : (object)(double) array.Count;
		}
		public override string ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			return string.Format(IsLocal ? "@{0}.length" : "${0}.length", path);
		}
	}
}