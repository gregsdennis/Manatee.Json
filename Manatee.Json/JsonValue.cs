using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
		private readonly bool _boolValue = default!;
		private readonly string _stringValue = default!;
		private readonly double _numberValue = default!;
		private readonly JsonObject _objectValue = default!;
		private readonly JsonArray _arrayValue = default!;

		/// <summary>
		/// Globally defined null-valued JSON value.
		/// </summary>
		/// <remarks>
		/// When adding values to a <see cref="JsonObject"/> or <see cref="JsonArray"/>, nulls will automatically be converted into this field.
		/// </remarks>
#pragma warning disable 618
		public static readonly JsonValue Null = new JsonValue();
#pragma warning restore 618

		/// <summary>
		/// Accesses the <see cref="JsonValue"/> as a boolean.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a boolean.
		/// </exception>
		public bool Boolean
		{
			get
			{
				if (Type != JsonValueType.Boolean && JsonOptions.ThrowOnIncorrectTypeAccess)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Boolean);
				return _boolValue;
			}
		}
		/// <summary>
		/// Accesses the <see cref="JsonValue"/> as a string.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a string.
		/// </exception>
		/// <remarks>
		/// Setting the value as a string will automatically change the <see cref="JsonValue"/>'s type and discard the old data.
		/// </remarks>
		public string String
		{
			get
			{
				if (Type != JsonValueType.String && JsonOptions.ThrowOnIncorrectTypeAccess)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.String);
				return _stringValue;
			}
		}
		/// <summary>
		/// Accesses the <see cref="JsonValue"/> as a numeric value.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a numeric value.
		/// </exception>
		public double Number
		{
			get
			{
				if (Type != JsonValueType.Number && JsonOptions.ThrowOnIncorrectTypeAccess)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Number);
				return _numberValue;
			}
		}
		/// <summary>
		/// Accesses the <see cref="JsonValue"/> as a JSON object.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a Json object.
		/// </exception>
		public JsonObject Object
		{
			get
			{
				if (Type != JsonValueType.Object && JsonOptions.ThrowOnIncorrectTypeAccess)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Object);
				return _objectValue;
			}
		}
		/// <summary>
		/// Accesses the <see cref="JsonValue"/> as a JSON array.
		/// </summary>
		/// <exception cref="JsonValueIncorrectTypeException">
		/// Thrown when this <see cref="JsonValue"/> does not contain a Json array.
		/// </exception>
		public JsonArray Array
		{
			get
			{
				if (Type != JsonValueType.Array && JsonOptions.ThrowOnIncorrectTypeAccess)
					throw new JsonValueIncorrectTypeException(Type, JsonValueType.Array);
				return _arrayValue;
			}
		}
		/// <summary>
		/// Gets the value type of the existing data.
		/// </summary>
		public JsonValueType Type { get; }

		/// <summary>
		/// Creates a null <see cref="JsonValue"/>.
		/// </summary>
		[Obsolete("Use JsonValue.Null instead.")]
		// TODO: make this private
		public JsonValue()
		{
			Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a boolean.
		/// </summary>
#if !DEBUG
		[Obsolete("Future versions will remove the null allowance.")]
#endif
		public JsonValue(bool? b)
		{
			if (b != null)
			{
				Type = JsonValueType.Boolean;
				_boolValue = b.Value;
			}
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a string.
		/// </summary>
#if !DEBUG
		[Obsolete("Future versions will require non-null parameters.")]
#endif
		public JsonValue(string? s)
		{
			if (s != null)
			{
				Type = JsonValueType.String;
				_stringValue = s;
			}
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a numeric value.
		/// </summary>
#if !DEBUG
		[Obsolete("Future versions will remove the null allowance.")]
#endif
		public JsonValue(double? n)
		{
			if (n != null)
			{
				Type = JsonValueType.Number;
				_numberValue = n.Value;
			}
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a JSON object.
		/// </summary>
#if !DEBUG
		[Obsolete("Future versions will require non-null parameters.")]
#endif
		public JsonValue(JsonObject? o)
		{
			if (o != null)
			{
				Type = JsonValueType.Object;
				_objectValue = o;
			}
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a <see cref="JsonValue"/> from a JSON array.
		/// </summary>
#if !DEBUG
		[Obsolete("Future versions will require non-null parameters.")]
#endif
		public JsonValue(JsonArray? a)
		{
			if (a != null)
			{
				Type = JsonValueType.Array;
				_arrayValue = a;
			}
			else Type = JsonValueType.Null;
		}
		/// <summary>
		/// Creates a copy of a <see cref="JsonValue"/>.
		/// </summary>
#if !DEBUG
		[Obsolete("Future versions will require non-null parameters.")]
#endif
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
		internal void AppendIndentedString(StringBuilder builder, int indentLevel)
		{
			switch (Type)
			{
				case JsonValueType.Object:
					_objectValue.AppendIndentedString(builder, indentLevel);
					break;
				case JsonValueType.Array:
					_arrayValue.AppendIndentedString(builder, indentLevel);
					break;
				default:
					AppendString(builder);
					break;
			}
		}

		/// <summary>
		/// Creates a string that represents this <see cref="JsonValue"/>.
		/// </summary>
		/// <returns>A string representation of this <see cref="JsonValue"/>.</returns>
		/// <remarks>
		/// Passing the returned string back into the parser will result in a copy of this <see cref="JsonValue"/>.
		/// </remarks>
		public override string ToString()
		{
			switch (Type)
			{
				case JsonValueType.Number:
					return string.Format(CultureInfo.InvariantCulture, "{0}", _numberValue);
				case JsonValueType.String:
					return string.Concat("\"", _stringValue.InsertEscapeSequences(), "\"");
				case JsonValueType.Boolean:
					return _boolValue ? "true" : "false";
				case JsonValueType.Object:
					return _objectValue.ToString();
				case JsonValueType.Array:
					return _arrayValue.ToString();
				default:
					return "null";
			}
		}
		internal void AppendString(StringBuilder builder)
		{
			switch (Type)
			{
				case JsonValueType.Number:
					builder.AppendFormat(CultureInfo.InvariantCulture, "{0}", _numberValue);
					break;
				case JsonValueType.String:
					builder.Append('"');
					_stringValue.InsertEscapeSequences(builder);
					builder.Append('"');
					break;
				case JsonValueType.Boolean:
					builder.Append(_boolValue ? "true" : "false");
					break;
				case JsonValueType.Object:
					_objectValue.AppendString(builder);
					break;
				case JsonValueType.Array:
					_arrayValue.AppendString(builder);
					break;
				default:
					builder.Append("null");
					break;
			}
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj?.AsJsonValue());
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(JsonValue? other)
		{
			// using a == here would result in recursion and death by stack overflow
			if (ReferenceEquals(other, null)) return false;
			if (other.Type != Type) return false;
			switch (Type)
			{
				case JsonValueType.Number:
					return _numberValue.Equals(other.Number);
				case JsonValueType.String:
					return _stringValue.Equals(other.String);
				case JsonValueType.Boolean:
					return _boolValue.Equals(other.Boolean);
				case JsonValueType.Object:
					return _objectValue.Equals(other.Object);
				case JsonValueType.Array:
					return _arrayValue.Equals(other.Array);
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
		public override int GetHashCode()
		{
			switch (Type)
			{
				case JsonValueType.Number:
					return _numberValue.GetHashCode();
				case JsonValueType.String:
					return _stringValue.GetHashCode();
				case JsonValueType.Boolean:
					return _boolValue.GetHashCode();
				case JsonValueType.Object:
					return _objectValue.GetHashCode();
				case JsonValueType.Array:
					return _arrayValue.GetHashCode();
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
		public static JsonValue Parse(TextReader stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			return JsonParser.Parse(stream);
		}
		/// <summary>
		/// Parses data from a <see cref="StreamReader"/> containing a JSON value.
		/// </summary>
		/// <param name="stream">the <see cref="StreamReader"/> to parse.</param>
		/// <returns>The JSON value represented by the <see cref="string"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> is at the end.</exception>
		/// <exception cref="JsonSyntaxException">Thrown if <paramref name="stream"/> contains invalid JSON syntax.</exception>
		public static Task<JsonValue> ParseAsync(TextReader stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			return JsonParser.ParseAsync(stream);
		}

#pragma warning disable CS8603 // Possible null reference return.
		/// <summary>
		/// Implicitly converts a <see cref="bool"/> into a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="b">A <see cref="bool"/>.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the <see cref="bool"/>.</returns>
		/// <remarks>
		/// This is useful when creating an initialized <see cref="JsonObject"/> or <see cref="JsonArray"/>.
		/// </remarks>
		/// <example>
		/// ```
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// ```
		/// </example>
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
		/// <example>
		/// ```
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// ```
		/// </example>
		public static implicit operator JsonValue(string? s)
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
		/// <example>
		/// ```
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// ```
		/// </example>
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
		/// <example>
		/// ```
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// ```
		/// </example>
		public static implicit operator JsonValue(JsonObject? o)
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
		/// <example>
		/// ```
		/// JsonObject obj = new JsonObject{
		///		{"stringData", "string"},
		///		{"numberData", 10.6},
		///		{"boolData", true},
		///		{"arrayData", new JsonArray{false, "Array String", JsonValue.Null, 8e-4}},
		///		{"objectData", new JsonObject{
		///			{"stringData2", "another string"},
		///			{"moreBoolData", false}}}};
		/// ```
		/// </example>
		public static implicit operator JsonValue(JsonArray? a)
		{
			return a == null ? null : new JsonValue(a);
		}
#pragma warning restore CS8603 // Possible null reference return.
		///<summary>
		/// Performs an equality comparison between two <see cref="JsonValue"/>s.
		///</summary>
		///<param name="a">A JsonValue.</param>
		///<param name="b">A JsonValue.</param>
		///<returns>true if the values are equal; otherwise, false.</returns>
		public static bool operator ==(JsonValue? a, JsonValue? b)
		{
			return ReferenceEquals(a, b) || (a != null && a.Equals(b));
		}
		///<summary>
		/// Performs an inverted equality comparison between two <see cref="JsonValue"/>s.
		///</summary>
		///<param name="a">A JsonValue.</param>
		///<param name="b">A JsonValue.</param>
		///<returns>false if the values are equal; otherwise, true.</returns>
		public static bool operator !=(JsonValue? a, JsonValue? b)
		{
			return !Equals(a, b);
		}

		internal object GetValue()
		{
			switch (Type)
			{
				case JsonValueType.Number:
					return _numberValue;
				case JsonValueType.String:
					return _stringValue;
				case JsonValueType.Boolean:
					return _boolValue;
				case JsonValueType.Object:
					return _objectValue;
				case JsonValueType.Array:
					return _arrayValue;
				default:
					throw new ArgumentOutOfRangeException(nameof(Type));
			}
		}

	}
}
