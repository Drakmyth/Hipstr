using Autofac;
using Autofac.Builder;
using Hipstr.Core.Services;
using System.Net.Http;

namespace Hipstr.Client
{
	public static class IoCContainer
	{
		private static IContainer Container { get; set; }

		public static void Build()
		{
			if (Container != null) return;

			ContainerBuilder builder = new ContainerBuilder();
			InitializeInjectionMappings(builder);
			Container = builder.Build();
		}

		private static void InitializeInjectionMappings(ContainerBuilder builder)
		{
			builder.RegisterType<ITeamService, TeamService>();
			builder.RegisterType<IHipChatService, HipChatService>();
			builder.RegisterType<INavigationService, NavigationService>().InstancePerLifetimeScope();
			builder.RegisterType<HttpClient, HttpClient>();
		}

		private static IRegistrationBuilder<InstanceType, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<Interface, InstanceType>(this ContainerBuilder builder) where InstanceType : Interface
		{
			return builder.RegisterType<InstanceType>().As<Interface>();
		}

		public static Interface Resolve<Interface>()
		{
			return Container.Resolve<Interface>();
		}
	}
}