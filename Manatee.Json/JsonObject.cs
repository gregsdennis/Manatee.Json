using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Internal;

namespace Manatee.Json
{
	/// <summary>
	/// Represents a collection of key:value pairs in a JSON structure.
	/// </summary>
	/// <remarks>
	/// A key is always represented as a string.  A value can consist of a string, a numerical value, a boolean (true or false), a null placeholder, a JSON array of values, or a nested JSON object.
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
			: base(collection.ToDictionary(kvp => kvp.Key, kvp => kvp.Value ?? JsonValue.Null)) {}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <param name="indentLevel">The indention level for the object.</param>
		/// <returns>A string.</returns>
		public string GetIndentedString(int indentLevel = 0)
		{
			if (Count == 0) return "{}";

			var builder = new StringBuilder();

			AppendIndentedString(builder, indentLevel);

			return builder.ToString();
		}
		internal void AppendIndentedString(StringBuilder builder, int indentLevel = 0)
		{
			if (Count == 0)
			{
				builder.Append("{}");
				return;
			}

			string tab0 = JsonOptions.PrettyPrintIndent.Repeat(indentLevel),
			       tab1 = tab0 + JsonOptions.PrettyPrintIndent;

			builder.Append("{\n");
			bool comma = false;
			foreach (var kvp in this)
			{
				if (comma)
					builder.Append(",\n");

				builder.Append(tab1);
				builder.Append('"');
				builder.Append(kvp.Key.InsertEscapeSequences());
				builder.Append("\" : ");
				kvp.Value.AppendIndentedString(builder, indentLevel + 2);

				comma = true;
			}
			builder.Append('\n');
			builder.Append(tab0);
			builder.Append('}');
		}
		/// <summary>
		/// Adds the specified key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. If the value is null, it will be replaced by <see cref="JsonValue.Null"/>.</param>
		public new void Add(string key, JsonValue value)
		{
			switch (JsonOptions.DuplicateKeyBehavior)
			{
				case DuplicateKeyBehavior.Overwrite:
					this[key] = value ?? JsonValue.Null;
					break;
				case DuplicateKeyBehavior.Throw:
					base.Add(key, value ?? JsonValue.Null);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Creates a string representation of the JSON data.
		/// </summary>
		/// <returns>A string.</returns>
		/// <remarks>
		/// Passing the returned string back into the parser will result in a copy of this JSON object.
		/// </remarks>
		public override string ToString()
		{
			if (Count == 0) return "{}";

			var builder = new StringBuilder();

			AppendString(builder);

			return builder.ToString();
		}
		internal void AppendString(StringBuilder builder)
		{
			if (Count == 0)
			{
				builder.Append("{}");
				return;
			}

			builder.Append('{');
			bool comma = false;
			foreach (var kvp in this)
			{
				if (comma)
					builder.Append(',');

				builder.Append('"');
				builder.Append(kvp.Key.InsertEscapeSequences());
				builder.Append("\":");
				kvp.Value.AppendString(builder);

				comma = true;
			}
			builder.Append('}');
		}
		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>. </param>
		public override bool Equals(object? obj)
		{
			if (!(obj is JsonObject json)) return false;
			if (!Keys.ContentsEqual(json.Keys)) return false;

			return this.All(pair => json[pair.Key].Equals(pair.Value));
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}
