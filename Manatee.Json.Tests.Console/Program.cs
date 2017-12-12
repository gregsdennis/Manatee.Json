using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using Humanizer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

[assembly:InternalsVisibleTo("Manatee.Json.DynamicTypes")]

namespace Manatee.Json.Tests.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var stopwatch = new Stopwatch();

			var count = 1000;
			var serializer = new JsonSerializer
				{
					Options =
						{
							DeserializationNameTransform = s => s.Pascalize(),
							SerializationNameTransform = s => s.Camelize(),
							EncodeDefaultValues = true
						}
				};


			stopwatch.Start();

			for (int i = 0; i < count; i++)
			{
				_RunTest(serializer);
			}

			stopwatch.Stop();

			System.Console.WriteLine($"# of runs {count}.");
			System.Console.WriteLine($"Elapsed time: {stopwatch.Elapsed} ({stopwatch.ElapsedTicks}).");

			System.Console.ReadLine();
		}

		private static void _RunTest(JsonSerializer serializer)
		{
			var text = File.ReadAllText("generated.json");
			var json = JsonValue.Parse(text);
			var target = serializer.Deserialize<IEnumerable<Target>>(json);
		}
	}

	internal class Target
	{
		[JsonMapTo("_id")]
		public string Id { get; set; }
		public int Index { get; set; }
		public Guid Guid { get; set; }
		public bool IsActive { get; set; }
		public string Balance { get; set; }
		public Uri Picture { get; set; }
		public int Age { get; set; }
		public EyeColor EyeColor { get; set; }
		public string Name { get; set; }
		public Gender Gender { get; set; }
		public string Company { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string About { get; set; }
		public DateTime Registered { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public List<string> Tags { get; set; }
		public IEnumerable<Friend> Friends { get; set; }
		public string Greeting { get; set; }
		public string FavoriteFruit { get; set; }
	}

	internal enum EyeColor
	{
		Blue,
		Brown,
		Black,
		Green,
		Hazel,
	}

	internal enum Gender
	{
		[Display(Description = "male")]
		Male,
		[Display(Description = "female")]
		Female,
		[Display(Description = "other")]
		IIdentifyAsAHelicopter
	}

	internal class Friend
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}