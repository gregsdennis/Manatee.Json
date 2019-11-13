using CommandLine;

namespace Manatee.Json.Console
{
	[Verb("syntax", HelpText = "Validates the syntax of a JSON document")]
	public class SyntaxCommand
	{
		[Option('j', "json", Required = true, HelpText = "The path to the JSON document")]
		public string JsonFile { get; set; }
	}
}