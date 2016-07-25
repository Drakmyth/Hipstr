using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Messages
{
	public class MessagesViewModel : ViewModelBase, IRoomReloader//, ITitled
	{
		public ICommand ReloadRoomCommand { get; set; }

		private readonly List<Message> _messages;
		public IEnumerable<Message> Messages
		{
			get { return _messages; }
			set
			{
				OnPropertyChanging();
				_messages.Clear();
				_messages.AddRange(value);
				OnPropertyChanged();
			}
		}

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				OnPropertyChanging();
				_title = value;
				OnPropertyChanged();
			}
		}

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

			_messages = new List<Message>();
			ReloadRoomCommand = new ReloadRoomCommand(this);
		}

		private void OnRoomChanged()
		{
			Title = _room.Name;
			ReloadRoom();
		}

		public void ReloadRoom()
		{
			IEnumerable<Message> messages = _hipChatService.GetMessages(_room);
			Messages = messages;
		}
	}
}