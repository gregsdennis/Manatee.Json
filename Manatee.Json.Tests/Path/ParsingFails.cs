using System;
using Manatee.Json.Path;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path
{
	[TestFixture]
	public class ParsingFails
	{
		[Test]
		[TestCase("$[]")]
		[TestCase("$[()]")]
		[TestCase("$[?()]")]
		[TestCase("$.")]
		[TestCase("$.tes*t")]
		// TODO: This may actually be the key to parsing bracket notation
		[TestCase("$[\"test\"]")]
		[TestCase("$[false]")]
		[TestCase("$[1.test")]
		[TestCase("$[1-5]")]
		[TestCase("$[(1]")]
		[TestCase("$[(1)")]
		[TestCase("$[(1).test")]
		[TestCase("$[?(@.name == 4].test")]
		[TestCase("$[?(@.name == 4)")]
		[TestCase("$[?(@.name == 4).test")]
		[TestCase("$name")]
		[TestCase("$.[0]")]
		[TestCase("$...name")]
		[TestCase("$[?(@[1,3] == 5)]")]
		[TestCase("$[?(@[1:3] == 5)]")]
		[TestCase("$[?(@[(@.length-1))]")]
		[TestCase("$[?(@[?(@.name == 5))]")]
		[TestCase("$..")]
		public static void Run(string text)
		{
			Assert.Throws<JsonPathSyntaxException>(() =>
				{
					try
					{
						Console.WriteLine($"\n{JsonPath.Parse(text)}");
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
						throw;
					}
				});
		}
	}
}
