﻿using Hipstr.Client.Commands;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Comparers;
using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Users
{
	[UsedImplicitly]
	public class UsersViewModel : ViewModelBase
	{
		private readonly ObservableCollection<User> _users;
		public ObservableCollection<ObservableGroupedUsersCollection> GroupedUsers { get; }
		public ICommand NavigateToUserProfileViewCommand { get; }
		public ICommand NavigateToMessagesViewCommand { get; }
		public ICommand RefreshUsersCommand { get; }

		private User _tappedUser;

		public User TappedUser
		{
			get { return _tappedUser; }
			set
			{
				_tappedUser = value;
				OnPropertyChanged();
			}
		}

		private bool _loadingUsers;

		public bool LoadingUsers
		{
			get { return _loadingUsers; }
			private set
			{
				_loadingUsers = value;
				OnPropertyChanged();
			}
		}

		private readonly IHipChatService _hipChatService;
		private readonly ITeamService _teamService;

		public UsersViewModel(IHipChatService hipChatService, ITeamService teamService)
		{
			_hipChatService = hipChatService;
			_teamService = teamService;

			LoadingUsers = false;

			_users = new ObservableCollection<User>();
			GroupedUsers = new ObservableCollection<ObservableGroupedUsersCollection>();

			NavigateToUserProfileViewCommand = new NavigateToViewCommand<UserProfileView, User>(user => user != null, this, nameof(TappedUser));
			// TODO: Disallow navigating to a 1-on-1 chat with yourself
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView, IMessageSource>(user => user != null);
			RefreshUsersCommand = new RelayCommandAsync(() => RefreshUsersAsync(), () => !LoadingUsers, this, nameof(LoadingUsers));
		}

		public override void Initialize()
		{
			base.Initialize();

			_users.CollectionChanged += UsersOnCollectionChanged;
		}

		public override async Task InitializeAsync()
		{
			if (!GroupedUsers.Any())
			{
				await RefreshUsersAsync(HipChatCacheBehavior.LoadFromCache);
			}
		}

		private void UsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			GroupedUsers.Clear();
			GroupedUsers.AddRange(OrderAndGroupUsers(_users));
		}

		private async Task RefreshUsersAsync(HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.RefreshCache)
		{
			try
			{
				LoadingUsers = true;
				IEnumerable<Team> teams = await _teamService.GetTeamsAsync();
				_users.Clear();
				foreach (Team team in teams)
				{
					IEnumerable<User> users = await _hipChatService.GetUsersForTeamAsync(team, cacheBehavior);

					// Because AddRange is an extension method that just calls Add for each element in the input collection,
					// it will cause the CollectionChanged event to be fired for each element in the input collection. This is
					// REALLY REALLY slow because we reorder and regroup the list whenever that event gets fired. Until the
					// built-in ObservableList supports AddRange properly (only throwing the event once), we can either subclass
					// ObservableList and do it ourselves, or just not OrderAndGroup until we add the last element. For now,
					// we'll do the latter. Note that the former is probably more correct, but subclassing native implementations
					// is ugh...
					// TODO: Stop modifying event subscriptions once we have a better way of handling this
					User lastUser = users.Last();
					_users.CollectionChanged -= UsersOnCollectionChanged;
					_users.AddRange(users.Take(users.Count() - 1));
					_users.CollectionChanged += UsersOnCollectionChanged;
					_users.Add(lastUser);
				}
			}
			finally
			{
				LoadingUsers = false;
			}
		}

		// TODO: Commonize GroupBy/OrderBy logic into base class or service
		private static IEnumerable<ObservableGroupedUsersCollection> OrderAndGroupUsers(IEnumerable<User> users)
		{
			IList<ObservableGroupedUsersCollection> userGroups = users.OrderBy(user => user.Name, UserNameComparer.Instance)
				.GroupBy(user => DetermineGroupHeader(user.Name), user => user,
					(group, groupedUsers) => new ObservableGroupedUsersCollection(group, groupedUsers)).ToList();

			char[] groupNames = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ?".ToCharArray();
			for (var i = 0; i < groupNames.Length; i++)
			{
				string groupName = groupNames[i].ToString();
				if (!userGroups.Any(rg => rg.Header.Equals(groupName)))
				{
					userGroups.Insert(i, new ObservableGroupedUsersCollection(groupName));
				}
			}

			return userGroups;
		}

		private static string DetermineGroupHeader(string name)
		{
			string nameFirstChar = name[0].ToString();

			var alphaRegex = new Regex("^[A-Za-z]$");
			if (alphaRegex.IsMatch(nameFirstChar))
			{
				return nameFirstChar.ToUpper();
			}

			var numRegex = new Regex("^[0-9]$");
			return numRegex.IsMatch(nameFirstChar) ? "#" : "?";
		}
	}
}