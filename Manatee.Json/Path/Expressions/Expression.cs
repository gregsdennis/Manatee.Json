﻿using System;

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
