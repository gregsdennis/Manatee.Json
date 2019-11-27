namespace Manatee.Json.Path.Parsing
{
	internal class StartParser : IJsonPathParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '$' || input[index] == '@';
		}

		public bool TryParse(string source, ref int index, ref JsonPath path, out string errorMessage)
		{
			if (path != null)
			{
				errorMessage = "Start token not valid in the middle of path.";
				return false;
			}

			path = new JsonPath();
			index++;
			errorMessage = null!;
			return true;
		}
	}
}
