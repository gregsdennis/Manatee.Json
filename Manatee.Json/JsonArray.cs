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
 
	File Name:		JsonArray.cs
	Namespace:		Manatee.Json
	Class Name:		JsonArray
	Purpose:		Represents a collection of JSON values.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json
{
	/// <summary>
	/// Represents a collection of JSON values.
	/// </summary>
	/// <remarks>
	/// A value can consist of a string, a numeric value, a boolean (true or false), a null placeholder,
	/// a JSON array of values, or a nested JSON object.
	/// </remarks>
	public class JsonArray : List<JsonValue>
	{
		/// <summary>
		/// Creates an empty instance of a JSON array.
		/// </summary>
		public JsonArray() {}
		/// <summary>
		/// Creates an instance of a JSON array and initializes it with the
		/// supplied JSON values.
		/// </summary>
		/// <param name="collection"></param>
		public JsonArray(IEnumerable<JsonValue> collection)
			: base(collection)
		{
			
		}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <param name="indentLevel">The indention level for the array.</param>
		/// <returns>A string.</returns>
		public string GetIndentedString(int indentLevel = 0)
		{
			if (Count == 0) return "[]";
			string tab0 = string.Empty.PadLeft(indentLevel, '\t'),
				   tab1 = string.Empty.PadLeft(indentLevel + 1, '\t'),
				   s = "[\n";
			int i;
			for (i = 0; i < Count - 1; i++)
			{
				var value = this[i] ?? JsonValue.Null;
				s += $"{tab1}{value.GetIndentedString(indentLevel + 1)},\n";
			}
			s += $"{tab1}{this[i].GetIndentedString(indentLevel + 1)}\n{tab0}]";
			return s;
		}
		/// <summary>
		/// Adds an object to the end of the <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="item">The object to be added to the end of the <see cref="JsonArray"/>.
		/// If the value is null, it will be replaced by <see cref="JsonValue.Null"/>.</param>
		public new void Add(JsonValue item)
		{
			base.Add(item ?? JsonValue.Null);
		}
		/// <summary>
		/// Adds the elements of the specified collection to the end of the <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the
		/// <see cref="JsonArray"/>. The collection itself cannot be null, but it can contain elements
		/// that are null.  These elements will be replaced by <see cref="JsonValue.Null"/></param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
		public new void AddRange(IEnumerable<JsonValue> collection)
		{
			base.AddRange(collection.Select(v => v ?? JsonValue.Null));
		}
		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <returns>A string.</returns>
		/// <remarks>
		/// Passing the returned string back into the parser will result in a copy of
		/// this Json array.
		/// </remarks>
		public override string ToString()
		{
			if (Count == 0) return "[]";
			return "[" + string.Join(",", this.Select(value => value?.ToString() ?? JsonValue.Null.ToString()).ToArray()) + "]";
		}

		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			var json = obj as JsonArray;
			if (json == null) return false;
			return this.SequenceEqual(json);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return this.GetCollectionHashCode();
		}
	}
}
