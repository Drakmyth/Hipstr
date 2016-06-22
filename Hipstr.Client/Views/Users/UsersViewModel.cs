using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel
	{
		private readonly IHipChatService _hipChatService;

		public ObservableCollection<UserSummary> Users { get; set; }

		public UsersViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }

		public UsersViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateUserList();
		}

		public void UpdateUserList()
		{
			CollectionWrapper<UserSummary> wrapper = _hipChatService.GetUsers();
			Users = new ObservableCollection<UserSummary>(wrapper.Items);
		}
	}
}