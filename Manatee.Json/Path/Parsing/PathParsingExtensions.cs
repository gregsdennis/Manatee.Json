using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class PathParsingExtensions
	{
		#region GetKey

		private static readonly int[] FibSequence = { 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657 };

		public static string GetKey(this string source, ref int index, out string key)
		{
			return source[index].In('"', '\'')
				       ? _GetQuotedKey(source, ref index, out key)
				       : _GetBasicKey(source, ref index, out key);
		}

		private static string _GetBasicKey(string source, ref int index, out string key)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var sourceLength = source.Length;
			var complete = false;
			var c = (char)0;
			while (index < sourceLength)
			{
				if (bufferIndex == bufferLength)
				{
					var currentLength = bufferLength;
					bufferSize++;
					bufferLength = FibSequence[bufferSize];
					var newBuffer = new char[bufferLength];
					Buffer.BlockCopy(buffer, 0, newBuffer, 0, currentLength * 2);
					buffer = newBuffer;
				}
				c = source[index];
				index++;
				if (!(char.IsLetterOrDigit(c) || c == '_'))
				{
					complete = true;
					index--;
					break;
				}
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			if (!complete && index + 1 < sourceLength)
			{
				key = null;
				return $"The character '{c}' is not supported for unquoted names.";
			}
			var result = new string(buffer, 0, bufferIndex);
			key = result;
			return null;
		}

		private static string _GetQuotedKey(string source, ref int index, out string key)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var sourceLength = source.Length;
			var foundEscape = false;
			var complete = false;
			var quoteChar = source[index];
			index++;
			while (index < sourceLength)
			{
				if (bufferIndex == bufferLength)
				{
					var currentLength = bufferLength;
					bufferSize++;
					bufferLength = FibSequence[bufferSize];
					var newBuffer = new char[bufferLength];
					Buffer.BlockCopy(buffer, 0, newBuffer, 0, currentLength * 2);
					buffer = newBuffer;
				}
				var c = source[index];
				index++;
				if (c == quoteChar && !foundEscape)
				{
					complete = true;
					break;
				}
				foundEscape = c == '\\';
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			if (!complete)
			{
				key = null;
				return "Could not find end of string value.";
			}
			var result = new string(buffer, 0, bufferIndex);
			var errorMessage = result.EvaluateEscapeSequences(out string escaped);
			key = escaped;
			return errorMessage;
		}

		#endregion

		#region GetSlice

		public static string GetSlices(this string source, ref int index, out IList<Slice> slices)
		{
			string error;
			Slice lastSlice;
			slices = new List<Slice>();
			do
			{
				index++;
				error = source._GetSlice(ref index, out lastSlice);
				if (lastSlice != null)
					slices.Add(lastSlice);
			} while (error == null && lastSlice != null);
			if (error != null) return error;

			return !slices.Any() ? "Index required inside '[]'" : null;
		}

		private static string _GetSlice(this string source, ref int index, out Slice slice)
		{
			slice = null;
			if (source[index - 1] == ']') return null;


			var error = _GetInt(source, ref index, out int? n1);
			if (error != null) return error;
			if (index >= source.Length) return "Expected ':', ',', or ']'.";
			if (n1.HasValue && source[index].In(',', ']'))
			{
				slice = new Slice(n1.Value);
				return null;
			}
			if (source[index] != ':') return "Expected ':', ',', or ']'.";

			index++;
			error = _GetInt(source, ref index, out int? n2);
			if (error != null) return error;
			if (source[index].In(',', ']'))
			{
				slice = new Slice(n1, n2);
				return null;
			}
			if (source[index] != ':') return "Expected ':', ',', or ']'.";

			index++;
			error = _GetInt(source, ref index, out int? n3);
			if (error != null) return error;
			if (source[index].In(',', ']'))
			{
				slice = new Slice(n1, n2, n3);
				return null;
			}
			return "Expected ',' or ']'.";
		}
		private static string _GetInt(string source, ref int index, out int? number)
		{
			var text = new string(source.Substring(index).TakeWhile(c => char.IsDigit(c) || c == '-').ToArray());
			if (text.Length == 0 && source[index].In(':', ',', ']'))
			{
				number = null;
				return null;
			}
			if (!int.TryParse(text, out int value))
			{
				number = null;
				return "Expected number.";
			}

			index += text.Length;
			number = value;
			return null;
		}

		#endregion

		#region GetNumber

		private const string NumberChars = "0123456789e.-";

		public static string GetNumber(this string source, ref int index, out double? number)
		{
			var text = new string(source.Substring(index).TakeWhile(c => NumberChars.Contains(c)).ToArray());
			if (text.Length == 0 && source[index].In(':', ',', ']'))
			{
				number = null;
				return null;
			}
			if (!double.TryParse(text, out double value))
			{
				number = null;
				return "Expected number.";
			}

			index += text.Length;
			number = value;
			return null;
		}

		#endregion
	}
}
