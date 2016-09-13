namespace Manatee.Json.Path.Parsing
{
	internal class WildcardArrayParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("[*]");
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			path = path.Array();
			index += 3;
			return null;
		}
	}
}