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
 
	File Name:		IndexExpressionQuery.cs
	Namespace:		Manatee.Json.Path.ArrayParameters
	Class Name:		IndexExpressionQuery
	Purpose:		Provides expression-based indexing for array queries.

***************************************************************************************/
using System;
using System.Collections.Generic;
using Manatee.Json.Path.Expressions;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class IndexExpressionQuery : IJsonPathArrayQuery
	{
		private readonly Expression<int, JsonArray> _expression;

		public Expression<int, JsonArray> Expression { get { return _expression; } }

		public IndexExpressionQuery(System.Linq.Expressions.Expression<Func<JsonArray, int>> expression)
		{
			_expression = expression;
		}

		public IEnumerable<JsonValue> Find(JsonArray json)
		{
			var index = _expression.Evaluate(json);
			return index >= 0 && index < json.Count ? new[] {json[index]} : null;
		}
		public override string ToString()
		{
			return string.Format("({0})", Expression);
		}
	}
}