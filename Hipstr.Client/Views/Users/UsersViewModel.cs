using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel : ViewModelBase
	{
		private readonly IHipChatService _hipChatService;

		public ObservableCollection<User> Users { get; set; }

		public UsersViewModel() : this(IoCContainer.Resolve<IHipChatService>())
		{
		}

		public UsersViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			Users = new ObservableCollection<User>();
		}

		public async Task UpdateUserListAsync()
		{
			IEnumerable<User> users = await _hipChatService.GetUsersAsync();
			Users.Clear();
			Users.AddRange(users);
		}
	}
}