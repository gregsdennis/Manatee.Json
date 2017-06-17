using System;
using System.Runtime.CompilerServices;
using Manatee.Json.Serialization;

//[assembly:InternalsVisibleTo("Manatee.Json.DynamicTypes")]

namespace Manatee.Json.Tests.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var json = new JsonObject
				{
					["test1"] = 1,
					["test2"] = "hello"
				};
			var serializer = new JsonSerializer();
			var obj = serializer.Deserialize<ITest>(json);

			System.Console.WriteLine(obj.Test1);
			System.Console.WriteLine(obj.Test2);

			System.Console.ReadLine();
		}
	}

	internal interface ITest
	{
		[JsonMapTo("test1")]
		int Test1 { get; set; }
		[JsonMapTo("test2")]
		string Test2 { get; set; }

		event EventHandler SomethingHappened;

		void Action(object withParameter);
		int Function(string withParameter);
	}
}