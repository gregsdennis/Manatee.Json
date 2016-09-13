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
 
	File Name:		FilterExpressionQuery.cs
	Namespace:		Manatee.Json.Path.ArrayParameters
	Class Name:		FilterExpressionQuery
	Purpose:		Provides expression-based filtering for array queries.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Path.Expressions;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class FilterExpressionQuery : IJsonPathArrayQuery, IEquatable<FilterExpressionQuery>
	{
		private readonly Expression<bool, JsonValue> _expression;

		public FilterExpressionQuery(Expression<bool, JsonValue> expression)
		{
			_expression = expression;
		}

		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			return json.Where(v => _expression.Evaluate(v, root));
		}
		public override string ToString()
		{
			return $"?({_expression})";
		}
		public bool Equals(FilterExpressionQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(_expression, other._expression);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as FilterExpressionQuery);
		}
		public override int GetHashCode()
		{
			return _expression?.GetHashCode() ?? 0;
		}
	}
}