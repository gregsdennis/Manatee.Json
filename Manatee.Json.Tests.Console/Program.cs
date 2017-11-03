using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

[assembly:InternalsVisibleTo("Manatee.Json.DynamicTypes")]

namespace Manatee.Json.Tests.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			dynamic dyn = new ExpandoObject();
			dyn.StringProp = "string";
			dyn.IntProp = 5;
			dyn.NestProp = new ExpandoObject();
			dyn.NestProp.Value = false;

			JsonValue expected = new JsonObject
				{
					["StringProp"] = "string",
					["IntProp"] = 5,
					["NestProp"] = new JsonObject
						{
							["Value"] = false
						}
				};

			var serializer = new JsonSerializer
				{
					Options =
						{
							TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions
						}
				};
			var json = serializer.Serialize<dynamic>(dyn);
			System.Console.WriteLine(json);

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