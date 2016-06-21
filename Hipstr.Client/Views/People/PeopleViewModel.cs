using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Views.People
{
	public class PeopleViewModel
	{
		public List<User> Users { get; set; }

		private readonly IHipChatService _hipChatService;

		public PeopleViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }

		public PeopleViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateUserList();
		}

		public void UpdateUserList()
		{
			CollectionWrapper<UserSummary> wrapper = _hipChatService.GetUsers();
			List<User> users = wrapper.Items.Select(userSummary => new User(userSummary.Name)).ToList();
			Users = users;
		}
	}
}