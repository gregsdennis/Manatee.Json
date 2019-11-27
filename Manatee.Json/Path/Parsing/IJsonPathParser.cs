namespace Manatee.Json.Path.Parsing
{
	internal interface IJsonPathParser
	{
		bool Handles(string input, int index);

		bool TryParse(string source, ref int index, ref JsonPath path, out string errorMessage);
	}
}