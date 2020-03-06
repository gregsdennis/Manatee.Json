using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manatee.Json.Tests.Benchmark
{
	class Program
	{
		private static readonly List<ICommandHandler> _commandHandlers =
			typeof(ICommandHandler).Assembly
				.GetTypes()
				.Where(t => typeof(ICommandHandler).IsAssignableFrom(t) && !t.IsAbstract)
				.Select(t => (ICommandHandler) Activator.CreateInstance(t))
				.ToList();

		static async Task Main(string[] args)
		{
			await _ProcessCommands(args);

			Console.ReadLine();
		}

		private static async Task _ProcessCommands(string[] args)
		{
			var commands = args.Select(a => a.Split('='));
			var commandsWithHandlers = commands.Join(_commandHandlers,
			                                  c => c[0],
			                                  h => h.Command,
			                                  (c, h) => new {Command = c, Handler = h})
				.OrderBy(h => h.Handler.Priority);

			foreach (var commandAndHandler in commandsWithHandlers)
			{
				await commandAndHandler.Handler.Run(commandAndHandler.Command.Skip(1).FirstOrDefault());
			}
		}
	}
}
