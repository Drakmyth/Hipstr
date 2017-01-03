using Hipstr.Client.Commands;
using Hipstr.Client.Services;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Comparers;
using Hipstr.Core.Models;
using Hipstr.Core.Models.Collections;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Rooms
{
	[UsedImplicitly]
	public class RoomsViewModel : ViewModelBase
	{
		public event EventHandler<ObservableGroupedCollection<Room>> RoomGroupScrollToHeaderRequest;

		public ObservableCollection<Room> Rooms { get; }
		public ObservableCollection<ObservableGroupedCollection<Room>> GroupedRooms { get; }
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
		private readonly ITeamService _teamService;
		private readonly IFavoritesService _favoritesService;

		public RoomsViewModel(IHipChatService hipChatService, ITeamService teamService, IMainPageService mainPageService, IFavoritesService favoritesService)
		{
			_hipChatService = hipChatService;
			_teamService = teamService;
			_favoritesService = favoritesService;

			LoadingRooms = false;
			mainPageService.Title = "Rooms";

			Rooms = new ObservableCollection<Room>();
			GroupedRooms = new ObservableCollection<ObservableGroupedCollection<Room>>();

			Rooms.CollectionChanged += RoomsOnCollectionChanged;

			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();
			JumpToHeaderCommand = new RelayCommandAsync(JumpToHeaderAsync);
			RefreshRoomsCommand = new RelayCommandAsync(() => RefreshRoomsAsync(), () => !LoadingRooms, this, nameof(LoadingRooms));
			MarkFavoriteCommand = new RelayCommand<Room>(MarkFavoriteAsync, room => room != null);
			UnmarkFavoriteCommand = new RelayCommand<Room>(UnmarkFavoriteAsync, room => room != null);
		}

		private void RoomsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			GroupedRooms.Clear();
			GroupedRooms.AddRange(OrderAndGroupRooms(Rooms));
		}

		public async Task RefreshRoomsAsync(HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.RefreshCache)
		{
			try
			{
				LoadingRooms = true;
				IEnumerable<Team> teams = await _teamService.GetTeamsAsync();
				Rooms.Clear();
				foreach (Team team in teams)
				{
					IEnumerable<Room> rooms = await _hipChatService.GetRoomsForTeamAsync(team, cacheBehavior);
					Rooms.AddRange(rooms);
				}
			}
			finally
			{
				LoadingRooms = false;
			}
		}

		// TODO: Commonize GroupBy/OrderBy logic into base class or service
		private static IEnumerable<ObservableGroupedCollection<Room>> OrderAndGroupRooms(IEnumerable<Room> rooms)
		{
			IList<ObservableGroupedCollection<Room>> roomGroups = rooms.OrderBy(room => room.Name, RoomNameComparer.Instance)
				.GroupBy(room => DetermineGroupHeader(room.Name), room => room,
					(group, groupedRooms) => new ObservableGroupedCollection<Room>(group, groupedRooms)).ToList();

			char[] groupNames = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ?".ToCharArray();
			for (var i = 0; i < groupNames.Length; i++)
			{
				string groupName = groupNames[i].ToString();
				if (!roomGroups.Any(rg => rg.Header.Equals(groupName)))
				{
					roomGroups.Insert(i, new ObservableGroupedCollection<Room>(groupName));
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
			ModalResult<string> headerText = await dialog.ShowAsync(GroupedRooms.Select(rg => new JumpHeader(rg.Header, rg.Any())));
			if (!headerText.Cancelled)
			{
				RoomGroupScrollToHeaderRequest?.Invoke(this, GroupedRooms.Where(rg => rg.Header == headerText.Result).Single());
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