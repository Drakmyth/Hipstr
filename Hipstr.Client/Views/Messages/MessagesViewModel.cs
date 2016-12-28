using Hipstr.Client.Commands;
using Hipstr.Client.Views.Rooms;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Messages
{
	public class MessagesViewModel : ViewModelBase
	{
		public ICommand ReloadRoomCommand { get; set; }
		public ICommand NavigateToRoomsViewCommand { get; set; }
		public ObservableCollection<Message> Messages { get; set; }

		private Room _room;

		public Room Room
		{
			get { return _room; }
			set
			{
				_room = value;
				_mainPageService.Title = _room.Name;
				OnPropertyChanged();
			}
		}

		private readonly IHipChatService _hipChatService;
		private readonly IMainPageService _mainPageService;

		public MessagesViewModel(IHipChatService hipChatService, IMainPageService mainPageService)
		{
			_hipChatService = hipChatService;
			_mainPageService = mainPageService;

			Messages = new ObservableCollection<Message>();
			_mainPageService.Title = "Messages";

			ReloadRoomCommand = new RelayCommandAsync(ReloadMessagesAsync);
			NavigateToRoomsViewCommand = new NavigateToViewCommand<RoomsView>();
		}

		public async Task ReloadMessagesAsync()
		{
			IEnumerable<Message> messages = await _hipChatService.GetMessagesAsync(_room);
			Messages.Clear();
			Messages.AddRange(messages);
		}
	}
}