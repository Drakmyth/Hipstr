﻿using Autofac;
using Autofac.Builder;
using Hipstr.Client.Views.Dialogs.AddRoomDialog;
using Hipstr.Client.Views.Dialogs.AddTeamDialog;
using Hipstr.Client.Views.Dialogs.EditTeamDialog;
using Hipstr.Client.Views.MainPage;
using Hipstr.Client.Views.Messages;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Settings;
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
			builder.RegisterType<IToastService, ToastService>();
			builder.RegisterType<IHttpClient, HipstrHttpClient>();
			builder.RegisterType<IAppSettings, AppSettings>();

			// XAML binding breaks when using an interface as the DataContext, so we
			// need to request implementations rather than interfaces for the view models
			builder.RegisterType<MainPageViewModel, MainPageViewModel>();
			builder.RegisterType<TeamsViewModel, TeamsViewModel>();
			builder.RegisterType<RoomsViewModel, RoomsViewModel>();
			builder.RegisterType<MessagesViewModel, MessagesViewModel>();
			builder.RegisterType<UsersViewModel, UsersViewModel>();
			builder.RegisterType<UserProfileViewModel, UserProfileViewModel>();
			builder.RegisterType<AddTeamDialogViewModel, AddTeamDialogViewModel>();
			builder.RegisterType<EditTeamDialogViewModel, EditTeamDialogViewModel>();
			builder.RegisterType<AddRoomDialogViewModel, AddRoomDialogViewModel>();
			builder.RegisterType<SettingsViewModel, SettingsViewModel>();
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