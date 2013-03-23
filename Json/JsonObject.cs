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
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;
using Manatee.Json.Helpers;
using Manatee.StateMachine;
using Manatee.StateMachine.Exceptions;

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
		enum State
		{
			Start,
			Key,
			Colon,
			Value,
			End
		}

		static StateMachine<State, JsonInput> StateMachine = new StateMachine<State, JsonInput>();

		string source, key;
		int index;
		JsonValue value;
		bool done;
		private InputStream<JsonInput> stream;

		static JsonObject()
		{
			StateMachine[State.Start, JsonInput.OpenBrace] = GotStart;
			StateMachine[State.Key, JsonInput.Quote] = GotKey;
			StateMachine[State.Key, JsonInput.CloseBrace] = GotEmpty;
			StateMachine[State.Colon, JsonInput.Colon] = GotColon;
			StateMachine[State.Value, JsonInput.OpenBrace] = GotValue;
			StateMachine[State.Value, JsonInput.Quote] = GotValue;
			StateMachine[State.Value, JsonInput.Number] = GotValue;
			StateMachine[State.Value, JsonInput.Boolean] = GotValue;
			StateMachine[State.Value, JsonInput.Null] = GotValue;
			StateMachine[State.Value, JsonInput.OpenBracket] = GotValue;
			StateMachine[State.End, JsonInput.Comma] = GotEnd;
			StateMachine[State.End, JsonInput.CloseBrace] = GotEnd;
			StateMachine.UpdateFunction = GetNextInput;
		}

		/// <summary>
		/// Creates an empty instance of a JSON object.
		/// </summary>
		public JsonObject() {}

		/// <summary>
		/// Creates an instance of a JSON object and fills it by parsing the
		/// supplied string.
		/// </summary>
		/// <param name="s">A string.</param>
		public JsonObject(string s)
			: this()
		{
			source = StripExternalSpaces(s);
			Parse(0);
		}

		internal JsonObject(string s, ref int i)
			: this()
		{
			source = s;
			i = Parse(i);
		}

		/// <summary>
		/// Finalizes memory management responsibilities.
		/// </summary>
		~JsonObject()
		{
			StateMachine.UnregisterOwner(this);
		}

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
				s += string.Format("{0}\"{1}\" :\n{2}{3},\n", tab1, key, tab2, this[key].GetIndentedString(indentLevel + 2));
			}
			key = Keys.ElementAt(i);
			s += string.Format("{0}\"{1}\" :\n{2}{3}\n{4}}}", tab1, key, tab2, this[key].GetIndentedString(indentLevel + 2), tab0);
			return s;
		}

		private static bool IsWhiteSpace(char c)
		{
			return (c == 10) || (c == 13) || (c == 32) || (c == 9);
		}
		private static string StripExternalSpaces(string s)
		{
			var inString = false;
			var ret = "";
			for (var i = 0; i < s.Length; i++)
			{
				if (s[i] == '"') inString = !inString;
				if (inString || !IsWhiteSpace(s[i]))
					ret += s[i];
			}
			return ret;
		}
		private int Parse(int i)
		{
			if (stream == null)
				stream = new InputStream<JsonInput>();
			else
				stream.Clear();
			value = null;
			index = i;
			done = false;
			try
			{
				StateMachine.Run(this, State.Start, stream);
				if (!done)
					throw new JsonSyntaxException(index);
			}
			catch (InputNotValidForStateException<State, JsonInput>)
			{
				throw new JsonSyntaxException(index);
			}
			catch (StateNotValidException<State>)
			{
				throw new JsonSyntaxException(index);
			}
			catch (ActionNotDefinedForStateAndInputException<State, JsonInput>)
			{
				throw new JsonSyntaxException(index);
			}
			return index;
		}

		private static string GetKey(string source, ref int index)
		{
			var temp = source.Substring(index);
			var length = temp.IndexOf('"');
			if (length < 0)
				throw new JsonKeyParseException(index);
			if (length == 0)
			{
				index += 1;
				return string.Empty;
			}
			index += length + 1;
			return temp.Substring(0, length);
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
			return "{" + string.Join(",", from kvp in this
										  select string.Format("\"{0}\":{1}",kvp.Key,kvp.Value)) + "}";
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			var json = obj as JsonObject;
			if (json == null) return false;
			return ((from key in Keys where !json.ContainsKey(key) select key).Count() == 0) &&
				   ((from key in json.Keys where !ContainsKey(key) select key).Count() == 0) &&
				   this.All(pair => json[pair.Key].Equals(pair.Value));
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static void GetNextInput(object owner)
		{
			var obj = owner as JsonObject;
			if (obj == null) return;
			if (obj.done || (obj.index == obj.source.Length)) return;
			try
			{
				var next = CharacterConverter.Item(obj.source[obj.index++]);
				obj.stream.Add(next);
			}
			catch (KeyNotFoundException)
			{
				throw new JsonSyntaxException(obj.index);
			}
		}
		private static State GotStart(object owner, JsonInput input)
		{
			return State.Key;
		}
		private static State GotKey(object owner, JsonInput input)
		{
			var obj = owner as JsonObject;
			obj.key = GetKey(obj.source, ref obj.index);
			return State.Colon;
		}
		private static State GotColon(object owner, JsonInput input)
		{
			return State.Value;
		}
		private static State GotValue(object owner, JsonInput input)
		{
			var obj = owner as JsonObject;
			obj.value = JsonValue.Parse(obj.source, ref obj.index);
			return State.End;
		}
		private static State GotEmpty(object owner, JsonInput input)
		{
			var obj = owner as JsonObject;
			obj.done = (input == JsonInput.CloseBrace);
			if (obj.Count != 0)
				throw new JsonSyntaxException(obj.index);
			return State.Value;
		}
		private static State GotEnd(object owner, JsonInput input)
		{
			var obj = owner as JsonObject;
			obj[obj.key] = obj.value;
			obj.done = (input == JsonInput.CloseBrace);
			return State.Key;
		}

	}
}
