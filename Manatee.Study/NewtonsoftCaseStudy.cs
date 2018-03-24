using System;
using System.Collections.Generic;
using Manatee.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Manatee.Study
{
	[TestFixture]
	//[Ignore("This test suite is for development purposes only.")]
	public class NewtonsoftCaseStudy
	{
		private class MyClass
		{
			public string Value { get; set; }
		}

		[Test]
		public void SerializeArrayOfObjects()
		{
			var list = new List<object> {1, "string", false, new MyClass {Value = "hello"}};
			var serialized = JsonConvert.SerializeObject(list);

			Console.WriteLine(serialized);

			var backToList = JsonConvert.DeserializeObject<List<object>>(serialized);

			Assert.AreEqual(list, backToList);
		}

		[Test]
		public void Conversions()
		{
			var target = new
				{
					LongProp = 500,
					stringProp = "hello",
					NestedProp = new
						{
							Culture = "en-nz"
						}
				};

			var jobject = JObject.FromObject(target); // This is a Newtonsoft Json object
			var asDictionary = jobject.ToObject<Dictionary<string, JsonValue>>();
			var jsonValue = new JsonObject(asDictionary);

			Console.WriteLine(jsonValue);
		}
	}
}
