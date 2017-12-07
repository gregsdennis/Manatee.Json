using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class StringParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '\"';
		}

		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
            value = null;

            bool complete = false;
            bool mustInterpret = false;
            int originalIndex = ++index; // eat initial '"'
            while (index < source.Length)
            {
                if (source[index] == '\\')
                {
                    mustInterpret = true;
                    break;
                }
                else if (source[index] == '"')
                {
                    complete = true;
                    break;
                }

                index++;
            }

            if (!mustInterpret)
            {
                if (!complete)
                    return "Could not find end of string value.";

                value = source.Substring(originalIndex, (index - originalIndex));

                index += 1; // eat the trailing '"'
                return null;
            }
            else
            {
                index = originalIndex;
                return TryParseInterpretedString(source, ref index, out value, allowExtraChars);
            }
		}

        private string TryParseInterpretedString(string source, ref int index, out JsonValue value, bool allowExtraChars)
        {
            value = null;

            string errorMessage = null;
            var builder = StringBuilderCache.Acquire();
            var complete = false;
            while (index < source.Length)
            {
                var c = source[index++];
                if (c != '\\')
                {
                    if (c == '"')
                    {
                        complete = true;
                        break;
                    }

                    builder.Append(c);
                }
                else
                {
                    if (index >= source.Length)
                        return "Could not find end of string value.";

                    string append = null;
                    switch (source[index++])
                    {
                        case 'b':
                            append = "\b";
                            break;
                        case 'f':
                            append = "\f";
                            break;
                        case 'n':
                            append = "\n";
                            break;
                        case 'r':
                            append = "\r";
                            break;
                        case 't':
                            append = "\t";
                            break;

                        case 'u':
                            var length = 4;
                            var hex = int.Parse(source.Substring(index, 4), NumberStyles.HexNumber);
                            if (source.IndexOf("\\u", index + 4, 2) == index + 4)
                            {
                                var hex2 = int.Parse(source.Substring(index + 6, 4), NumberStyles.HexNumber);
                                hex = (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
                                length += 6;
                            }
                            append = char.ConvertFromUtf32(hex);
                            index += length;
                            break;

                        case '"':
                            append = "\"";
                            break;
                        case '\\':
                            append = "\\";
                            break;

                            // Is this correct?
                        case '/':
                            append = "/";
                            break;

                        default:
                            errorMessage = $"Invalid escape sequence: '\\{c}'.";
                            break;
                    }

                    if (append != null)
                    {
                        builder.Append(append);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!complete)
            {
                value = null;
                StringBuilderCache.Release(builder);
                return "Could not find end of string value.";
            }
            value = StringBuilderCache.GetStringAndRelease(builder);
            return errorMessage;
        }

        public string TryParse(TextReader stream, out JsonValue value)
		{
            value = null;

            stream.Read(); // waste the '"'

            var builder = StringBuilderCache.Acquire();

            var complete = false;
            bool mustInterpret = false;
            char c;
            while ((c = (char)stream.Peek()) != -1)
            {
                if (c == '\\')
                {
                    mustInterpret = true;
                    break;
                }

                stream.Read(); // eat the character
                if (c == '"')
                {
                    complete = true;
                    break;
                }

                builder.Append(c);
            }

            // if there are not any escape sequences--most of a JSON's strings--just
            // return the string as-is.
            if (!mustInterpret)
            {
                if (!complete)
                    return "Could not find end of string value.";

                value = StringBuilderCache.GetStringAndRelease(builder);
                return null;
            }
            else
            {
                // NOTE: TryParseInterpretedString is responsible for releasing builder
                // NOTE: TryParseInterpretedString assumes stream is sitting at the '\\'
                return TryParseInterpretedString(builder, stream, out value);
            }
        }

        private string TryParseInterpretedString(StringBuilder builder, TextReader stream, out JsonValue value)
        {
            // NOTE: `builder` contains the portion of the string found in `stream`, up to the first
            //       (possible) escape sequence.
            System.Diagnostics.Debug.Assert('\\' == (char)stream.Peek());

            value = null;

            string errorMessage = null;
            bool complete = false;

            int? previousHex = null;

            char c;
            while ((c = (char)stream.Peek()) != -1)
            {
                stream.Read(); // eat this character

                if (c == '\\')
                {
                    // escape sequence
                    var lookAhead = (char)stream.Peek();
                    if (!MustInterpretComplex(lookAhead))
                    {
                        stream.Read(); // eat the simple escape
                        c = InterpretSimpleEscapeSequence(lookAhead);
                    }
                    else
                    {
                        // NOTE: Currently we only handle 'u' here
                        if (lookAhead != 'u')
                        {
                            StringBuilderCache.Release(builder);
                            return $"Invalid escape sequence: '\\{lookAhead}'.";
                        }

                        var buffer = SmallBufferCache.Acquire(4);
                        stream.Read(buffer, 0, 4);
                        var hexString = new string(buffer);
                        var currentHex = int.Parse(hexString, NumberStyles.HexNumber);

                        if (previousHex != null)
                        {
                            // Our last character was \u, so combine and emit the UTF32 codepoint
                            currentHex = ((previousHex - 0xD800) * 0x400 + (currentHex - 0xDC00) % 0x400 + 0x10000).Value;
                            builder.Append(char.ConvertFromUtf32(currentHex));
                            previousHex = null;
                        }
                        else
                        {
                            previousHex = currentHex;
                        }

                        SmallBufferCache.Release(buffer);
                        continue;
                    }
                }
                else if (c == '"')
                {
                    complete = true;
                    break;
                }

                // Check if last character was \u, and if so emit it as-is, because
                // this character is not a continuation of a UTF-32 escape sequence
                if (previousHex != null)
                {
                    builder.Append(char.ConvertFromUtf32(previousHex.Value));
                    previousHex = null;
                }

                // non-escape sequence
                builder.Append(c);
            }

            // if we had a hanging UTF32 escape sequence, apply it now
            if (previousHex != null)
            {
                builder.Append(char.ConvertFromUtf32(previousHex.Value));
            }

            if (!complete)
            {
                value = null;
                StringBuilderCache.Release(builder);
                return "Could not find end of string value.";
            }

            value = StringBuilderCache.GetStringAndRelease(builder);
            return errorMessage;
        }

        /// <summary>
        /// Indicates whether or not the lookahead character is a 
        /// complex escape code.
        /// </summary>
        /// <param name="lookAhead">Lookahead character.</param>
        /// <returns><c>true</c> if and only if <paramref name="lookAhead"/>
        /// would require complex interpretation.</returns>
        private static bool MustInterpretComplex(char lookAhead)
        {
            switch (lookAhead)
            {
                case '"':
                case '/':
                case '\\':
                    // These escape 'as-is'
                    return false;
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                    // These escape as an escape char
                    return false;

                case 'u':
                    // this requires more work...
                    return true;

                default:
                    // this is an error, which our complex handler will report
                    return true;
            }
        }

        private static char InterpretSimpleEscapeSequence(char lookAhead)
        {
            switch (lookAhead)
            {
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                default:
                    return lookAhead;
            }
        }

        public async Task<(string errorMessage, JsonValue value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
            var scratch = SmallBufferCache.Acquire(4);

            await stream.TryRead(scratch, 0, 1); // waste the '"'

            var builder = StringBuilderCache.Acquire();

            var complete = false;
            bool mustInterpret = false;
            char c;
            while ((c = (char)stream.Peek()) != -1)
            {
                if (c == '\\')
                {
                    mustInterpret = true;
                    break;
                }

                await stream.TryRead(scratch, 0, 1); // eat the character
                if (c == '"')
                {
                    complete = true;
                    break;
                }

                builder.Append(c);
            }

            // if there are not any escape sequences--most of a JSON's strings--just
            // return the string as-is.
            if (!mustInterpret)
            {
                SmallBufferCache.Release(scratch);

                string errorMessage = null;
                JsonValue value = null;
                if (!complete)
                {
                    errorMessage = "Could not find end of string value.";
                }
                else
                { 
                    value = StringBuilderCache.GetStringAndRelease(builder);
                }

                return (errorMessage, value);
            }
            else
            {
                // NOTE: TryParseInterpretedString is responsible for releasing builder
                // NOTE: TryParseInterpretedString assumes stream is sitting at the '\\'
                // NOTE: TryParseInterpretedString assumes scratch can hold at least 4 chars
                return await TryParseInterpretedStringAsync(builder, stream, scratch);
            }
		}

        private async Task<(string errorMessage, JsonValue value)> TryParseInterpretedStringAsync(StringBuilder builder, TextReader stream, char[] scratch)
        {
            // NOTE: `builder` contains the portion of the string found in `stream`, up to the first
            //       (possible) escape sequence.
            System.Diagnostics.Debug.Assert('\\' == (char)stream.Peek());
            System.Diagnostics.Debug.Assert(scratch.Length >= 4);

            bool complete = false;

            int? previousHex = null;

            char c;
            while ((c = (char)stream.Peek()) != -1)
            {
                await stream.TryRead(scratch, 0, 1); // eat this character

                if (c == '\\')
                {
                    // escape sequence
                    var lookAhead = (char)stream.Peek();
                    if (!MustInterpretComplex(lookAhead))
                    {
                        await stream.TryRead(scratch, 0, 1); // eat the simple escape
                        c = InterpretSimpleEscapeSequence(lookAhead);
                    }
                    else
                    {
                        // NOTE: Currently we only handle 'u' here
                        if (lookAhead != 'u')
                        {
                            StringBuilderCache.Release(builder);
                            SmallBufferCache.Release(scratch);
                            return ($"Invalid escape sequence: '\\{lookAhead}'.", null);
                        }

                        var charsRead = await stream.ReadAsync(scratch, 0, 4);
                        if (charsRead < 4)
                        {
                            StringBuilderCache.Release(builder);
                            SmallBufferCache.Release(scratch);
                            return ("Could not find end of string value.", null);
                        }

                        var hexString = new string(scratch, 0, 4);
                        var currentHex = int.Parse(hexString, NumberStyles.HexNumber);

                        if (previousHex != null)
                        {
                            // Our last character was \u, so combine and emit the UTF32 codepoint
                            currentHex = ((previousHex - 0xD800) * 0x400 + (currentHex - 0xDC00) % 0x400 + 0x10000).Value;
                            builder.Append(char.ConvertFromUtf32(currentHex));
                            previousHex = null;
                        }
                        else
                        {
                            previousHex = currentHex;
                        }

                        continue;
                    }
                }
                else if (c == '"')
                {
                    complete = true;
                    break;
                }

                // Check if last character was \u, and if so emit it as-is, because
                // this character is not a continuation of a UTF-32 escape sequence
                if (previousHex != null)
                {
                    builder.Append(char.ConvertFromUtf32(previousHex.Value));
                    previousHex = null;
                }

                // non-escape sequence
                builder.Append(c);
            }

            SmallBufferCache.Release(scratch);

            // if we had a hanging UTF32 escape sequence, apply it now
            if (previousHex != null)
            {
                builder.Append(char.ConvertFromUtf32(previousHex.Value));
            }

            if (!complete)
            {
                StringBuilderCache.Release(builder);
                return ("Could not find end of string value.", null);
            }

            JsonValue value = StringBuilderCache.GetStringAndRelease(builder);
            return (null, value);
        }
    }
}
