using System;
using System.Linq;
using Manatee.Json.Pointer;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains a syntax error while parsing a <see cref="JsonObject"/>, <see cref="JsonArray"/>, or <see cref="JsonValue"/>.
	/// </summary>
	public class JsonSyntaxException : Exception
	{
		public string Source { get; }

		/// <summary>
		/// Gets a JSON Pointer to the location at which the error was found.
		/// </summary>
		public JsonPointer Location { get; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>
		/// The error message that explains the reason for the exception, or an empty string("").
		/// </returns>
		public override string Message => $"{base.Message} Path: '{Location}'";

		internal JsonSyntaxException(string message, JsonValue value)
			: base(message)
		{
			Location = _BuildPointer(value);
		}

		internal JsonSyntaxException(string source, string message, JsonValue value)
			: base(message)
		{
			Source = source;
			Location = _BuildPointer(value);
		}

		private static JsonPointer _BuildPointer(JsonValue value)
		{
			var pointer = new JsonPointer();

			if (value == null) return pointer;

			switch (value.Type)
			{
				case JsonValueType.Object:
					if (!value.Object.Any()) return pointer;

					var pair = value.Object.Last();
					var key = pair.Key;
					pointer.Add(key);
					pointer.AddRange(_BuildPointer(pair.Value));
					break;
				case JsonValueType.Array:
					if (!value.Array.Any()) return pointer;

					var item = value.Array.Last();
					var index = value.Array.Count - 1;
					pointer.Add(index.ToString());
					pointer.AddRange(_BuildPointer(item));
					break;
			}

			return pointer;
		}
	}
}