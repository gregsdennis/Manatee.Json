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
 
	File Name:		Expression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		Expression
	Purpose:		Represents an expression in JsonPaths.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class Expression<T, TIn> : IEquatable<Expression<T, TIn>>
	{
		private readonly ExpressionTreeNode<TIn> _root;

		public Expression(ExpressionTreeNode<TIn> root)
		{
			_root = root;
		}

		public T Evaluate(TIn json, JsonValue root)
		{
			var result = _root.Evaluate(json, root);
			if (typeof(T) == typeof(bool) && result == null)
				return (T) (object) false;
			if (typeof(T) == typeof(bool) && result != null && !(result is bool))
				return (T) (object) true;
			if (typeof(T) == typeof(int) && result == null)
				return (T) (object) -1;
			var resultAsJsonValue = result as JsonValue;
			if (typeof(T) == typeof(int) && resultAsJsonValue?.Type == JsonValueType.Number)
				return (T) Convert.ChangeType(resultAsJsonValue.Number, typeof(T));
			if (typeof(T) == typeof(JsonValue))
			{
				if (result is JsonValue) return (T) result;
				if (result is double)
					return (T) (object) new JsonValue((double) result);
				if (result is bool)
					return (T) (object) new JsonValue((bool) result);
				if (result is string)
					return (T) (object) new JsonValue((string) result);
				if (result is JsonArray)
					return (T) (object) new JsonValue((JsonArray) result);
				if (result is JsonObject)
					return (T) (object) new JsonValue((JsonObject) result);
			}
			return (T) Convert.ChangeType(result, typeof(T));
		}
		public override string ToString()
		{
			return _root.ToString();
		}
		public bool Equals(Expression<T, TIn> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(_root, other._root);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as Expression<T, TIn>);
		}
		public override int GetHashCode()
		{
			return _root?.GetHashCode() ?? 0;
		}
	}
}
