using Autofac;
using Autofac.Builder;
using Hipstr.Client.Views.MainPage;
using Hipstr.Client.Views.Messages;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Teams;
using Hipstr.Client.Views.Users;
using Hipstr.Core.Services;

// ReSharper disable InconsistentNaming

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
			builder.RegisterType<IMainPageService, MainPageService>().SingleInstance();

			// HttpClient doesn't use an interface, and I don't really want to write a wrapper
			builder.RegisterType<IHttpClient, HipstrHttpClient>();

			// XAML binding breaks when using an interface as the DataContext, so we
			// need to request implementations rather than interfaces for the view models
			builder.RegisterType<MainPageViewModel, MainPageViewModel>();
			builder.RegisterType<TeamsViewModel, TeamsViewModel>();
			builder.RegisterType<RoomsViewModel, RoomsViewModel>();
			builder.RegisterType<MessagesViewModel, MessagesViewModel>();
			builder.RegisterType<UsersViewModel, UsersViewModel>();
			builder.RegisterType<UserProfileViewModel, UserProfileViewModel>();
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