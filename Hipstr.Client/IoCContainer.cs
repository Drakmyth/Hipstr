using Autofac;
using Autofac.Builder;
using Hipstr.Core.Converters;
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

			var builder = new ContainerBuilder();
			InitializeInjectionMappings(builder);
			Container = builder.Build();
		}

		private static void InitializeInjectionMappings(ContainerBuilder builder)
		{
			builder.RegisterType<ITeamService, TeamService>();
			builder.RegisterType<IHipChatService, HipChatService>();
			builder.RegisterType<IUserConverter, UserConverter>();
			builder.RegisterType<IDataService, DataService>();
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