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
 
	File Name:		ValueExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ValueExpression
	Purpose:		Expresses a constant value.

***************************************************************************************/

using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class ValueExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 5; } }
		public object Value { get; set; }

		public override object Evaluate(T json)
		{
			return Value;
		}
		public override string ToString()
		{
			return Value is string
				       ? string.Format("\"{0}\"", Value)
				       : Value.ToString();
		}
	}

	internal abstract class PathExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 5; } }
		public JsonPath Path { get; set; }

		public override object Evaluate(T json)
		{
			var value = json as JsonValue;
			if (value == null)
				throw new NotSupportedException("Path expressions require a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException(string.Format("Path '{0}' returned more than one result on value '{1}'", Path, value));
			return results.FirstOrDefault();
		}
		public override string ToString()
		{
			return string.Format("@{0}", Path.GetRawString());
		}
	}

	internal class PathNumberExpression<T> : PathExpression<T>
	{
		public override object Evaluate(T json)
		{
			var result = (JsonValue)base.Evaluate(json);
			if (result == null) return null;
			return result.Type == JsonValueType.Number
					   ? result.Number
					   : (double?)null;
		}
	}

	internal class PathStringExpression<T> : PathExpression<T>
	{
		public override object Evaluate(T json)
		{
			var result = (JsonValue)base.Evaluate(json);
			if (result == null) return null;
			return result.Type == JsonValueType.String
				       ? result.String
				       : null;
		}
	}
}