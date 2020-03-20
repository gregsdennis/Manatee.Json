namespace Manatee.Json.Console
{
	public interface ICommandHandler<in T>
	{
		void Execute(T command);
	}
}