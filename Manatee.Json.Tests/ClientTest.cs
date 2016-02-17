using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Serialization;
using Manatee.Json.Transform;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class ClientTest
	{
		[TestMethod]
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
			var actual = JsonValue.Parse(s);
			var newString = actual.ToString();

			Assert.AreEqual(s, newString);
		}

		[TestMethod]
		public void Parse_InternationalCharacters_DaVincious_TrelloIssue19()
		{
			var s = @"""ÅÄÖ""";
			var actual = JsonValue.Parse(s);
			var newString = actual.ToString();

			Console.WriteLine(s);
			Console.WriteLine(actual);
			Assert.AreEqual(s, newString);
		}

		public enum NoNamedZero
		{
			One = 1,
			Two = 2,
			Three = 3
		}

		[TestMethod]
		public void SerializerTemplateGeneration_EnumWithoutNamedZero_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var expected = "0";

			var actual = serializer.GenerateTemplate<NoNamedZero>();

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DeserializeUnnamedEnumEtry_InspiredBy_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer { Options = { EnumSerializationFormat = EnumSerializationFormat.AsName } };
			JsonValue json = "10";
			var expected = (NoNamedZero) 10;

			var actual = serializer.Deserialize<NoNamedZero>(json);

			Assert.AreEqual(expected, actual);
		}
	}
}
