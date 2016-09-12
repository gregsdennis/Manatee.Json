namespace Manatee.Json.Path.Parsing
{
	internal interface IJsonPathParser
	{
		bool Handles(string input);

		string TryParse(string source, ref int index, ref JsonPath path);
	}
}