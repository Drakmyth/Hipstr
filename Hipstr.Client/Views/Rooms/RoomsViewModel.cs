using Hipstr.Client.Commands;
using Hipstr.Client.Services;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Comparers;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Rooms
{
	[UsedImplicitly]
	public class RoomsViewModel : ViewModelBase
	{
		public event EventHandler<RoomGroup> RoomGroupScrollToHeaderRequest;

		public ObservableCollection<RoomGroup> RoomGroups { get; }
		public ICommand NavigateToMessagesViewCommand { get; }
		public ICommand JumpToHeaderCommand { get; }
		public ICommand RefreshRoomsCommand { get; }
		public ICommand MarkFavoriteCommand { get; }
		public ICommand UnmarkFavoriteCommand { get; }

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

		private Room _tappedRoom;
		public Room TappedRoom
		{
			get { return _tappedRoom; }
			set
			{
				_tappedRoom = value;
				OnPropertyChanged();
			}
		}

		private readonly IHipChatService _hipChatService;
		private readonly IDataService _dataService;
		private readonly IFavoritesService _favoritesService;

		public RoomsViewModel(IHipChatService hipChatService, IDataService dataService, IMainPageService mainPageService, IFavoritesService favoritesService)
		{
			_hipChatService = hipChatService;
			_dataService = dataService;
			_favoritesService = favoritesService;

			LoadingRooms = false;
			mainPageService.Title = "Rooms";

			RoomGroups = new ObservableCollection<RoomGroup>();
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();
			JumpToHeaderCommand = new RelayCommandAsync(JumpToHeaderAsync);
			RefreshRoomsCommand = new RelayCommandAsync(RefreshRoomsAsync, () => !LoadingRooms, this, nameof(LoadingRooms));
			MarkFavoriteCommand = new RelayCommand<Room>(MarkFavoriteAsync, room => room != null);
			UnmarkFavoriteCommand = new RelayCommand<Room>(UnmarkFavoriteAsync, room => room != null);
		}

		// TODO: Commonize Refresh/Cache logic into base class or service
		// TODO: Commonize GroupBy/OrderBy logic into base class or service
		public async Task LoadRoomsAsync()
		{
			try
			{
				LoadingRooms = true;
				IEnumerable<RoomGroup> roomGroups = await _dataService.LoadRoomGroupsAsync();
				if (!roomGroups.Any())
				{
					roomGroups = await RebuildRoomGroupCache();
				}
				RoomGroups.Clear();
				RoomGroups.AddRange(roomGroups);
			}
			finally
			{
				LoadingRooms = false;
			}
		}

		private async Task RefreshRoomsAsync()
		{
			try
			{
				RoomGroups.Clear();
				LoadingRooms = true;
				IEnumerable<RoomGroup> roomGroups = await RebuildRoomGroupCache();
				RoomGroups.AddRange(roomGroups);
			}
			finally
			{
				LoadingRooms = false;
			}
		}

		private async Task<IEnumerable<RoomGroup>> RebuildRoomGroupCache()
		{
			IEnumerable<Room> rooms = await _hipChatService.GetRoomsAsync();
			IEnumerable<RoomGroup> roomGroups = OrderAndGroupRooms(rooms).ToList();
			await _dataService.SaveRoomGroupsAsync(roomGroups);

			return roomGroups;
		}

		private static IEnumerable<RoomGroup> OrderAndGroupRooms(IEnumerable<Room> rooms)
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

		private static string DetermineGroupHeader(string name)
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

		private async Task JumpToHeaderAsync()
		{
			var dialog = new ListGroupJumpDialog();
			ModalResult<string> headerText = await dialog.ShowAsync(RoomGroups.Select(rg => new JumpHeader(rg.Header, rg.Rooms.Any())));
			if (!headerText.Cancelled)
			{
				RoomGroupScrollToHeaderRequest?.Invoke(this, RoomGroups.Where(rg => rg.Header == headerText.Result).Single());
			}
		}

		private void MarkFavoriteAsync(Room room)
		{
			_favoritesService.MarkFavorite(room);
		}

		private void UnmarkFavoriteAsync(Room room)
		{
			_favoritesService.UnmarkFavorite(room);
		}
	}
}