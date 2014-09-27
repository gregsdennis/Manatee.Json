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
 
	File Name:		JsonArray.cs
	Namespace:		Manatee.Json
	Class Name:		JsonArray
	Purpose:		Represents a collection of JSON values.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.StateMachine;
using Manatee.StateMachine.Exceptions;

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
		enum State
		{
			Start,
			Value,
			End
		}

		private static readonly StateMachine<State, JsonInput> StateMachine = new StateMachine<State, JsonInput>();

		private readonly string _source;
		private int _index;
		private JsonValue _value;
		private bool _done;
		readonly InputStream<JsonInput> _stream = new InputStream<JsonInput>();

		static JsonArray()
		{
			StateMachine[State.Start, JsonInput.OpenBracket] = GotStart;
			StateMachine[State.Value, JsonInput.OpenBrace] = GotValue;
			StateMachine[State.Value, JsonInput.Quote] = GotValue;
			StateMachine[State.Value, JsonInput.Number] = GotValue;
			StateMachine[State.Value, JsonInput.Boolean] = GotValue;
			StateMachine[State.Value, JsonInput.Null] = GotValue;
			StateMachine[State.Value, JsonInput.OpenBracket] = GotValue;
			StateMachine[State.Value, JsonInput.CloseBracket] = GotEmpty;
			StateMachine[State.End, JsonInput.Comma] = GotEnd;
			StateMachine[State.End, JsonInput.CloseBracket] = GotEnd;
			StateMachine.UpdateFunction = GetNextInput;
		}

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

		internal JsonArray(string s, ref int i)
			: this()
		{
			_source = s;
			i = Parse(i);
		}

		/// <summary>
		/// Finalizes memory management responsibilities.
		/// </summary>
		~JsonArray()
		{
			StateMachine.UnregisterOwner(this);
		}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <param name="indentLevel">The indention level for the array.</param>
		/// <returns>A string.</returns>
		public string GetIndentedString(int indentLevel = 0)
		{
			if (Count == 0) return JsonValue.Null.ToString();
			string tab0 = string.Empty.PadLeft(indentLevel, '\t'),
				   tab1 = string.Empty.PadLeft(indentLevel + 1, '\t'),
				   s = "[\n";
			int i;
			for (i = 0; i < Count - 1; i++)
			{
				if (this[i] == null) this[i] = new JsonValue();
				s += string.Format("{0}{1},\n", tab1, this[i].GetIndentedString(indentLevel + 1));
			}
			s += string.Format("{0}{1}\n{2}]", tab1, this[i].GetIndentedString(indentLevel + 1), tab0);
			return s;
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
					throw new JsonSyntaxException(_index);
			}
			catch (InputNotValidForStateException<State, JsonInput> e)
			{
				throw new JsonSyntaxException(_index, e);
			}
			catch (StateNotValidException<State> e)
			{
				throw new JsonSyntaxException(_index, e);
			}
			catch (ActionNotDefinedForStateAndInputException<State, JsonInput> e)
			{
				throw new JsonSyntaxException(_index, e);
			}
			return _index;
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
			return "[" + string.Join(",", this.Select(value => value.ToString()).ToArray()) + "]";
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
			return base.GetHashCode();
		}

		private static void GetNextInput(object owner)
		{
			var array = owner as JsonArray;
			if (array == null) return;
			if (array._done || (array._index == array._source.Length)) return;
			try
			{
				var next = CharacterConverter.Item(array._source[array._index++]);
				array._stream.Add(next);
			}
			catch (KeyNotFoundException)
			{
				throw new JsonSyntaxException(array._index);
			}
		}
		private static State GotStart(object owner, JsonInput input)
		{
			return State.Value;
		}
		private static State GotValue(object owner, JsonInput input)
		{
			var array = owner as JsonArray;
			array._value = JsonValue.Parse(array._source, ref array._index);
			return State.End;
		}
		private static State GotEmpty(object owner, JsonInput input)
		{
			var array = owner as JsonArray;
			array._done = (input == JsonInput.CloseBracket);
			if (array.Count != 0)
				throw new JsonSyntaxException(array._index);
			return State.Value;
		}
		private static State GotEnd(object owner, JsonInput input)
		{
			var array = owner as JsonArray;
			array.Add(array._value);
			array._done = (input == JsonInput.CloseBracket);
			return State.Value;
		}
	}
}
