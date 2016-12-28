using Hipstr.Client.Commands;
using Hipstr.Core.Comparers;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel : ViewModelBase
	{
		public event EventHandler<UserGroup> UserGroupScrollToHeaderRequest;

		public ObservableCollection<UserGroup> UserGroups { get; }
		public ICommand NavigateToUserProfileViewCommand { get; }
		public ICommand JumpToHeaderCommand { get; }
		public ICommand RefreshUsersCommand { get; }

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
		private readonly IDataService _dataService;

		public UsersViewModel(IHipChatService hipChatService, IDataService dataService)
		{
			_hipChatService = hipChatService;
			_dataService = dataService;

			LoadingUsers = false;
			UserGroups = new ObservableCollection<UserGroup>();
			NavigateToUserProfileViewCommand = new NavigateToViewCommand<UserProfileView>();
			JumpToHeaderCommand = new RelayCommandAsync(OnJumpToHeaderCommandAsync);
			RefreshUsersCommand = new RelayCommandAsync(RefreshUsersAsync, () => !LoadingUsers, this, nameof(LoadingUsers));
		}

		// TODO: Commonize Refresh/Cache logic into base class or service
		// TODO: Commonize GroupBy/OrderBy logic into base class or service
		public async Task LoadUsersAsync()
		{
			LoadingUsers = true;
			IEnumerable<UserGroup> userGroups = await _dataService.LoadUserGroupsAsync();
			if (!userGroups.Any())
			{
				userGroups = await RebuildUserGroupCache();
			}
			LoadingUsers = false;

			UserGroups.Clear();
			UserGroups.AddRange(userGroups);
		}

		private async Task RefreshUsersAsync()
		{
			UserGroups.Clear();
			LoadingUsers = true;
			IEnumerable<UserGroup> userGroups = await RebuildUserGroupCache();
			LoadingUsers = false;
			UserGroups.AddRange(userGroups);
		}

		private async Task<IEnumerable<UserGroup>> RebuildUserGroupCache()
		{
			IEnumerable<User> users = await _hipChatService.GetUsersAsync();
			IEnumerable<UserGroup> userGroups = OrderAndGroupUsers(users).ToList();
			await _dataService.SaveUserGroupsAsync(userGroups);

			return userGroups;
		}

		private IEnumerable<UserGroup> OrderAndGroupUsers(IEnumerable<User> users)
		{
			IList<UserGroup> userGroups = users.OrderBy(user => user.Name, UserNameComparer.Instance)
				.GroupBy(user => DetermineGroupHeader(user.Name), user => user,
					(group, groupedUsers) => new UserGroup(group, groupedUsers)).ToList();

			char[] groupNames = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ?".ToCharArray();
			for (var i = 0; i < groupNames.Length; i++)
			{
				string groupName = groupNames[i].ToString();
				if (!userGroups.Any(rg => rg.Header.Equals(groupName)))
				{
					userGroups.Insert(i, new UserGroup(groupName));
				}
			}

			return userGroups;
		}

		private string DetermineGroupHeader(string name)
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

		private async Task OnJumpToHeaderCommandAsync()
		{
			var dialog = new ListGroupJumpDialog();
			ModalResult<string> headerText = await dialog.ShowAsync(UserGroups.Select(ug => new JumpHeader(ug.Header, ug.Users.Any())));
			if (!headerText.Cancelled)
			{
				UserGroupScrollToHeaderRequest?.Invoke(this, UserGroups.Where(ug => ug.Header == headerText.Result).Single());
			}
		}
	}
}