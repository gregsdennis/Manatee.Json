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
 
	File Name:		FieldExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		FieldExpression
	Purpose:		Expresses the intent to retrieve a value from a field.

***************************************************************************************/

using System;
using System.Reflection;

namespace Manatee.Json.Path.Expressions
{
	internal class FieldExpression<T> : ExpressionTreeNode<T>, IEquatable<FieldExpression<T>>
	{
		public FieldInfo Field { get; set; }
		public object Source { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			if (Field.FieldType == typeof(string))
				return Field.GetValue(Source);
			return Convert.ToDouble(Field.GetValue(Source));
		}
		public override string ToString()
		{
			var value = Evaluate(default(T), null);
			return value is string
				       ? $"\"{value}\""
				       : value?.ToString() ?? "null";
		}
		public bool Equals(FieldExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Field, other.Field) && Equals(Source, other.Source);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as FieldExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Field != null ? Field.GetHashCode() : 0)*397) ^ (Source?.GetHashCode() ?? 0); }
		}
	}
}