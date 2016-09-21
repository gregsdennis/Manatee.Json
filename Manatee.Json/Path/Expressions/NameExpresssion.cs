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
 
	File Name:		NameExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		NameExpression
	Purpose:		Expresses the intent to retrieve a value from a named
					property in an object.

***************************************************************************************/
using System;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions
{
	internal class NameExpression<T> : PathExpression<T>, IEquatable<NameExpression<T>>
	{
		public string Name { get; set; }
		public ExpressionTreeNode<T> NameExp { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			var value = IsLocal ? json.AsJsonValue() : root;
			if (value == null)
				throw new NotSupportedException("Name requires a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			var result = results.FirstOrDefault();
			var name = GetName();
			return result != null && result.Type == JsonValueType.Object && result.Object.ContainsKey(name)
				       ? result.Object[name].GetValue()
				       : null;
		}
		public override string ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			return string.Format(IsLocal ? "@{0}.{1}" : "${0}.{1}", path, GetName());
		}

		private string GetName()
		{
			var value = NameExp?.Evaluate(default(T), null);
			if (value != null)
				return (string)value;
			return Name;
		}
		public bool Equals(NameExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(Name, other.Name) && Equals(NameExp, other.NameExp);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as NameExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (NameExp?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}