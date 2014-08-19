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
 
	File Name:		FieldExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		FieldExpression
	Purpose:		Expresses the intent to retrieve a value from a field.

***************************************************************************************/
using System.Reflection;

namespace Manatee.Json.Path.Expressions
{
	internal class FieldExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 5; } }
		public FieldInfo Field { get; set; }
		public object Source { get; set; }

		public override object Evaluate(T json)
		{
			return Field.GetValue(Source);
		}
		public override string ToString()
		{
			var value = Evaluate(default(T));
			return value is string
					   ? string.Format("\"{0}\"", value)
					   : value.ToString();
		}
	}
}