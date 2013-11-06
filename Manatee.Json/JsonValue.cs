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

		private static readonly IEnumerable<char> AvailableChars = Enumerable.Range(ushort.MinValue, ushort.MaxValue)
																			 .Select(n => (char)n)
																			 .Where(c => !char.IsControl(c));

		private bool _boolValue;
		private string _stringValue;
		private double _numberValue;
		private JsonObject _objectValue;
		private JsonArray _arrayValue;

		/// <summary>
		/// Globally defined null-valued JSON value.
		/// </summary>
		/// <remarks>
		/// Use this when initializing a JSON object or array.
		/// </remarks>
		public static readonly JsonValue Null = new JsonValue();

		/// <summary>
		/// Accesses the <see cref="JsonValue"/> as a boolean.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a boolean.
		/// </exception>
		/// <remarks>
		/// Setting the value as a boolean will automatically change the <see cref="JsonValue"/>'s type and
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
		/// Accesses the <see cref="JsonValue"/> as a string.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a string.
		/// </exception>
		/// <remarks>
		/// Setting the value as a string will automatically change the <see cref="JsonValue"/>'s type and
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
		/// Accesses the <see cref="JsonValue"/> as a numeric value.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a numeric value.
		/// </exception>
		/// <remarks>
		/// Setting the value as a numeric value will automatically change the <see cref="JsonValue"/>'s type and
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
		/// Accesses the <see cref="JsonValue"/> as a JSON object.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a Json object.
		/// </exception>
		/// <remarks>
		/// Setting the value as a JSON object will automatically change the <see cref="JsonValue"/>'s type and
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
		/// Accesses the <see cref="JsonValue"/> as a JSON array.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a Json array.
		/// </exception>
		/// <remarks>
		/// Setting the value as a JSON array will automatically change the <see cref="JsonValue"/>'s type and
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
		/// Creates a null <see cref="JsonValue"/>.
		/// </summary>
		public JsonValue()
		{
			Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a boolean.
		/// </summary>
		public JsonValue(bool? b)
		{
			if (b != null) Boolean = b.Value;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a string.
		/// </summary>
		public JsonValue(string s)
		{
			if (s != null) String = s;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a numeric value.
		/// </summary>
		public JsonValue(double? n)
		{
			if (n != null) Number = n.Value;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a JSON object.
		/// </summary>
		public JsonValue(JsonObject o)
		{
			if (o != null) Object = o;
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a JSON array.
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
		/// Creates a string that represents this <see cref="JsonValue"/>.
		/// </summary>
		/// <returns>A string representation of this <see cref="JsonValue"/>.</returns>
		/// <remarks>
		/// Passing the returned string back into the parser will result in a copy of
		/// this <see cref="JsonValue"/>.
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
		/// Parses a <see cref="string"/> containing a JSON value.
		/// </summary>
		/// <param name="source">the <see cref="string"/> to parse.</param>
		/// <returns>The JSON value represented by the <see cref="string"/>.</returns>
		public static JsonValue Parse(string source)
		{
			var i = 1;
			return Parse(StripExternalSpaces(source), ref i);
		}

		/// <summary>
		/// Implicitly converts a <see cref="bool"/> into a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="b">A <see cref="bool"/>.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the <see cref="bool"/>.</returns>
		/// <remarks>
		/// This is useful when creating an initialized <see cref="JsonObject"/> or <see cref="JsonArray"/>.
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
		/// Implicitly converts a <see cref="string"/> into a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="s">A <see cref="string"/>.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the <see cref="string"/>.</returns>
		/// <remarks>
		/// This is useful when creating an initialized <see cref="JsonObject"/> or <see cref="JsonArray"/>.
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
		/// Implicitly converts a <see cref="double"/> into a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="n">A <see cref="double"/>.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the <see cref="double"/>.</returns>
		/// <remarks>
		/// This is useful when creating an initialized <see cref="JsonObject"/> or <see cref="JsonArray"/>.
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
		/// Implicitly converts a <see cref="JsonObject"/> into a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="o">A JSON object.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the <see cref="JsonObject"/>.</returns>
		/// <remarks>
		/// This is useful when creating an initialized <see cref="JsonObject"/> or <see cref="JsonArray"/>.
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
		/// Implicitly converts a <see cref="JsonArray"/> into a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="a">A JSON array.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the <see cref="JsonArray"/>.</returns>
		/// <remarks>
		/// This is useful when creating an initialized <see cref="JsonObject"/> or <see cref="JsonArray"/>.
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
						return new JsonValue(string.Empty);
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
					switch (temp.ToLower())
					{
						case "true":
							index += temp.Length - 1;
							return new JsonValue(true);
						case "false":
							index += temp.Length - 1;
							return new JsonValue(false);
						case "null":
							index += temp.Length - 1;
							return Null;
						default:
							double d;
							if (!double.TryParse(temp, out d))
								throw new JsonValueParseException(index, temp);
							index += temp.Length - 1;
							return new JsonValue(d);
					}
			}
		}

		private static string EvaluateEscapeSequences(string s)
		{
			var i = 0;
			int length;
			while (i < s.Length)
			{
				length = 1;
				if (s[i] == '\\')
					switch (s[i + 1])
					{
						case '"':
						case '/':
						case '\\':
							s = s.Remove(i, 1);
							break;
						case 'b':
							length = 2;
							s = s.Substring(0, i) + '\b' + s.Substring(i + length);
							break;
						case 'f':
							length = 2;
							s = s.Substring(0, i) + '\f' + s.Substring(i + length);
							break;
						case 'n':
							length = 2;
							s = s.Substring(0, i) + '\n' + s.Substring(i + length);
							break;
						case 'r':
							length = 2;
							s = s.Substring(0, i) + '\r' + s.Substring(i + length);
							break;
						case 't':
							length = 2;
							s = s.Substring(0, i) + '\t' + s.Substring(i + length);
							break;
						case 'u':
							length = 6;
							var hex = int.Parse(s.Substring(i + 2, 4), NumberStyles.HexNumber);
							if (s.Substring(i + 6, 2) == "\\u")
							{
								var hex2 = int.Parse(s.Substring(i + 8, 4), NumberStyles.HexNumber);
								hex = (hex2 - 0xDC00) + ((hex - 0xD800) << 10);
								length += 6;
							}
							s = s.Substring(0, i) + char.ConvertFromUtf32(hex) + s.Substring(i + length);
							break;
					}
				i += length;
			}
			return s;
		}

		private static string InsertEscapeSequences(string s)
		{
			var i = 0;
			while (i < s.Length)
			{
				switch (s[i])
				{
					case '"':
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
			foreach (var t in s)
			{
				if (t == '"') inString = !inString;
				if (inString || !IsWhiteSpace(t))
					ret += t;
			}
			return ret;
		}
	}
}
