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
 
	File Name:		NotExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		NotExpression
	Purpose:		Expresses the intent to invert a boolean.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class NotExpression<T> : ExpressionTreeNode<T>, IEquatable<NotExpression<T>>
	{
		public ExpressionTreeNode<T> Root { get; set; }

		protected override int BasePriority => 5;

		public override object Evaluate(T json, JsonValue root)
		{
			var result = Root.Evaluate(json, root);
			return result != null && result.Equals(true);
		}
		public override string ToString()
		{
			return Root is AndExpression<T> || Root is OrExpression<T> ||
				   Root is IsEqualExpression<T> || Root is IsNotEqualExpression<T> ||
				   Root is IsLessThanExpression<T> || Root is IsLessThanEqualExpression<T> ||
				   Root is IsGreaterThanExpression<T> || Root is IsGreaterThanEqualExpression<T>
					   ? $"!({Root})"
				       : $"!{Root}";
		}
		public bool Equals(NotExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Root, other.Root);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as NotExpression<T>);
		}
		public override int GetHashCode()
		{
			return Root?.GetHashCode() ?? 0;
		}
	}
}