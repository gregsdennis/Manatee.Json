using System;
using System.IO;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class BoolParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c.In('t', 'T', 'f', 'F');
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			char[] buffer;
			int count;
			if (source[index].In('t', 'T'))
			{
				buffer = new char[4];
				count = 4;
			}
			else
			{
				buffer = new char[5];
				count = 5;
			}
			count = Math.Min(count, source.Length - index);
			for (var i = 0; i < count; i++)
			{
				buffer[i] = source[index + i];
			}
			var result = new string(buffer).ToLower();
			if (result == "true")
			{
				index += 4;
				value = true;
				return null;
			}
			if (result == "false")
			{
				index += 5;
				value = false;
				return null;
			}
			value = null;
			return $"Value not recognized: '{result}'";
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			char[] buffer;
			int count;
			var current = (char) stream.Read();
			if (current.In('t', 'T'))
			{
				buffer = new char[4];
				count = 4;
			}
			else
			{
				buffer = new char[5];
				count = 5;
			}
			buffer[0] = current;
			for (int i = 1; i < count && !stream.EndOfStream; i++)
			{
				buffer[i] = (char) stream.Read();
			}
			if (buffer[count - 1] == (char) 0 && stream.EndOfStream)
			{
				value = null;
				return "Unexpected end of input.";
			}
			var result = new string(buffer).ToLower();
			if (result == "true")
			{
				value = true;
				return null;
			}
			if (result == "false")
			{
				value = false;
				return null;
			}
			value = null;
			return $"Value not recognized: '{result}'";
		}
	}
}