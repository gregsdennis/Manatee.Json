using System.IO;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Console
{
	public class SchemaCommandHandler : ICommandHandler<SchemaCommand>
	{
		public void Execute(SchemaCommand command)
		{
			var serializer = new JsonSerializer();

			var instanceText = File.ReadAllText(command.InstanceFile);
			var instance = JsonValue.Parse(instanceText);

			if (!_TryFindWellKnownSchema(command.SchemaFile, out var schema))
			{
				var schemaText = File.ReadAllText(command.SchemaFile);
				var schemaJson = JsonValue.Parse(schemaText);

				schema = serializer.Deserialize<JsonSchema>(schemaJson);
			}

			JsonSchemaOptions.OutputFormat = command.OutputFormat;
			var results = schema.Validate(instance);

			JsonOptions.PrettyPrintIndent = "  ";
			var output = serializer.Serialize(results).GetIndentedString();

			if (string.IsNullOrWhiteSpace(command.OutputFile))
			{
				System.Console.WriteLine(output);
			}
			else
			{
				File.WriteAllText(command.OutputFile, output);
			}
		}

		private static bool _TryFindWellKnownSchema(string key, out JsonSchema schema)
		{
			switch (key)
			{
				case "draft-04":
					schema = MetaSchemas.Draft04;
					return true;
				case "draft-06":
					schema = MetaSchemas.Draft06;
					return true;
				case "draft-07":
					schema = MetaSchemas.Draft07;
					return true;
				case "draft-2019-09":
					schema = MetaSchemas.Draft2019_09;
					return true;
			}

			schema = null;
			return false;
		}
	}
}