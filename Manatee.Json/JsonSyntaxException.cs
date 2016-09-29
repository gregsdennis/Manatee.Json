/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSyntaxException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonSyntaxException
	Purpose:		Thrown when an input string contains a syntax error while
					parsing a <see cref="JsonObject"/>, <see cref="JsonArray"/>,
					or <see cref="JsonValue"/>.

***************************************************************************************/

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains a syntax error while parsing a <see cref="JsonObject"/>, <see cref="JsonArray"/>, or <see cref="JsonValue"/>.
	/// </summary>
#if !IOS
	[Serializable]
#endif
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
			_path = BuildPath(value);
		}

		private static string BuildPath(JsonValue value)
		{
			if (value == null) return string.Empty;
			switch (value.Type)
			{
				case JsonValueType.Object:
					if (!value.Object.Any()) return string.Empty;
					var pair = value.Object.Last();
					var key = pair.Key;
					return $".{key}{BuildPath(pair.Value)}";
				case JsonValueType.Array:
					if (!value.Array.Any()) return string.Empty;
					var item = value.Array.Last();
					var index = value.Array.Count - 1;
					return $"[{index}]{BuildPath(item)}";
				default:
					return string.Empty;
			}
		}
	}
}