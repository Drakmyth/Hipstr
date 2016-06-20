
using System.Net.Http;
using Autofac;
using Hipstr.Core.Services;

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
			builder.RegisterType<IHipChatService, HipChatService>();
			builder.RegisterType<HttpClient, HttpClient>();
		}

		private static void RegisterType<Interface, InstanceType>(this ContainerBuilder builder) where InstanceType : Interface
		{
			builder.RegisterType<InstanceType>().As<Interface>();
		}

		public static Interface Resolve<Interface>()
		{
			return Container.Resolve<Interface>();
		}
	}
}