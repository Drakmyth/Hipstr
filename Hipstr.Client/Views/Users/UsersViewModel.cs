using Hipstr.Client.Commands;
using Hipstr.Core.Comparers;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel : ViewModelBase
	{
		private readonly IHipChatService _hipChatService;
		private readonly IDataService _dataService;

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

		public ObservableCollection<User> Users { get; set; }

		public UsersViewModel() : this(IoCContainer.Resolve<IHipChatService>(), IoCContainer.Resolve<IDataService>())
		{
		}

		public UsersViewModel(IHipChatService hipChatService, IDataService dataService)
		{
			_hipChatService = hipChatService;
			_dataService = dataService;

			LoadingUsers = false;
			Users = new ObservableCollection<User>();
			RefreshUsersCommand = new RelayCommandAsync(RefreshUsersAsync, () => !LoadingUsers, this, nameof(LoadingUsers));
		}

		public async Task LoadUsersAsync()
		{
			LoadingUsers = true;
			IEnumerable<User> users = await _dataService.LoadUsersAsync();
			if (!users.Any())
			{
				users = await RebuildUserCache();
			}
			LoadingUsers = false;

			Users.Clear();
			Users.AddRange(users);
		}

		public async Task RefreshUsersAsync()
		{
			Users.Clear();
			LoadingUsers = true;
			IEnumerable<User> users = await RebuildUserCache();
			LoadingUsers = false;
			Users.AddRange(users);
		}

		private async Task<IEnumerable<User>> RebuildUserCache()
		{
			IEnumerable<User> users = (await _hipChatService.GetUsersAsync()).OrderBy(user => user.Name, UserNameComparer.Instance).ToList();
			await _dataService.SaveUsersAsync(users);

			return users;
		}
	}
}