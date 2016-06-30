using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Views.Messages
{
	public class MessagesViewModel : ViewModelBase//, ITitled
	{
		private List<Message> _messages;
		public List<Message> Messages
		{
			get { return _messages; }
			set
			{
				OnPropertyChanging();
				_messages = value;
				OnPropertyChanged();
			}
		}

		//		private string _title;
		//		public string Title
		//		{
		//			get { return _title; }
		//			set
		//			{
		//				OnPropertyChanging();
		//				_title = value;
		//				OnPropertyChanged();
		//			}
		//		}

		private Room _room;
		public Room Room
		{
			get { return _room; }
			set
			{
				OnPropertyChanging();
				_room = value;
				OnPropertyChanged();
				OnRoomChanged();
			}
		}

		private readonly IHipChatService _hipChatService;

		public MessagesViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }
		public MessagesViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;

			Messages = new List<Message>();
		}

		private void OnRoomChanged()
		{
			//			Title = _room.Name;
			IEnumerable<Message> messages = _hipChatService.GetMessages(_room);
			Messages = messages.ToList();
		}
	}
}