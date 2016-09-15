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
 
	File Name:		IndexOfExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		IndexOfExpression
	Purpose:		Expresses the intent to retrieve the index of a given value.

***************************************************************************************/
using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class IndexOfExpression<T> : PathExpression<T>, IEquatable<IndexOfExpression<T>>
	{
		public JsonValue Parameter { get; set; }
		public ExpressionTreeNode<JsonArray> ParameterExpression { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			var value = IsLocal ? json as JsonValue : root;
			if (value == null)
				throw new NotSupportedException("IndexOf requires a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			var result = results.FirstOrDefault();
			var parameter = GetParameter();
			return result != null && result.Type == JsonValueType.Array && parameter != null
					   ? result.Array.IndexOf(parameter)
					   : (object)null;
		}
		public override string ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			var parameter = ParameterExpression?.ToString() ?? Parameter.ToString();
			return string.Format(IsLocal ? "@{0}.indexOf({1})" : "${0}.indexOf({1})", path, parameter);
		}
		public bool Equals(IndexOfExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(Parameter, other.Parameter) && Equals(ParameterExpression, other.ParameterExpression);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as IndexOfExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (ParameterExpression?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		private JsonValue GetParameter()
		{
			var value = ParameterExpression?.Evaluate(null, null);
			if (value != null)
			{
				if (value is double)
					return new JsonValue((double)value);
				if (value is bool)
					return new JsonValue((bool)value);
				if (value is string)
					return new JsonValue((string)value);
				if (value is JsonArray)
					return new JsonValue((JsonArray)value);
				if (value is JsonObject)
					return new JsonValue((JsonObject)value);
				if (value is JsonValue)
					return (JsonValue) value;
			}
			return Parameter;
		}
	}
}
