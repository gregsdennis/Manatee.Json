using System;
using System.Linq;
using JetBrains.Annotations;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains a syntax error while parsing a <see cref="JsonObject"/>, <see cref="JsonArray"/>, or <see cref="JsonValue"/>.
	/// </summary>
	public class JsonSyntaxException : Exception
	{
		private readonly string _path;

		/// <summary>
		/// Gets the path up to the point at which the error was found.
		/// </summary>
		public string Path => $"${_path}";

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>
		/// The error message that explains the reason for the exception, or an empty string("").
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string Message => $"{base.Message} Path: '{Path}'";

		[StringFormatMethod("format")]
		internal JsonSyntaxException(string message, JsonValue value)
			: base(message)
		{
			_path = _BuildPath(value);
		}

		private static string _BuildPath(JsonValue value)
		{
			if (value == null) return string.Empty;
			switch (value.Type)
			{
				case JsonValueType.Object:
					if (!value.Object.Any()) return string.Empty;
					var pair = value.Object.Last();
					var key = pair.Key;
					return $".{key}{_BuildPath(pair.Value)}";
				case JsonValueType.Array:
					if (!value.Array.Any()) return string.Empty;
					var item = value.Array.Last();
					var index = value.Array.Count - 1;
					return $"[{index}]{_BuildPath(item)}";
				default:
					return string.Empty;
			}
		}
	}
}