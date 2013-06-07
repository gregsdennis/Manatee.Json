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
 
	File Name:		JsonValue.cs
	Namespace:		Manatee.Json
	Class Name:		JsonValue
	Purpose:		Represents a JSON value.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;

namespace Manatee.Json
{
	/// <summary>
	/// Represents a JSON value.
	/// </summary>
	/// <remarks>
	/// A value can consist of a string, a numerical value, a boolean (true or false), a null
	/// placeholder, a JSON array of values, or a nested JSON object.
	/// </remarks>
	public class JsonValue
	{
		private const string EscapeChars = @"\""/bfnrtu";

		private const string TypeableChars =
			@"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 `-=,./;'[]\~!@#$%^&*()_+<>?:\""{}|";

		private static readonly IEnumerable<char> AvailableChars = Enumerable.Range(UInt16.MinValue, UInt16.MaxValue)
																			 .Select(n => (char)n)
																			 .Where(c => !char.IsControl(c));

		bool _boolValue;
		string _stringValue;
		double _numberValue;
		JsonObject _objectValue;
		JsonArray _arrayValue;

		/// <summary>
		/// Globally defined null-valued JSON value.
		/// </summary>
		/// <remarks>
		/// Use this when initializing a JSON object or array.
		/// </remarks>
		public readonly static JsonValue Null = new JsonValue();

		/// <summary>
		/// Accesses the JsonValue as a boolean.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this JsonValue does not contain a boolean.
		/// </exception>
		/// <remarks>
		/// Setting the value as a boolean will automatically change the JsonValue's type and
		/// discard the old data.
		/// </remarks>
		public bool Boolean
		{
			get
			{
				if (Type != JsonValueType.Boolean)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Boolean);
				return _boolValue;
			}
			private set
			{
				Type = JsonValueType.Boolean;
				_boolValue = value;
			}
		}
		/// <summary>
		/// Accesses the JsonValue as a string.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this JsonValue does not contain a string.
		/// </exception>
		/// <remarks>
		/// Setting the value as a string will automatically change the JsonValue's type and
		/// discard the old data.
		/// </remarks>
		public string String
		{
			get
			{
				if (Type != JsonValueType.String)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.String);
				return _stringValue;
			}
			private set
			{
				Type = JsonValueType.String;
				_stringValue = value;
			}
		}
		/// <summary>
		/// Accesses the JsonValue as a numeric value.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this JsonValue does not contain a numeric value.
		/// </exception>
		/// <remarks>
		/// Setting the value as a numeric value will automatically change the JsonValue's type and
		/// discard the old data.
		/// </remarks>
		public double Number
		{
			get
			{
				if (Type != JsonValueType.Number)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Number);
				return _numberValue;
			}
			private set
			{
				Type = JsonValueType.Number;
				_numberValue = value;
			}
		}
		/// <summary>
		/// Accesses the JsonValue as a JSON object.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this JsonValue does not contain a Json object.
		/// </exception>
		/// <remarks>
		/// Setting the value as a Json object will automatically change the JsonValue's type and
		/// discard the old data.
		/// </remarks>
		public JsonObject Object
		{
			get
			{
				if (Type != JsonValueType.Object)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Object);
				return _objectValue;
			}
			private set
			{
				Type = JsonValueType.Object;
				_objectValue = value;
			}
		}
		/// <summary>
		/// Accesses the JsonValue as a JSON array.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this JsonValue does not contain a Json array.
		/// </exception>
		/// <remarks>
		/// Setting the value as a Json array will automatically change the JsonValue's type and
		/// discard the old data.
		/// </remarks>
		public JsonArray Array
		{
			get
			{
				if (Type != JsonValueType.Array)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Array);
				return _arrayValue;
			}
			private set
			{
				Type = JsonValueType.Array;
				_arrayValue = value;
			}
		}
		/// <summary>
		/// Gets the value type of the existing data.
		/// </summary>
		public JsonValueType Type { get; private set; }

		/// <summary>
		/// Creates a null JsonValue.
		/// </summary>
		public JsonValue()
		{
			Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a JsonValue from a boolean.
		/// </summary>
		public JsonValue(bool? b)
		{
			if (b != null) Boolean = b.Value;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a JsonValue from a string.
		/// </summary>
		public JsonValue(string s)
		{
			if (s != null) String = s;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a JsonValue from a numeric value.
		/// </summary>
		public JsonValue(double? n)
		{
			if (n != null) Number = n.Value;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a JsonValue from a JSON object.
		/// </summary>
		public JsonValue(JsonObject o)
		{
			if (o != null) Object = o;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a JsonValue from a JSON array.
		/// </summary>
		public JsonValue(JsonArray a)
		{
			if (a != null) Array = a;
			else Type = JsonValueType.Null;
		}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <param name="indentLevel">The indention level for the value.</param>
		/// <returns>A string.</returns>
		public string GetIndentedString(int indentLevel = 0)
		{
			switch (Type)
			{
				case JsonValueType.Object:
					return _objectValue.GetIndentedString(indentLevel);
				case JsonValueType.Array:
					return _arrayValue.GetIndentedString(indentLevel);
				default:
					return ToString();
			}
		}

		/// <summary>
		/// Creates a string that represents this JsonValue.
		/// </summary>
		/// <returns>A string representation of this JsonValue.</returns>
		/// <remarks>
		/// Passing the returned string back into the parser will result in a copy of
		/// this JsonValue.
		/// </remarks>
		public override string ToString()
		{
			switch (Type)
			{
				case JsonValueType.Number:
					return string.Format("{0}", _numberValue);
				case JsonValueType.String:
					return string.Format("\"{0}\"", InsertEscapeSequences(_stringValue));
				case JsonValueType.Boolean:
					return _boolValue ? "true" : "false";
				case JsonValueType.Object:
					return string.Format("{0}", _objectValue);
				case JsonValueType.Array:
					return string.Format("{0}", _arrayValue);
				default:
					return "null";
			}
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
			var json = obj as JsonValue;
			if (json == null) return false;
			if (json.Type != Type) return false;
			switch (Type)
			{
				case JsonValueType.Number:
					return Number.Equals(json.Number);
				case JsonValueType.String:
					return String.Equals(json.String);
				case JsonValueType.Boolean:
					return Boolean.Equals(json.Boolean);
				case JsonValueType.Object:
					return Object.Equals(json.Object);
				case JsonValueType.Array:
					return Array.Equals(json.Array);
				case JsonValueType.Null:
					return true;
			}
			return false;
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

		/// <summary>
		/// Parses a string containing a JSON value.
		/// </summary>
		/// <param name="source">the string to parse.</param>
		/// <returns>The JSON value represented by the string.</returns>
		public static JsonValue Parse(string source)
		{
			var i = 1;
			return Parse(StripExternalSpaces(source), ref i);
		}

		/// <summary>
		/// Implicitly converts a boolean into a JsonValue.
		/// </summary>
		/// <param name="b">A boolean.</param>
		/// <returns>A JsonValue that represents the boolean.</returns>
		/// <remarks>
		/// This is useful when creating an initialized JsonObject or JsonArray.
		/// </remarks>
		/// <example><code>
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// </code></example>
		public static implicit operator JsonValue(bool? b)
		{
			return new JsonValue(b);
		}
		/// <summary>
		/// Implicitly converts a string into a JsonValue.
		/// </summary>
		/// <param name="s">A string.</param>
		/// <returns>A JsonValue that represents the string.</returns>
		/// <remarks>
		/// This is useful when creating an initialized JsonObject or JsonArray.
		/// </remarks>
		/// <example><code>
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// </code></example>
		public static implicit operator JsonValue(string s)
		{
			return new JsonValue(s);
		}
		/// <summary>
		/// Implicitly converts a double into a JsonValue.
		/// </summary>
		/// <param name="n">A double.</param>
		/// <returns>A JsonValue that represents the double.</returns>
		/// <remarks>
		/// This is useful when creating an initialized JsonObject or JsonArray.
		/// </remarks>
		/// <example><code>
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// </code></example>
		public static implicit operator JsonValue(double? n)
		{
			return new JsonValue(n);
		}
		/// <summary>
		/// Implicitly converts a JSON object into a JsonValue.
		/// </summary>
		/// <param name="o">A Json object.</param>
		/// <returns>A JsonValue that represents the Json object.</returns>
		/// <remarks>
		/// This is useful when creating an initialized JsonObject or JsonArray.
		/// </remarks>
		/// <example><code>
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// </code></example>
		public static implicit operator JsonValue(JsonObject o)
		{
			return new JsonValue(o);
		}
		/// <summary>
		/// Implicitly converts a JSON array into a JsonValue.
		/// </summary>
		/// <param name="a">A Json array.</param>
		/// <returns>A JsonValue that represents the Json array.</returns>
		/// <remarks>
		/// This is useful when creating an initialized JsonObject or JsonArray.
		/// </remarks>
		/// <example><code>
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// </code></example>
		public static implicit operator JsonValue(JsonArray a)
		{
			return new JsonValue(a);
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(JsonValue a, JsonValue b)
		{
			return ReferenceEquals(a,b) || ((a != null) && (a.Equals(b)));
		}

		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(JsonValue a, JsonValue b)
		{
			return !Equals(a, b);
		}

		internal static JsonValue Parse(string source, ref int index)
		{
			string temp;
			int length;
			switch (source[index-1])
			{
				case '"':										// string
					temp = source.Substring(index);
					if (temp.Length < 2)
						throw new JsonValueParseException(JsonValueType.String, index);
					length = 0;
					var found = false;
					while (!found && length < temp.Length)
					{
						if (temp[length] == '\\')
							if (!EscapeChars.Contains(temp[length + 1].ToString()))
								throw new JsonStringInvalidEscapeSequenceException(temp.Substring(length, 2), index + length);
							else
								if (temp[length + 1] == 'u')
									length += 4;
								else
									length++;
						else found = (temp[length] == '"');
						length++;
					}
					if (!found)
						throw new JsonValueParseException(JsonValueType.String, index);
					if (length == 0)
					{
						index += 2;
						return new JsonValue("");
					}
					index += length;
					return new JsonValue(EvaluateEscapeSequences(temp.Substring(0, length-1)));
				case '{':										// object
					index--;
					return new JsonValue(new JsonObject(source, ref index));
				case '[':										// array
					index--;
					return new JsonValue(new JsonArray(source, ref index));
				default:										// bool, number, null
					temp = source.Substring(index-1);
					length = temp.IndexOfAny(new[] { ',', ']', '}' });
					if (length > 0) temp = temp.Substring(0, length);
					index += temp.Length - 1;
					switch (temp.ToLower())
					{
						case "true":
							return new JsonValue(true);
						case "false":
							return new JsonValue(false);
						case "null":
							return Null;
						default:
							double d;
							if (!double.TryParse(temp, out d))
								throw new JsonValueParseException(JsonValueType.Number, index);
							return new JsonValue(d);
					}
			}
		}

		internal static string EvaluateEscapeSequences(string s)
		{
			var i = 0;
			while (i < s.Length)
			{
				if (s[i] == '\\')
					switch (s[i + 1])
					{
						case '"':
						case '/':
						case '\\':
							s = s.Remove(i, 1);
							break;
						case 'b':
							s = s.Substring(0, i) + '\b' + s.Substring(i + 2);
							break;
						case 'f':
							s = s.Substring(0, i) + '\f' + s.Substring(i + 2);
							break;
						case 'n':
							s = s.Substring(0, i) + '\n' + s.Substring(i + 2);
							break;
						case 'r':
							s = s.Substring(0, i) + '\r' + s.Substring(i + 2);
							break;
						case 't':
							s = s.Substring(0, i) + '\t' + s.Substring(i + 2);
							break;
						case 'u':
							var hex = int.Parse(s.Substring(i + 2, 4), NumberStyles.HexNumber);
							s = s.Substring(0, i) + (char)hex + s.Substring(i + 6);
							break;
					}
				i++;
			}
			return s;
		}

		internal static string InsertEscapeSequences(string s)
		{
			var i = 0;
			while (i < s.Length)
			{
				switch (s[i])
				{
					case '"':
					case '/':
					case '\\':
						s = s.Insert(i, "\\");
						i++;
						break;
					case '\b':
						s = s.Substring(0, i) + "\\b" + s.Substring(i + 1);
						i++;
						break;
					case '\f':
						s = s.Substring(0, i) + "\\f" + s.Substring(i + 1);
						i++;
						break;
					case '\n':
						s = s.Substring(0, i) + "\\n" + s.Substring(i + 1);
						i++;
						break;
					case '\r':
						s = s.Substring(0, i) + "\\r" + s.Substring(i + 1);
						i++;
						break;
					case '\t':
						s = s.Substring(0, i) + "\\t" + s.Substring(i + 1);
						i++;
						break;
					default:
						if (!AvailableChars.Contains(s[i]))
						{
							var hex = Convert.ToInt16(s[i]).ToString("X4");
							s = s.Substring(0, i) + "\\u" + hex + s.Substring(i + 1);
							i += 5;
						}
						break;
				}
				i++;
			}
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
	}
}
