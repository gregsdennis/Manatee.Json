using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using CommandLine;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Module = Autofac.Module;

namespace Manatee.Json.Console
{
	public class Program
	{
		static void Main(string[] args)
		{

			Parser.Default.ParseArguments<SyntaxCommand, SchemaCommand>(args)
				.WithParsed<SyntaxCommand>(_Execute)
				.WithParsed<SchemaCommand>(_Execute);
		}

		private static void _Execute<T>(T command)
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule<AppModule>();
			var container = builder.Build();

			var handler = container.Resolve<ICommandHandler<T>>();
			handler.Execute(command);
		}
	}

	public class AppModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(ThisAssembly)
				.AsImplementedInterfaces()
				.AsSelf();
		}
	}

	public interface ICommandHandler<in T>
	{
		void Execute(T command);
	}

	[Verb("syntax", HelpText = "Validates the syntax of a JSON document")]
	public class SyntaxCommand
	{
		[Option('j', "json", Required = true, HelpText = "The path to the JSON document")]
		public string JsonFile { get; set; }
	}

	public class SyntaxCommandHandler : ICommandHandler<SyntaxCommand>
	{
		public void Execute(SyntaxCommand command)
		{
			var text = File.ReadAllText(command.JsonFile);
			var json = JsonValue.Parse(text);

			System.Console.WriteLine(json.GetIndentedString());
		}
	}

	[Verb("schema", HelpText = "Validates the content of a JSON document using a JSON Schema document")]
	public class SchemaCommand
	{
		[Option('j', "json", Required = true, HelpText = "The path to the JSON document")]
		public string InstanceFile { get; set; }
		[Option('s', "schema", Required = true, HelpText = "The path to the JSON Schema document")]
		public string SchemaFile { get; set; }
		[Option('f', "output-format", HelpText = "The output format", Default = SchemaValidationOutputFormat.Flag)]
		public SchemaValidationOutputFormat OutputFormat { get; set; }
	}

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

			System.Console.WriteLine(serializer.Serialize(results).GetIndentedString());
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
