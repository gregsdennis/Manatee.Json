using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonSerializer = Manatee.Json.Serialization.JsonSerializer;

namespace Manatee.Study
{
	[TestFixture]
	public class PerformanceTests
	{
		public class MyClass
		{
			public string Value { get; set; }
			public int OtherValue { get; set; }
			public MyClass NestedValue { get; set; }
		}

		private readonly JsonSerializer _serializer = new JsonSerializer();

		public static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				yield return new TestCaseData(new MyClass
					{
						Value = "string",
						OtherValue = 5,
						NestedValue = new MyClass {Value = "other string"}
					});
				yield return new TestCaseData(new MyClass
					{
						Value = "string2",
						OtherValue = 52
					});
			}
		}

		[TestCaseSource(nameof(TestCases))]
		public void RunSingle(MyClass subject)
		{
			Console.WriteLine("Newtonsoft");
			_Run(subject, JsonConvert.SerializeObject, JsonConvert.DeserializeObject<MyClass>, 1);

			Console.WriteLine("\nManatee");
			_Run(subject, _ManateeSerialize, _ManateeDeserialize, 1);
		}

		[TestCaseSource(nameof(TestCases))]
		public void RunBulk(MyClass subject)
		{
			Console.WriteLine("Newtonsoft");
			_Run(subject, JsonConvert.SerializeObject, JsonConvert.DeserializeObject<MyClass>, 10000);

			Console.WriteLine("\nManatee");
			_Run(subject, _ManateeSerialize, _ManateeDeserialize, 10000);
		}

		private static void _Run(MyClass subject, Func<MyClass, string> serialize, Func<string, MyClass> deserialize, int count)
		{
			var watch = new Stopwatch();

			watch.Start();
			string json = null;
			for (int i = 0; i < count; i++)
			{
				json = serialize(subject);
			}
			watch.Stop();

			Console.WriteLine($"  Serialize: {watch.Elapsed} ({watch.ElapsedTicks})");

			watch.Start();
			MyClass back;
			for (int i = 0; i < count; i++)
			{
				back = deserialize(json);
			}
			watch.Stop();

			Console.WriteLine($"  Deserialize: {watch.Elapsed} ({watch.ElapsedTicks})");
		}

		private string _ManateeSerialize(MyClass obj)
		{
			var json = _serializer.Serialize(obj);
			return json.ToString();
		}

		private MyClass _ManateeDeserialize(string jsonString)
		{
			var json = JsonValue.Parse(jsonString);
			return _serializer.Deserialize<MyClass>(json);
		}
	}
}