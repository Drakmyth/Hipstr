using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Messages
{
	public class MessagesViewModel : ViewModelBase
	{
		public ObservableCollection<Message> Messages { get; set; }

		private readonly IHipChatService _hipChatService;

		public MessagesViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }
		public MessagesViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;

			Messages = new ObservableCollection<Message>();
		}
	}
}