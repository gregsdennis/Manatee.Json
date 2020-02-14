using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using JsonSchema = Manatee.Json.Schema.JsonSchema;
using JsonSerializer = Manatee.Json.Serialization.JsonSerializer;

namespace Manatee.Json.Tests.Benchmark
{
    public class SchemaPerformance
    {
		private static readonly Stopwatch _manateeWatch = new Stopwatch();
		private static bool _shouldOutput;

		private static readonly JsonSerializer _serializer = new JsonSerializer
		{
			Options =
					{
						CaseSensitiveDeserialization = false
					}
		};

		public static async Task Run()
		{
			await _RunBulk();
		}

		private static async Task _RunBulk()
		{
			var dataString = await File.ReadAllTextAsync("TestData/large_message.json");
			var schemaString = await File.ReadAllTextAsync("TestData/schema.json");

			Console.WriteLine($"Data length: {dataString.Length}");
			Console.WriteLine($"Schema length: {schemaString.Length}");


			var runCount = 5;
			var page = 1;
			for (int i = 0; i < runCount; i++)
			{
				_shouldOutput = i % page == 0 || i == runCount - 1;
				Log($"Run {i}");
				_ExecuteTest(dataString, schemaString);
				Log();
				Log();
			}
		}
		private static void _ExecuteTest(string dataString, string schemaString)
		{
			Log($"\nNewton");
			_Run(dataString, schemaString, _NewtonDeserializeAndValidate, true);

			Log($"\nManatee");
			_Run(dataString, schemaString, _ManateeDeserializeAndValidate, true);
		}

		private static void _Run(string dataString, string schemaString, Action<string, string> deserializeAndValidate, bool details = false)
		{
			Thread.Sleep(1);

			var watch = new Stopwatch();

			watch.Start();
			deserializeAndValidate(dataString, schemaString);
			watch.Stop();

			Log($"  Deserialize and validate: {watch.Elapsed}");

		}
		private static void Log(string message = null)
		{
			if (_shouldOutput)
				Console.WriteLine(message);
		}
		private static void _ManateeDeserializeAndValidate(string dataString, string schemaString)
		{
			_manateeWatch.Reset();
			_manateeWatch.Start();
			var json = JsonValue.Parse(dataString);
			var schemaJson = JsonValue.Parse(schemaString);
			var schema = new JsonSchema();
			schema.FromJson(schemaJson, _serializer);
			var result = schema.Validate(json);

			_manateeWatch.Stop();		
		}
		private static void _NewtonDeserializeAndValidate(string dataString, string schemaString)
		{
			
			_manateeWatch.Reset();
			_manateeWatch.Start();
			JSchema schema = JSchema.Parse(schemaString);

			JObject json = JObject.Parse(dataString);
			var result = json.IsValid(schema);

			_manateeWatch.Stop();

		}
	}
}
