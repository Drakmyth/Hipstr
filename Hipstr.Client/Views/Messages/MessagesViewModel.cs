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
				OnPropertyChanged();
			}
		}

		private readonly IHipChatService _hipChatService;

		public MessagesViewModel() : this(IoCContainer.Resolve<IHipChatService>())
		{
		}

		public MessagesViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;

			Messages = new ObservableCollection<Message>();
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