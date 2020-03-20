using Autofac;

namespace Manatee.Json.Console
{
	public class AppModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(ThisAssembly)
				.AsImplementedInterfaces()
				.AsSelf();
		}
	}
}