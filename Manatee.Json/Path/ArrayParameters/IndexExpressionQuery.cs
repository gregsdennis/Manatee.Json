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
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Path.Expressions;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class IndexExpressionQuery : IJsonPathArrayQuery
	{
		private readonly Expression<int, JsonArray> _expression;

		public IndexExpressionQuery(Expression<int, JsonArray> expression)
		{
			_expression = expression;
		}

		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			var index = _expression.Evaluate(json, root);
			return index >= 0 && index < json.Count ? new[] {json[index]} : Enumerable.Empty<JsonValue>();
		}
		public override string ToString()
		{
			return string.Format("({0})", _expression);
		}
	}
}