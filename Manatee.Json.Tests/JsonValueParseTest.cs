using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonValueParseTest
	{
		private static IEnumerable<(string test, JsonValue expected, string message)> _GetData()
		{
			yield return ("false", new JsonValue(false), null);
			yield return ("true", new JsonValue(true), null);
			yield return ("42", new JsonValue(42), null);
			yield return ("\"a string\"", new JsonValue("a string"), null);
			yield return ("[false,42,\"a string\"]", new JsonValue(new JsonArray {false, 42, "a string"}), null);
			yield return ("{\"bool\":false,\"int\":42,\"string\":\"a string\"}", new JsonValue(new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}}), null);
			yield return ("{\"bool\":false,\"array\":[false, 4a, null],\"string\":\"a string\"}", null, "Expected ',', ']', or '}'. Path: '/array/1'");
			yield return ("null", JsonValue.Null, null);
			yield return ("nulf", null, "Value not recognized: 'nulf'. Path: '/'");
			yield return ("invalid data", null, "Cannot determine type. Path: '/'");
			yield return ("\"An \\\"escaped quote with\\\" spaces\"", new JsonValue("An \"escaped quote with\" spaces"), null);
			yield return ("\"An \\\\escaped\\\\ solidus\"", new JsonValue("An \\escaped\\ solidus"), null);
			yield return ("\"An \\/escaped/ reverse solidus\"", new JsonValue("An /escaped/ reverse solidus"), null);
			yield return ("\"An \\bescaped\\b backspace\"", new JsonValue("An \bescaped\b backspace"), null);
			yield return ("\"An \\fescaped\\f form feed\"", new JsonValue("An \fescaped\f form feed"), null);
			yield return ("\"An \\nescaped\\n new line\"", new JsonValue("An \nescaped\n new line"), null);
			yield return ("\"An \\rescaped\\r carriage return\"", new JsonValue("An \rescaped\r carriage return"), null);
			yield return ("\"An \\tescaped\\t horizontal tab\"", new JsonValue("An \tescaped\t horizontal tab"), null);
			yield return ("\"An \\u25A0escaped\\u25A0 hex value\"", new JsonValue("An " + (char) 0x25A0 + "escaped" + (char) 0x25A0 + " hex value"), null);
			yield return ("\"\\uD83D\\uDCA9\"", new JsonValue(char.ConvertFromUtf32(0x1F4A9)), null);
			yield return ("\"\\uD83D\\uDCA9\\uD83D\\uDCA9\"", new JsonValue(char.ConvertFromUtf32(0x1F4A9) + char.ConvertFromUtf32(0x1F4A9)), null);
			yield return ("\"An \\rescaped\\a carriage return\"", null, "Invalid escape sequence: '\\a'. Path: '/'");
			yield return ("\"some text\\\\\"", new JsonValue("some text\\"), null);
			yield return ("\"\\uA000\\uA000\"", new JsonValue(string.Empty + (char) 0xA000 + (char) 0xA000), null);
			yield return ("{\"data\": \"“Silly Gooseberry!”\"}", new JsonValue(new JsonObject{["data"] = "“Silly Gooseberry!”"}), null);
			}

		public static IEnumerable TestData => _GetData()
			.Select(d => new TestCaseData(d.test, d.expected, d.message));


		#region String Tests

		[TestCaseSource(nameof(TestData))]
		public void Parse_Strings(string test, JsonValue expected, string message)
		{
			if (message == null)
				_RunTest(test, expected);
			else
				_RunFail(test, message);
		}

		private static void _RunTest(string test, JsonValue expected)
		{
			var actual = JsonValue.Parse(test);
			Assert.AreEqual(expected, actual);
		}

		private static void _RunFail(string test, string message)
		{
			var ex = Assert.Throws<JsonSyntaxException>(() =>
				{
					var json = JsonValue.Parse(test);
					Console.WriteLine(json);
				});
			Assert.AreEqual(message, ex.Message);
		}

		#endregion

		#region Stream Tests

		[TestCaseSource(nameof(TestData))]
		public void Parse_Streams(string test, JsonValue expected, string message)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(test), false);
			if (message == null)
				_RunTest(stream, expected);
			else
				_RunFail(stream, message);
		}

		private static void _RunTest(Stream test, JsonValue expected)
		{
			using (var reader = new StreamReader(test))
			{
				var actual = JsonValue.Parse(reader);
				Assert.AreEqual(expected, actual);
			}
		}

		private static void _RunFail(Stream test, string message)
		{
			try
			{
				using (var reader = new StreamReader(test))
				{
					JsonValue.Parse(reader);
				}
				Assert.Fail("Expected an exception.");
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual(message, e.Message);
			}
		}

		[TestCaseSource(nameof(TestData))]
		public async Task Parse_StreamsAsync(string test, JsonValue expected, string message)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(test), false);
			if (message == null)
				await _RunTestAsync(stream, expected);
			else
				await _RunFailAsync(stream, message);
		}

		private static async Task _RunTestAsync(Stream test, JsonValue expected)
		{
			using (var reader = new StreamReader(test))
			{
				var actual = await JsonValue.ParseAsync(reader);
				Assert.AreEqual(expected, actual);
			}
		}

		private static async Task _RunFailAsync(Stream test, string message)
		{
			try
			{
				using (var reader = new StreamReader(test))
				{
					await JsonValue.ParseAsync(reader);
				}
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual(message, e.Message);
			}
		}

		#endregion

		[Test]
		public void Parse_NullString_ThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() => JsonValue.Parse((string) null));
		}
		[Test]
		public void Parse_NullStream_ThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() => JsonValue.Parse((StreamReader) null));
		}
		[Test]
		public void Parse_EmptyString_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => JsonValue.Parse(string.Empty));
		}
		[Test]
		public void Parse_EmptyStream_ThrowsException()
		{
			Assert.Throws<JsonSyntaxException>(() => JsonValue.Parse(StreamReader.Null));
		}
		[Test]
		public void Parse_WhitespaceString_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => JsonValue.Parse("  \t\n"));
		}
		[Test]
		public void Parse_WhitespaceStream_ThrowsException()
		{
			Assert.Throws<JsonSyntaxException>(() =>
				{
					var stream = new MemoryStream(Encoding.UTF8.GetBytes("  \t\n"), false);
					using (var reader = new StreamReader(stream))
					{
						JsonValue.Parse(reader);
					}
				});
		}
		[Test]
		public void Parse_Escaping_String()
		{
			var str = "{\"string\":\"double\\n\\nspaced\"}";
			var json = JsonValue.Parse(str).Object;
			Console.WriteLine(json["string"].String);
		}
		[Test]
		public void Parse_Escaping_Stream()
		{
			var str = "{\"string\":\"double\\n\\nspaced\"}";
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(str), false);
			using (var reader = new StreamReader(stream))
			{
				var json = JsonValue.Parse(reader).Object;
				Console.WriteLine(json["string"].String);
			}
		}
		[Test]
		public void Parse_TrelloCard()
		{
			var fileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\TrelloCard.json").AdjustForOS();
			var str = File.ReadAllText(fileName);
			var json = JsonValue.Parse(str);
			Console.WriteLine(json);
		}
	}
}