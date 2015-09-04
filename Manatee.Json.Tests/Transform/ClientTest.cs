using System;
using Manatee.Json.Transform;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Transform
{
	[TestClass]
	public class ClientTest
	{
		// http://stackoverflow.com/q/25307714/878701
		[TestMethod]
		public void StackOverflow_25307714()
		{
			JsonValue source = JsonValue.Parse("[{\"Key1\":87,\"Key2\":99,\"Key3\":11},{\"Key1\":42,\"Key2\":-8,\"Key3\":12}]");
			JsonValue template = JsonValue.Parse("[[\"Key1\",\"Key2\",\"Key3\"],[\"$[*]\",\"@.Key1\"],[\"$[*]\",\"@.Key2\"],[\"$[*]\",\"@.Key3\"]]");
			JsonValue reverseTemplate = JsonValue.Parse("[\"$[1][*]\",{\"Key1\":\"@\",\"Key2\":\"$[2][*]\",\"Key3\":\"$[3][*]\"}]");
			JsonValue expected = JsonValue.Parse("[[\"Key1\",\"Key2\",\"Key3\"],[87,42],[99,-8],[11,12]]");
			var result = source.Transform(template);

			Console.WriteLine("Source: {0}", source);
			Console.WriteLine("Template: {0}", template);
			Console.WriteLine("Target:      {0}", expected);
			Console.WriteLine("Transformed: {0}", result);
			Console.WriteLine();
			Console.WriteLine("Reverse Template: {0}", reverseTemplate);
			result = expected.Transform(reverseTemplate);
			Console.WriteLine("Original Source: {0}", source);
			Console.WriteLine("Untransformed:   {0}", result);
		}
	}
}
