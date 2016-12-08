using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel : ViewModelBase, ITitled
	{
		private readonly IHipChatService _hipChatService;

		public string Title => "Users";
		public ObservableCollection<User> Users { get; set; }

		public UsersViewModel() : this(IoCContainer.Resolve<IHipChatService>())
		{
		}

		public UsersViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateUserListAsync().Wait();
		}

		public async Task UpdateUserListAsync()
		{
			IEnumerable<User> users = await _hipChatService.GetUsersAsync();
			Users = new ObservableCollection<User>(users);
		}
	}
}