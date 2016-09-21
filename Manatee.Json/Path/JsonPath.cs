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
 
	File Name:		JsonPath.cs
	Namespace:		Manatee.Json.Path
	Class Name:		JsonPath
	Purpose:		Provides primary functionality for JSON Path objects.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides primary functionality for JSON Path objects.
	/// </summary>
	public class JsonPath : IEquatable<JsonPath>
	{
		internal List<IJsonPathOperator> Operators { get; } = new List<IJsonPathOperator>();

		/// <summary>
		/// Parses a <see cref="string"/> containing a JSON path.
		/// </summary>
		/// <param name="source">the <see cref="string"/> to parse.</param>
		/// <returns>The JSON path represented by the <see cref="string"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="source"/> is empty or whitespace.</exception>
		/// <exception cref="JsonPathSyntaxException">Thrown if <paramref name="source"/> contains invalid JSON path syntax.</exception>
		public static JsonPath Parse(string source)
		{
			return JsonPathParser.Parse(source);
		}
		/// <summary>
		/// Evaluates a JSON value using the path.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> to evaulate.</param>
		/// <returns></returns>
		public JsonArray Evaluate(JsonValue json)
		{
			var current = new JsonArray {json};
			var found = Operators.Aggregate(current, (c, o) => o.Evaluate(c, json));
			return found;
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return $"${GetRawString()}";
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(JsonPath other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Operators.SequenceEqual(other.Operators);
		}
		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
		/// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			return Equals(obj as JsonPath);
		}
		/// <summary>Serves as a hash function for a particular type. </summary>
		/// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return Operators?.GetCollectionHashCode() ?? 0;
		}

		internal string GetRawString()
		{
			return Operators.Select(o => o.ToString()).Join(string.Empty);
		}
	}
}