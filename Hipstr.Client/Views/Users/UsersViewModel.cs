using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel : ViewModelBase, ITitled
	{
		private readonly IHipChatService _hipChatService;

		public string Title => "Users";
		public ObservableCollection<User> Users { get; set; }

		public UsersViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }
		public UsersViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateUserList();
		}

		public void UpdateUserList()
		{
			IEnumerable<User> users = _hipChatService.GetUsers();
			Users = new ObservableCollection<User>(users);
		}
	}
}