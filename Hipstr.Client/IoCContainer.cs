using Autofac;
using Hipstr.Client.Views.Dialogs.AddRoomDialog;
using Hipstr.Client.Views.Dialogs.AddTeamDialog;
using Hipstr.Client.Views.Dialogs.EditTeamDialog;
using Hipstr.Client.Views.Dialogs.JoinChatDialog;
using Hipstr.Client.Views.MainPage;
using Hipstr.Client.Views.Messages;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Settings;
using Hipstr.Client.Views.Subscriptions;
using Hipstr.Client.Views.Teams;
using Hipstr.Client.Views.Users;
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
			builder.RegisterType<ISubscriptionService, SubscriptionService>();
			builder.RegisterType<IToastService, ToastService>();
			builder.RegisterType<IHttpClient, HipstrHttpClient>();
			builder.RegisterType<IAppSettings, AppSettings>();

			// XAML binding breaks when using an interface as the DataContext, so we
			// need to request implementations rather than interfaces for the view models
			builder.RegisterType<NewMainPageViewModel, NewMainPageViewModel>();
			builder.RegisterType<NewTeamsViewModel, NewTeamsViewModel>();
			builder.RegisterType<RoomsViewModel, RoomsViewModel>();
			builder.RegisterType<SubscriptionsViewModel, SubscriptionsViewModel>();
			builder.RegisterType<MessagesViewModel, MessagesViewModel>();
			builder.RegisterType<UserProfileViewModel, UserProfileViewModel>();
			builder.RegisterType<AddTeamDialogViewModel, AddTeamDialogViewModel>();
			builder.RegisterType<EditTeamDialogViewModel, EditTeamDialogViewModel>();
			builder.RegisterType<AddRoomDialogViewModel, AddRoomDialogViewModel>();
			builder.RegisterType<JoinChatDialogViewModel, JoinChatDialogViewModel>();
			builder.RegisterType<SettingsViewModel, SettingsViewModel>();
		}

		private static void RegisterType<TInterface, TInstanceType>(this ContainerBuilder builder) where TInstanceType : TInterface
		{
			builder.RegisterType<TInstanceType>().As<TInterface>();
		}

		public static TInterface Resolve<TInterface>()
		{
			return Container.Resolve<TInterface>();
		}
	}
}