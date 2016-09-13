using Hipstr.Client.Commands;
using Hipstr.Client.Views.Rooms;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hipstr.Client.Views.Messages
{
	public class MessagesViewModel : ViewModelBase//, ITitled
	{
		public ICommand ReloadRoomCommand { get; set; }
		public ICommand NavigateToRoomsViewCommand { get; set; }

		private readonly ObservableCollection<Message> _messages;
		public ObservableCollection<Message> Messages
		{
			get { return _messages; }
			set
			{
				OnPropertyChanging();
				_messages.Clear();
				foreach (Message message in value)
				{
					_messages.Add(message);
				}
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

			_messages = new ObservableCollection<Message>();
			ReloadRoomCommand = new RelayCommand(ReloadRoom);
			NavigateToRoomsViewCommand = new NavigateToViewCommand<RoomsView>();
		}

		private void OnRoomChanged()
		{
			Title = _room.Name;
			ReloadRoom();
		}

		private void ReloadRoom()
		{
			IEnumerable<Message> messages = _hipChatService.GetMessages(_room);
			Messages = new ObservableCollection<Message>(messages);
		}
	}
}