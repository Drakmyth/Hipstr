using Hipstr.Client.Commands;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel : ViewModelBase
	{
		private readonly IHipChatService _hipChatService;

		public ObservableCollection<Room> Rooms { get; set; }
		public ICommand NavigateToMessagesViewCommand { get; }

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>())
		{
		}

		public RoomsViewModel(IHipChatService hipChatService)
		{
			Rooms = new ObservableCollection<Room>();
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();

			_hipChatService = hipChatService;
		}

		public async Task UpdateRoomsAsync()
		{
			IEnumerable<Room> rooms = await _hipChatService.GetRoomsAsync();
			Rooms.Clear();
			Rooms.AddRange(rooms);
		}
	}
}