using Autofac;

namespace Manatee.Json.Console.Logging
{
	public class LoggingModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			Log.InitializeWith<NullLog>();
		}
	}
}
