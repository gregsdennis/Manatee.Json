/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonObject.cs
	Namespace:		Manatee.Json
	Class Name:		JsonObject
	Purpose:		Represents a collection of key:value pairs in a JSON
					structure.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json
{
	/// <summary>
	/// Represents a collection of key:value pairs in a JSON structure.
	/// </summary>
	/// <remarks>
	/// A key is always represented as a string.  A value can consist of a string, a numerical value, 
	/// a boolean (true or false), a null placeholder, a JSON array of values, or a nested JSON object.
	/// </remarks>
	public class JsonObject : Dictionary<string, JsonValue>
	{
		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <returns>The value associated with the specified key.</returns>
		public new JsonValue this[string key]
		{
			get { return base[key]; }
			set { base[key] = value ?? JsonValue.Null; }
		}

		/// <summary>
		/// Creates an empty instance of a JSON object.
		/// </summary>
		public JsonObject() {}
		/// <summary>
		/// Creates an instance of a JSON object and initializes it with the
		/// supplied JSON values.
		/// </summary>
		/// <param name="collection"></param>
		public JsonObject(IDictionary<string, JsonValue> collection)
			: base(collection) {}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <param name="indentLevel">The indention level for the object.</param>
		/// <returns>A string.</returns>
		public string GetIndentedString(int indentLevel = 0)
		{
			if (Count == 0) return string.Empty;
			string key, tab0 = string.Empty.PadLeft(indentLevel, '\t'),
				   tab1 = string.Empty.PadLeft(indentLevel + 1, '\t'),
				   tab2 = string.Empty.PadLeft(indentLevel + 2, '\t'),
				   s = "{\n";
			int i;
			for (i = 0; i < Count - 1; i++)
			{
				key = Keys.ElementAt(i);
				if (this[key] == null) this[key] = new JsonValue();
				s += string.Format("{0}\"{1}\" : {3},\n", tab1, key, tab2, this[key].GetIndentedString(indentLevel + 2));
			}
			key = Keys.ElementAt(i);
			s += string.Format("{0}\"{1}\" : {3}\n{4}}}", tab1, key, tab2, this[key].GetIndentedString(indentLevel + 2), tab0);
			return s;
		}
		/// <summary>
		/// Adds the specified key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
		public new void Add(string key, JsonValue value)
		{
			base.Add(key, value ?? JsonValue.Null);
		}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <returns>A string.</returns>
		/// <remarks>
		/// Passing the returned string back into the parser will result in a copy of
		/// this JSON object.
		/// </remarks>
		public override string ToString()
		{
			if (Count == 0) return "{}";
			return "{" + string.Join(",", this.Select(kvp => string.Format("\"{0}\":{1}", kvp.Key, kvp.Value)).ToArray()) + "}";
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
			var json = obj as JsonObject;
			if (json == null) return false;
			return Keys.All(json.ContainsKey) && (Keys.Count == json.Keys.Count) &&
			       this.All(pair => json[pair.Key].Equals(pair.Value));
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
			return base.GetHashCode();
		}
	}
}
