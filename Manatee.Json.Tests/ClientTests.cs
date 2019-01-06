using System;
using System.Globalization;
using NUnit.Framework;
// ReSharper disable EqualExpressionComparison
// ReSharper disable ExpressionIsAlwaysNull

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class ClientTests
	{
		[Test]
		public void Parse_StringFromSourceForge_kheimric()
		{
			var s = @"{
  ""self"": ""self"",
  ""name"": ""name"",
  ""emailAddress"": ""test at test dot com"",
  ""avatarUrls"": {
	""16x16"": ""http://smallUrl"",
	""48x48"": ""https://largeUrl""
  },
  ""displayName"": ""Display Name"",
  ""active"": true,
  ""timeZone"": ""Europe"",
  ""groups"": {
	""size"": 1,
	""items"": [
	  {
		""name"": ""users""
	  }
	]
  },
  ""expand"": ""groups""
}";
			JsonValue expected = new JsonObject
				{
					{"self", "self"},
					{"name", "name"},
					{"emailAddress", "test at test dot com"},
					{
						"avatarUrls", new JsonObject
							{
								{"16x16", "http://smallUrl"},
								{"48x48", "https://largeUrl"},
							}
					},
					{"displayName", "Display Name"},
					{"active", true},
					{"timeZone", "Europe"},
					{
						"groups", new JsonObject
							{
								{"size", 1},
								{
									"items", new JsonArray
										{
											new JsonObject {{"name", "users"}}
										}
								},
							}
					},
					{"expand", "groups"},
				};

			var actual = JsonValue.Parse(s);
			

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Parse_InternationalCharacters_DaVincious_TrelloIssue19()
		{
			var s = @"""ÅÄÖ""";
			var actual = JsonValue.Parse(s);
			var newString = actual.ToString();

			Console.WriteLine(s);
			Console.WriteLine(actual);
			Assert.AreEqual(s, newString);
		}

		[Test]
		[Ignore("This test no longer applies as of v9.1.0")]
		public void Issue56_InconsistentNullAssignment()
		{
			JsonValue json1 = null;  // this is actually null
			string myVar = null;
			JsonValue json2 = myVar;  // this is JsonValue.Null

			Assert.IsNull(json1);
			Assert.IsTrue(Equals(null, json1));

			Assert.IsNotNull(json2);
			Assert.IsTrue(null == json2);
			// R# isn't considering my == overload
			// ReSharper disable once HeuristicUnreachableCode
			Assert.IsTrue(json2.Equals(null));
			// This may seem inconsistent, but check out the notes in the issue.
			Assert.IsFalse(Equals(null, json2));
		}
	}
}
