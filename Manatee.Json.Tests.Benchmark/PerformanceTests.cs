using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = Manatee.Json.Serialization.JsonSerializer;

namespace Manatee.Json.Tests.Benchmark
{
	public static class PerformanceTests
	{
		private static readonly JsonSerializer _serializer = new JsonSerializer
			{
				Options =
					{
						CaseSensitiveDeserialization = false
					}
			};
		private static readonly Stopwatch _manateeWatch = new Stopwatch();
		private static long _parseTime;
		private static long _serializeTime;
		private static long _deserializeTime;
		private static long _toStringTime;
		private static bool _shouldOutput;

		public static async Task Run()
		{
			await _RunBulk();
		}

		private static async Task _RunBulk()
		{
			var data = await GetEmojiData();
			Console.WriteLine($"Data length: {data.Length}");
			Console.WriteLine($"Object count: 1528");

			var runCount = 100;
			var page = 10;
			for (int i = 0; i < runCount; i++)
			{
				_shouldOutput = i % page == 0 || i == runCount - 1;
				Log($"Run {i}");
				_ExecuteTest(data);
				Log();
				Log();
			}
		}

		private static void _ExecuteTest(string data)
		{
			_parseTime = 0;
			_deserializeTime = 0;
			_serializeTime = 0;
			_toStringTime = 0;

			Log($"\nNewtonsoft");
			_Run(data, JsonConvert.SerializeObject, JsonConvert.DeserializeObject<EmojiResponse>);

			Log($"\nManatee");
			_Run(data, _ManateeSerialize, _ManateeDeserialize, true);

		}

		private static void _Run(string data, Func<EmojiResponse, string> serialize, Func<string, EmojiResponse> deserialize, bool details = false)
		{
			Thread.Sleep(1);

			var watch = new Stopwatch();

			watch.Start();
			var objects = deserialize(data);
			watch.Stop();

			Log($"  Deserialize: {watch.Elapsed}");
			if (details)
			{
				Log($"    Parse: {TimeSpan.FromTicks(_parseTime)}");
				Log($"    Deserialize: {TimeSpan.FromTicks(_deserializeTime)}");
			}

			watch.Reset();
			watch.Start();
			var json = serialize(objects);
			watch.Stop();

			Log($"  Serialize: {watch.Elapsed}");
			if (details)
			{
				Log($"    Serialize: {TimeSpan.FromTicks(_serializeTime)}");
				Log($"    ToString: {TimeSpan.FromTicks(_toStringTime)}");
			}
		}

		private static string _ManateeSerialize(EmojiResponse obj)
		{
			_manateeWatch.Reset();
			_manateeWatch.Start();
			var json = _serializer.Serialize(obj);
			_manateeWatch.Stop();
			_serializeTime += _manateeWatch.ElapsedTicks;

			_manateeWatch.Reset();
			_manateeWatch.Start();
			var str = json.ToString();
			_manateeWatch.Stop();
			_toStringTime += _manateeWatch.ElapsedTicks;

			return str;
		}

		private static EmojiResponse _ManateeDeserialize(string jsonString)
		{
			_manateeWatch.Reset();
			_manateeWatch.Start();
			var json = JsonValue.Parse(jsonString);
			_manateeWatch.Stop();
			_parseTime += _manateeWatch.ElapsedTicks;

			_manateeWatch.Reset();
			_manateeWatch.Start();
			var obj = _serializer.Deserialize<EmojiResponse>(json);
			_manateeWatch.Stop();
			_deserializeTime += _manateeWatch.ElapsedTicks;

			return obj;
		}

		private static async Task<string> GetEmojiData()
		{
			using (var client = new HttpClient())
			using (var response = await client.GetAsync("https://api.trello.com/1/emoji?spritesheets=false&key=062109670e7f56b88783721892f8f66f"))
			{
				return await response.Content.ReadAsStringAsync();
			}
		}

		private static void Log(string message = null)
		{
			if (_shouldOutput)
				Console.WriteLine(message);
		}
	}
}