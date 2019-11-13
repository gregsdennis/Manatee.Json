using System.IO;
using TacoPos.Logging;

namespace Manatee.Json.Console
{
	public class SyntaxCommandHandler : ICommandHandler<SyntaxCommand>
	{
		public void Execute(SyntaxCommand command)
		{
			var text = File.ReadAllText(command.JsonFile);
			var json = JsonValue.Parse(text);

			this.Log().Info(json.GetIndentedString());
		}
	}
}