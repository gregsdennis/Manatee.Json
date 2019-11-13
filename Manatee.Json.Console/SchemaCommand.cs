using CommandLine;
using Manatee.Json.Schema;

namespace Manatee.Json.Console
{
	[Verb("schema", HelpText = "Validates the content of a JSON document using a JSON Schema document")]
	public class SchemaCommand
	{
		[Option('j', "json", Required = true, HelpText = "The path to the JSON document")]
		public string InstanceFile { get; set; }
		[Option('s', "schema", Required = true, HelpText = "The path to the JSON Schema document")]
		public string SchemaFile { get; set; }
		[Option('f', "output-format", HelpText = "The output format", Default = SchemaValidationOutputFormat.Flag)]
		public SchemaValidationOutputFormat OutputFormat { get; set; }
		[Option('o', "output-file", HelpText = "The path to the output file")]
		public string OutputFile { get; set; }
	}
}