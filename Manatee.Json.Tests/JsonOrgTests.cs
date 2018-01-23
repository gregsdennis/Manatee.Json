using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonOrgTests
	{
		[Test]
		[Ignore("not ready yet.")]
		public async Task JsonCheckerTest()
		{
			var text = await new HttpClient().GetStringAsync("http://www.json.org/JSON_checker/test/pass1.json");

			// Just make sure the parser doesn't fail.
			var json = JsonValue.Parse(text);
		}
	}
}
