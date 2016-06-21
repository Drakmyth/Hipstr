using System.Collections.Generic;
using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;

namespace Hipstr.Client.Views.Users
{
	public class UsersViewModel
	{
		public IEnumerable<UserSummary> Users { get; set; }

		private readonly IHipChatService _hipChatService;

		public UsersViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }

		public UsersViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateUserList();
		}

		public void UpdateUserList()
		{
			CollectionWrapper<UserSummary> wrapper = _hipChatService.GetUsers();
			Users = wrapper.Items;
		}
	}
}