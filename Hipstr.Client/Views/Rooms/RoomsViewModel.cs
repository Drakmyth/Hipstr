using Hipstr.Client.Commands;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Comparers;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System;
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
		public event EventHandler<RoomGroup> RoomGroupScrollToHeaderRequest;

		public ObservableCollection<RoomGroup> RoomGroups { get; }
		public ICommand NavigateToMessagesViewCommand { get; }
		public ICommand JumpToHeaderCommand { get; }
		public ICommand RefreshRoomsCommand { get; }

		private bool _loadingRooms;

		public bool LoadingRooms
		{
			get { return _loadingRooms; }
			private set
			{
				_loadingRooms = value;
				OnPropertyChanged();
			}
		}

		private readonly IHipChatService _hipChatService;
		private readonly IDataService _dataService;

		public RoomsViewModel(IHipChatService hipChatService, IDataService dataService, IMainPageService mainPageService)
		{
			_hipChatService = hipChatService;
			_dataService = dataService;

			LoadingRooms = false;
			mainPageService.Title = "Rooms";

			RoomGroups = new ObservableCollection<RoomGroup>();
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();
			JumpToHeaderCommand = new RelayCommandAsync(OnJumpToHeaderCommandAsync);
			RefreshRoomsCommand = new RelayCommandAsync(RefreshRoomsAsync, () => !LoadingRooms, this, nameof(LoadingRooms));
		}

		// TODO: Commonize Refresh/Cache logic into base class or service
		// TODO: Commonize GroupBy/OrderBy logic into base class or service
		public async Task LoadRoomsAsync()
		{
			LoadingRooms = true;
			IEnumerable<RoomGroup> roomGroups = await _dataService.LoadRoomGroupsAsync();
			if (!roomGroups.Any())
			{
				roomGroups = await RebuildRoomGroupCache();
			}
			LoadingRooms = false;

			RoomGroups.Clear();
			RoomGroups.AddRange(roomGroups);
		}

		private async Task RefreshRoomsAsync()
		{
			RoomGroups.Clear();
			LoadingRooms = true;
			IEnumerable<RoomGroup> roomGroups = await RebuildRoomGroupCache();
			LoadingRooms = false;
			RoomGroups.AddRange(roomGroups);
		}

		private async Task<IEnumerable<RoomGroup>> RebuildRoomGroupCache()
		{
			IEnumerable<Room> rooms = await _hipChatService.GetRoomsAsync();
			IEnumerable<RoomGroup> roomGroups = OrderAndGroupRooms(rooms).ToList();
			await _dataService.SaveRoomGroupsAsync(roomGroups);

			return roomGroups;
		}

		private IEnumerable<RoomGroup> OrderAndGroupRooms(IEnumerable<Room> rooms)
		{
			IList<RoomGroup> roomGroups = rooms.OrderBy(room => room.Name, RoomNameComparer.Instance)
				.GroupBy(room => DetermineGroupHeader(room.Name), room => room,
					(group, groupedRooms) => new RoomGroup(group, groupedRooms)).ToList();

			char[] groupNames = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ?".ToCharArray();
			for (var i = 0; i < groupNames.Length; i++)
			{
				string groupName = groupNames[i].ToString();
				if (!roomGroups.Any(rg => rg.Header.Equals(groupName)))
				{
					roomGroups.Insert(i, new RoomGroup(groupName));
				}
			}

			return roomGroups;
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

		private async Task OnJumpToHeaderCommandAsync()
		{
			var dialog = new ListGroupJumpDialog();
			ModalResult<string> headerText = await dialog.ShowAsync(RoomGroups.Select(rg => new JumpHeader(rg.Header, rg.Rooms.Any())));
			if (!headerText.Cancelled)
			{
				RoomGroupScrollToHeaderRequest?.Invoke(this, RoomGroups.Where(rg => rg.Header == headerText.Result).Single());
			}
		}
	}
}