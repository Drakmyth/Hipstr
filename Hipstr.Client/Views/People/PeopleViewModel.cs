using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Views.People
{
	public class PeopleViewModel
	{
		public List<Person> People { get; set; }

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
			List<Person> people = wrapper.Items.Select(userSummary => new Person(userSummary.Name)).ToList();
			People = people;
		}
	}
}