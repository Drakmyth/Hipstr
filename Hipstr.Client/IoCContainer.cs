using Autofac;
using Autofac.Builder;
using Hipstr.Client.Views.MainPage;
using Hipstr.Core.Services;
using Hipstr.Core.Utility;

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
			builder.RegisterType<IDataService, DataService>();
			builder.RegisterType<IToastService, ToastService>();
			builder.RegisterType<IHttpClient, HipstrHttpClient>();
			builder.RegisterType<IAppSettings, AppSettings>();

			// XAML binding breaks when using an interface as the DataContext, so we
			// need to request implementations rather than interfaces for the view models
			builder.RegisterType<MainPageViewModel, MainPageViewModel>();
		}

		private static IRegistrationBuilder<TInstanceType, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<TInterface, TInstanceType>(this ContainerBuilder builder) where TInstanceType : TInterface
		{
			return builder.RegisterType<TInstanceType>().As<TInterface>();
		}

		public static TInterface Resolve<TInterface>()
		{
			return Container.Resolve<TInterface>();
		}
	}
}