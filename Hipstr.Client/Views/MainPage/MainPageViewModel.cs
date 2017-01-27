﻿using Hipstr.Client.Commands;
using Hipstr.Client.Services;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Settings;
using Hipstr.Client.Views.Teams;
using Hipstr.Client.Views.Users;
using JetBrains.Annotations;
using System.Windows.Input;

namespace Hipstr.Client.Views.MainPage
{
	[UsedImplicitly]
	public class MainPageViewModel : ViewModelBase
	{
		public ICommand NavigateToTeamsViewCommand { get; }
		public ICommand NavigateToRoomsViewCommand { get; }
		public ICommand NavigateToUsersViewCommand { get; }
		public ICommand NavigateToSettingsViewCommand { get; }

		private string _title;

		public string Title
		{
			get { return _title; }
			private set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		private readonly IMainPageService _mainPageService;

		public MainPageViewModel(IMainPageService mainPageService) : base("Hipstr")
		{
			_mainPageService = mainPageService;
			_title = "Hipstr";

			NavigateToTeamsViewCommand = new NavigateToViewCommand<TeamsView>
			{
				ClearBackStackOnNavigate = true
			};
			NavigateToRoomsViewCommand = new NavigateToViewCommand<RoomsView>
			{
				ClearBackStackOnNavigate = true,
				BackToSpecificType = typeof(TeamsView),
				ClearNavigationCache = true
			};
			NavigateToUsersViewCommand = new NavigateToViewCommand<UsersView>
			{
				ClearBackStackOnNavigate = true,
				BackToSpecificType = typeof(TeamsView),
				ClearNavigationCache = true
			};
			NavigateToSettingsViewCommand = new NavigateToViewCommand<SettingsView>();
		}

		public override void Initialize()
		{
			base.Initialize();

			_mainPageService.TitleChanged += OnTitleChanged;
		}

		private void OnTitleChanged(object sender, string title)
		{
			Title = title;
		}
	}
}