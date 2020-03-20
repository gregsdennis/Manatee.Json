using System;
using Autofac;
using CommandLine;
using Manatee.Json.Console.Logging;
using TacoPos.Logging;

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
			builder.RegisterModule<LoggingModule>();
			var container = builder.Build();

			try
			{
				var handler = container.Resolve<ICommandHandler<T>>();
				handler.Execute(command);
			}
			catch (Exception e)
			{
				container.Log().Error(() => string.Empty, e);
			}
		}
	}
}
