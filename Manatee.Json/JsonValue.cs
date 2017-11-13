using System;
using System.Globalization;
using System.IO;
using Manatee.Json.Internal;
using Manatee.Json.Parsing;

namespace Manatee.Json
{
	/// <summary>
	/// Represents a JSON value.
	/// </summary>
	/// <remarks>
	/// A value can consist of a string, a numerical value, a boolean (true or false), a null
	/// placeholder, a JSON array of values, or a nested JSON object.
	/// </remarks>
	public class JsonValue : IEquatable<JsonValue>
	{
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
		/// Creates a copy of a <see cref="JsonValue"/>.
		/// </summary>
		public JsonValue(JsonValue other)
		{
			_arrayValue = other._arrayValue;
			_objectValue = other._objectValue;
			_numberValue = other._numberValue;
			_stringValue = other._stringValue;
			_boolValue = other._boolValue;
			Type = other.Type;
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
					return string.Format(CultureInfo.InvariantCulture, "{0}", _numberValue);
				case JsonValueType.String:
					return $"\"{_stringValue.InsertEscapeSequences()}\"";
				case JsonValueType.Boolean:
					return _boolValue ? "true" : "false";
				case JsonValueType.Object:
					return $"{_objectValue}";
				case JsonValueType.Array:
					return $"{_arrayValue}";
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
			return Equals(obj.AsJsonValue());
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(JsonValue other)
		{
			// using a == here would result in recursion and death by stack overflow
			if (ReferenceEquals(other, null)) return false;
			if (other.Type != Type) return false;
			switch (Type)
			{
				case JsonValueType.Number:
					return Number.Equals(other.Number);
				case JsonValueType.String:
					return String.Equals(other.String);
				case JsonValueType.Boolean:
					return Boolean.Equals(other.Boolean);
				case JsonValueType.Object:
					return Object.Equals(other.Object);
				case JsonValueType.Array:
					return Array.Equals(other.Array);
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
			switch (Type)
			{
				case JsonValueType.Number:
					return Number.GetHashCode();
				case JsonValueType.String:
					return String.GetHashCode();
				case JsonValueType.Boolean:
					return Boolean.GetHashCode();
				case JsonValueType.Object:
					return Object.GetHashCode();
				case JsonValueType.Array:
					return Array.GetHashCode();
				case JsonValueType.Null:
					return JsonValueType.Null.GetHashCode();
			}
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		/// <summary>
		/// Parses a <see cref="string"/> containing a JSON value.
		/// </summary>
		/// <param name="source">the <see cref="string"/> to parse.</param>
		/// <returns>The JSON value represented by the <see cref="string"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="source"/> is empty or whitespace.</exception>
		/// <exception cref="JsonSyntaxException">Thrown if <paramref name="source"/> contains invalid JSON syntax.</exception>
		public static JsonValue Parse(string source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (string.IsNullOrWhiteSpace(source))
				throw new ArgumentException("Source string contains no data.");
			return JsonParser.Parse(source);
		}
		/// <summary>
		/// Parses data from a <see cref="StreamReader"/> containing a JSON value.
		/// </summary>
		/// <param name="stream">the <see cref="StreamReader"/> to parse.</param>
		/// <returns>The JSON value represented by the <see cref="string"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> is at the end.</exception>
		/// <exception cref="JsonSyntaxException">Thrown if <paramref name="stream"/> contains invalid JSON syntax.</exception>
		public static JsonValue Parse(StreamReader stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			if (stream.EndOfStream)
				throw new ArgumentException("Source string contains no data.");
			return JsonParser.Parse(stream);
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
			return b == null ? null : new JsonValue(b);
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
			return s == null ? null : new JsonValue(s);
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
			return n == null ? null : new JsonValue(n);
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
			return o == null ? null : new JsonValue(o);
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
			return a == null ? null : new JsonValue(a);
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(JsonValue a, JsonValue b)
		{
			return ReferenceEquals(a, b) || (a != null && a.Equals(b));
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

		internal object GetValue()
		{
			switch (Type)
			{
				case JsonValueType.Number:
					return Number;
				case JsonValueType.String:
					return String;
				case JsonValueType.Boolean:
					return Boolean;
				case JsonValueType.Object:
					return Object;
				case JsonValueType.Array:
					return Array;
			}
			return null;
		}

	}
}
