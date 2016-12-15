using Hipstr.Client.Commands;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel : ViewModelBase
	{
		private readonly IHipChatService _hipChatService;

		public ObservableCollection<RoomGroup> RoomGroups { get; }
		public ICommand NavigateToMessagesViewCommand { get; }

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>())
		{
		}

		public RoomsViewModel(IHipChatService hipChatService)
		{
			RoomGroups = new ObservableCollection<RoomGroup>();
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();

			_hipChatService = hipChatService;
		}

		public async Task UpdateRoomsAsync()
		{
			IEnumerable<Room> rooms = await _hipChatService.GetRoomsAsync();

			IEnumerable<RoomGroup> roomGroups = rooms.OrderBy(room => room.Name)
				.GroupBy(room => room.Name[0].ToString(), room => room,
					(group, groupedRooms) => new RoomGroup(group, groupedRooms));

			RoomGroups.Clear();
			RoomGroups.AddRange(roomGroups);
		}
	}
}