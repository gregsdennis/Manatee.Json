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

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
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

		private static readonly StateMachine<State, JsonInput> StateMachine = new StateMachine<State, JsonInput>();

		private readonly string _source;
		private string _key;
		private int _index;
		private JsonValue _value;
		private bool _done;
		private readonly InputStream<JsonInput> _stream = new InputStream<JsonInput>();

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
		/// Creates an instance of a JSON object and initializes it with the
		/// supplied JSON values.
		/// </summary>
		/// <param name="collection"></param>
		public JsonObject(IDictionary<string, JsonValue> collection)
			: base(collection) {}
		/// <summary>
		/// Creates an instance of a JSON object and fills it by parsing the
		/// supplied string.
		/// </summary>
		/// <param name="source">A string.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="source"/> is empty or whitespace.</exception>
		/// <exception cref="JsonSyntaxException">Thrown if <paramref name="source"/> contains invalid JSON syntax.</exception>
		/// <exception cref="JsonStringInvalidEscapeSequenceException">Thrown if <paramref name="source"/> contains a
		/// string value with an invalid escape sequence.</exception>
		public JsonObject(string source)
			: this()
		{
			_source = source;
			Parse(0);
		}
		internal JsonObject(string s, ref int i)
			: this()
		{
			_source = s;
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

		private int Parse(int i)
		{
			_stream.Clear();
			_value = null;
			_index = i;
			_done = false;
			try
			{
				StateMachine.Run(this, State.Start, _stream);
				if (!_done)
					throw new JsonSyntaxException("Found incomplete JSON object.");
			}
			catch (InputNotValidForStateException<State, JsonInput> e)
			{
				switch (e.State)
				{
					case State.Start:
						throw new JsonSyntaxException("Expected '{{'.");
					case State.Key:
						if (_key != null)
							throw new JsonSyntaxException("Expected a key. Last key: `{0}`.", _key);
						throw new JsonSyntaxException("Expected a key.");
					case State.Colon:
						throw new JsonSyntaxException("Expected ':' after key '{0}'.", _key);
					case State.Value:
						throw new JsonSyntaxException("Expected a value for key '{0}'", _key);
					case State.End:
						throw new JsonSyntaxException("Expected either ',' or '}}'. Last key: `{0}`.", _key);
					default:
						throw new IndexOutOfRangeException();
				}
			}
			catch (StateNotValidException<State>)
			{
				throw new JsonSyntaxException("An unrecoverable error occurred while parsing a JSON object. Please report to littlecrabsolutions@yahoo.com.");
			}
			catch (ActionNotDefinedForStateAndInputException<State, JsonInput>)
			{
				throw new JsonSyntaxException("An unrecoverable error occurred while parsing a JSON object. Please report to littlecrabsolutions@yahoo.com.");
			}
			catch (JsonSyntaxException e)
			{
				e.PrependPath(string.Format(".{0}", _key));
				throw;
			}
			return _index;
		}

		private static string GetKey(string source, ref int index)
		{
			var temp = source.Substring(index);
			var length = temp.IndexOf('"');
			if (length < 0)
				throw new JsonSyntaxException("Could not find end of string.");
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

		private static void GetNextInput(object owner)
		{
			var obj = (JsonObject) owner;
			if (obj._done || (obj._index == obj._source.Length)) return;
			var c = default(char);
			try
			{
				JsonInput next;
				do
				{
					c = obj._source[obj._index++];
				} while (!CharacterConverter.Item(c, out next));
				obj._stream.Add(next);
			}
			catch (KeyNotFoundException)
			{
				throw new JsonSyntaxException("Unrecognized character '{0}' in input string.  Last key: '{1}'", c, obj._key);
			}
		}
		private static State GotStart(object owner, JsonInput input)
		{
			return State.Key;
		}
		private static State GotKey(object owner, JsonInput input)
		{
			var obj = (JsonObject) owner;
			obj._key = GetKey(obj._source, ref obj._index);
			return State.Colon;
		}
		private static State GotColon(object owner, JsonInput input)
		{
			return State.Value;
		}
		private static State GotValue(object owner, JsonInput input)
		{
			var obj = (JsonObject) owner;
			obj._value = JsonValue.Parse(obj._source, ref obj._index);
			return State.End;
		}
		private static State GotEmpty(object owner, JsonInput input)
		{
			var obj = (JsonObject) owner;
			obj._done = true;
			return State.Value;
		}
		private static State GotEnd(object owner, JsonInput input)
		{
			var obj = (JsonObject) owner;
			obj[obj._key] = obj._value;
			obj._done = (input == JsonInput.CloseBrace);
			return State.Key;
		}
	}
}
