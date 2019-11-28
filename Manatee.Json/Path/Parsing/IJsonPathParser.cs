using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Parsing
{
	internal interface IJsonPathParser
	{
		bool Handles(string input, int index);

		bool TryParse(string source, ref int index, [NotNullWhen(true)] ref JsonPath? path, [NotNullWhen(false)] out string? errorMessage);
	}
}