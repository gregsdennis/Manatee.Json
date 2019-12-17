using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Serialization
{
	[TestFixture]
	public class ConcurrencyTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			JsonOptions.LogCategory = LogCategory.Serialization;
		}

		private class LocalClass
		{
			public int Value1 { get; set; }
			public string Value2 { get; set; }
		}

		private static Task<T[]> _RunConcurrently<T>(Func<T> action1, Func<T> action2)
		{
			return Task.WhenAll(Task.Run(action1), Task.Run(action2));
		}

		[Test]
		public async Task AutoSerializingCachesTypeInfo()
		{
			var serializer = new JsonSerializer();

			var instance1 = new LocalClass
				{
					Value1 = 4,
					Value2 = "test1"
				};
			JsonValue expected1 = new JsonObject
				{
					["Value1"] = 4,
					["Value2"] = "test1"
				};
			var instance2 = new LocalClass
				{
					Value1 = 5,
					Value2 = "test2"
				};
			JsonValue expected2 = new JsonObject
				{
					["Value1"] = 5,
					["Value2"] = "test2"
				};

			var results = await _RunConcurrently(() => serializer.Serialize(instance1),
			                                     () => serializer.Serialize(instance2));

			Assert.AreEqual(expected1, results[0]);
			Assert.AreEqual(expected2, results[1]);
		}
	}
}
