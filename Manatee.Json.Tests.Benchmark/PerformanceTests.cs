using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using JsonSerializer = Manatee.Json.Serialization.JsonSerializer;

namespace Manatee.Json.Tests.Benchmark
{
	public class PerformanceTests
	{
		public class MyClass
		{
			public string Value { get; set; }
			public int OtherValue { get; set; }
			public MyClass NestedValue { get; set; }
		}

		private static readonly JsonSerializer _serializer = new JsonSerializer();

		public static void Run()
		{
			// This initializes whatever caches might be inside the serializer
			//_RunSingle(false);

			//_RunSingle();
			_RunBulk();
		}

		private static void _RunSingle(bool output = true)
		{
			var subjects = new[] {_GenerateSubject()};

			if (output)
				Console.WriteLine("\nNewtonsoft @1");
			//_Run(subjects, JsonConvert.SerializeObject, JsonConvert.DeserializeObject<MyClass>, output);

			if (output)
				Console.WriteLine("\nManatee @1");
			_Run(subjects, _ManateeSerialize, _ManateeDeserialize, output);
		}

		private static void _RunBulk()
		{
			var count = 100000;
			var subjects = Enumerable.Range(0, count).Select(i => _GenerateSubject()).ToList();

			Console.WriteLine($"\nNewtonsoft @{count}");
			_Run(subjects, JsonConvert.SerializeObject, JsonConvert.DeserializeObject<MyClass>);

			Console.WriteLine($"\nManatee @{count}");
			_Run(subjects, _ManateeSerialize, _ManateeDeserialize);
		}

		private static void _Run(IEnumerable<MyClass> subjects, Func<MyClass, string> serialize, Func<string, MyClass> deserialize, bool output = true)
		{
			Thread.Sleep(1);
		
			var watch = new Stopwatch();

			watch.Start();
			var json = subjects.Select(serialize).ToArray();
			watch.Stop();
			
			if (output)
				Console.WriteLine($"  Serialize: {watch.Elapsed}");

			watch.Reset();
			watch.Start();
			var back = json.Select(deserialize).ToArray();
			watch.Stop();

			if (output)
				Console.WriteLine($"  Deserialize: {watch.Elapsed}");
		}

		private static string _ManateeSerialize(MyClass obj)
		{
			var json = _serializer.Serialize(obj);
			return json.ToString();
		}

		private static MyClass _ManateeDeserialize(string jsonString)
		{
			var json = JsonValue.Parse(jsonString);
			return _serializer.Deserialize<MyClass>(json);
		}

		private static MyClass _GenerateSubject(int nest = 0)
		{
			return new MyClass
				{
					Value = Guid.NewGuid().ToString(),
					OtherValue = new Random().Next(),
					NestedValue = nest < 3 && new Random().Next(10)%2 == 0 ? _GenerateSubject(nest + 1) : null
				};
		}
	}
}