using Hipstr.Client.Commands;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Comparers;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel : ViewModelBase
	{
		public ObservableCollection<RoomGroup> RoomGroups { get; }
		public ICommand NavigateToMessagesViewCommand { get; }

		private readonly IHipChatService _hipChatService;

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

			IEnumerable<RoomGroup> roomGroups = rooms.OrderBy(room => room.Name, RoomNameComparer.Instance)
				.GroupBy(room => DetermineGroupHeader(room.Name), room => room,
					(group, groupedRooms) => new RoomGroup(group, groupedRooms));

			RoomGroups.Clear();
			RoomGroups.AddRange(roomGroups);
		}

		private string DetermineGroupHeader(string name)
		{
			string nameFirstChar = name[0].ToString();

			var alphaRegex = new Regex("^[A-Za-z]$");
			if (alphaRegex.IsMatch(nameFirstChar))
			{
				return nameFirstChar.ToUpper();
			}

			var numRegex = new Regex("^[0-9]$");
			return numRegex.IsMatch(nameFirstChar) ? "#" : "?";
		}
	}
}