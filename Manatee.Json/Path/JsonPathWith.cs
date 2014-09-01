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
 
	File Name:		JsonPathWith.cs
	Namespace:		Manatee.Json.Path
	Class Name:		JsonPathWith
	Purpose:		Provides methods to be used when working with JSON Paths.

***************************************************************************************/
using System;
using System.Linq.Expressions;
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Expressions.Translation;
using Manatee.Json.Path.Operators;
using Manatee.Json.Path.SearchParameters;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides methods to be used when working with JSON Paths.
	/// </summary>
	public static class JsonPathWith
	{
		#region Start New Path

		/// <summary>
		/// Creates a new <see cref="JsonPath"/> object which starts by specifying an array length.
		/// </summary>
		/// <returns>A new <see cref="JsonPath"/>.</returns>
		public static JsonPath Length()
		{
			var path = new JsonPath();
			path.Operators.Add(LengthOperator.Instance);
			return path;
		}
		/// <summary>
		/// Creates a new <see cref="JsonPath"/> object which starts by specifying an object property.
		/// </summary>
		/// <param name="name">The name to follow.</param>
		/// <returns>A new <see cref="JsonPath"/>.</returns>
		/// <remarks>If <paramref name="name"/> is "length", operates as <see cref="Length()"/></remarks>
		public static JsonPath Name(string name)
		{
			var path = new JsonPath();
			if (name == "length")
				path.Operators.Add(LengthOperator.Instance);
			else
				path.Operators.Add(new NameOperator(name));
			return path;
		}
		/// <summary>
		/// Creates a new <see cref="JsonPath"/> object which starts by including all object properties.
		/// </summary>
		/// <returns>A new <see cref="JsonPath"/>.</returns>
		public static JsonPath Wildcard()
		{
			var path = new JsonPath();
			path.Operators.Add(WildCardOperator.Instance);
			return path;
		}
		/// <summary>
		/// Creates a new <see cref="JsonPath"/> object which starts by searching for all values.
		/// </summary>
		/// <returns>A new <see cref="JsonPath"/>.</returns>
		public static JsonPath Search()
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(WildCardSearchParameter.Instance));
			return path;
		}
		/// <summary>
		/// Creates a new <see cref="JsonPath"/> object which starts by searching for an object property.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>A new <see cref="JsonPath"/>.</returns>
		/// <remarks>If <paramref name="name"/> is "length", operates as <see cref="SearchLength()"/></remarks>
		public static JsonPath Search(string name)
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(new NameSearchParameter(name)));
			return path;
		}
		/// <summary>
		/// Creates a new <see cref="JsonPath"/> object which starts by searching for array lengths.
		/// </summary>
		/// <returns>A new <see cref="JsonPath"/>.</returns>
		public static JsonPath SearchLength()
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(LengthSearchParameter.Instance));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by including all array values.
		/// </summary>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath SearchArray()
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(new ArraySearchParameter(WildCardQuery.Instance)));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a series of array indicies.
		/// </summary>
		/// <param name="indices">The indices of the <see cref="JsonValue"/>s to include.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath SearchArray(params int[] indices)
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(new ArraySearchParameter(new IndexQuery(indices))));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a series of array indicies using array slice notation.
		/// </summary>
		/// <param name="start">The start index of the <see cref="JsonValue"/>s to include.</param>
		/// <param name="end">The end index of the <see cref="JsonValue"/>s to include.</param>
		/// <param name="step">The index interval of the <see cref="JsonValue"/>s to include.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		/// <remarks>The format for the array slice is [start:end:step].  All parameters are individually optional,
		/// however either the start or end must be defines.  Negative values for start and end indicate that the
		/// iterator should begin counting from the end of the array.</remarks>
		public static JsonPath SearchArraySlice(int? start, int? end, int? step = null)
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(new ArraySearchParameter(new SliceQuery(start, end, step))));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying an expression which evaluates to the index to include.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath SearchArray(Expression<Func<JsonPathArray, int>> expression)
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(new ArraySearchParameter(new IndexExpressionQuery(ExpressionTranslator.Translate(expression)))));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a predicate expression which filters the values.
		/// </summary>
		/// <param name="expression">The predicate expression</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath SearchArray(Expression<Func<JsonPathValue, bool>> expression)
		{
			var path = new JsonPath();
			path.Operators.Add(new SearchOperator(new ArraySearchParameter(new FilterExpressionQuery(ExpressionTranslator.Translate(expression)))));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by including all array values.
		/// </summary>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array()
		{
			var path = new JsonPath();
			path.Operators.Add(new ArrayOperator(WildCardQuery.Instance));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a series of array indicies.
		/// </summary>
		/// <param name="indices">The indices of the <see cref="JsonValue"/>s to include.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(params int[] indices)
		{
			var path = new JsonPath();
			path.Operators.Add(new ArrayOperator(new IndexQuery(indices)));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a series of array indicies using array slice notation.
		/// </summary>
		/// <param name="start">The start index of the <see cref="JsonValue"/>s to include.</param>
		/// <param name="end">The end index of the <see cref="JsonValue"/>s to include.</param>
		/// <param name="step">The index interval of the <see cref="JsonValue"/>s to include.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		/// <remarks>The format for the array slice is [start:end:step].  All parameters are individually optional,
		/// however either the start or end must be defines.  Negative values for start and end indicate that the
		/// iterator should begin counting from the end of the array.</remarks>
		public static JsonPath ArraySlice(int? start, int? end, int? step = null)
		{
			var path = new JsonPath();
			path.Operators.Add(new ArrayOperator(new SliceQuery(start, end, step)));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying an expression which evaluates to the index to include.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(Expression<Func<JsonPathArray, int>> expression)
		{
			var path = new JsonPath();
			path.Operators.Add(new ArrayOperator(new IndexExpressionQuery(ExpressionTranslator.Translate(expression))));
			return path;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a predicate expression which filters the values.
		/// </summary>
		/// <param name="expression">The predicate expression</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(Expression<Func<JsonPathValue, bool>> expression)
		{
			var path = new JsonPath();
			path.Operators.Add(new ArrayOperator(new FilterExpressionQuery(ExpressionTranslator.Translate(expression))));
			return path;
		}

		#endregion

		#region Operators

		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying an array length.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Length(this JsonPath path)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(LengthOperator.Instance);
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying an object property.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <param name="name">The name to follow.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		/// <remarks>If <paramref name="name"/> is "length", operates as <see cref="Length(JsonPath)"/></remarks>
		public static JsonPath Name(this JsonPath path, string name)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			if (name == "length")
				newPath.Operators.Add(LengthOperator.Instance);
			else
				newPath.Operators.Add(new NameOperator(name));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by including all object properties.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Wildcard(this JsonPath path)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(WildCardOperator.Instance);
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by searching for all values.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Search(this JsonPath path)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new SearchOperator(WildCardSearchParameter.Instance));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by searching for an object property.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <param name="name">The name to follow.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		/// <remarks>If <paramref name="name"/> is "length", operates as <see cref="SearchLength(JsonPath)"/></remarks>
		public static JsonPath Search(this JsonPath path, string name)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new SearchOperator(new NameSearchParameter(name)));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by searching for array lengths.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath SearchLength(this JsonPath path)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new SearchOperator(LengthSearchParameter.Instance));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by including all array values.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(this JsonPath path)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new ArrayOperator(WildCardQuery.Instance));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a series of array indicies.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <param name="indices">The indices of the <see cref="JsonValue"/>s to include.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(this JsonPath path, params int[] indices)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new ArrayOperator(new IndexQuery(indices)));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a series of array indicies using array slice notation.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <param name="start">The start index of the <see cref="JsonValue"/>s to include.</param>
		/// <param name="end">The end index of the <see cref="JsonValue"/>s to include.</param>
		/// <param name="step">The index interval of the <see cref="JsonValue"/>s to include.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		/// <remarks>The format for the array slice is [start:end:step].  All parameters are individually optional,
		/// however either the start or end must be defines.  Negative values for start and end indicate that the
		/// iterator should begin counting from the end of the array.</remarks>
		public static JsonPath ArraySlice(this JsonPath path, int? start, int? end, int? step = null)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new ArrayOperator(new SliceQuery(start, end, step)));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying an expression which evaluates to the index to include.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(this JsonPath path, Expression<Func<JsonPathArray, int>> expression)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new ArrayOperator(new IndexExpressionQuery(ExpressionTranslator.Translate(expression))));
			return newPath;
		}
		/// <summary>
		/// Appends a <see cref="JsonPath"/> by specifying a predicate expression which filters the values.
		/// </summary>
		/// <param name="path">The <see cref="JsonPath"/> to extend.</param>
		/// <param name="expression">The predicate expression</param>
		/// <returns>The new <see cref="JsonPath"/>.</returns>
		public static JsonPath Array(this JsonPath path, Expression<Func<JsonPathValue, bool>> expression)
		{
			var newPath = new JsonPath();
			newPath.Operators.AddRange(path.Operators);
			newPath.Operators.Add(new ArrayOperator(new FilterExpressionQuery(ExpressionTranslator.Translate(expression))));
			return newPath;
		}

		#endregion
	}
}