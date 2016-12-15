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
 
	File Name:		ConversionExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ConversionExpression
	Purpose:		Expresses the intent to convert from one type to another.

***************************************************************************************/

using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions
{
	internal class ConversionExpression<T> : ExpressionTreeNode<T>, IEquatable<ConversionExpression<T>>
	{
		public ExpressionTreeNode<T> Root { get; set; }
		public Type TargetType { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			var value = Root.Evaluate(json, root);
			var result = CastValue(value);
			return result;
		}
		public override string ToString()
		{
			return Root.ToString();
		}
		public bool Equals(ConversionExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Root, other.Root) && TargetType == other.TargetType;
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ConversionExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Root?.GetHashCode() ?? 0)*397) ^ (TargetType?.GetHashCode() ?? 0); }
		}

		private object CastValue(object value)
		{
			if (TargetType != typeof (JsonValue)) return value;
			if (value is bool)
				return new JsonValue((bool)value);
			if (value is string)
				return new JsonValue((string)value);
			if (value.IsNumber())
				return new JsonValue(Convert.ToDouble(value));
			return value;
		}
	}
}