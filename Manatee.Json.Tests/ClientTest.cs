using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		}
	}
}
