using Hipstr.Client.Commands;
using Hipstr.Client.Views.Dialogs;
using Hipstr.Client.Views.Dialogs.ListGroupJumpDialog;
using Hipstr.Client.Views.Messages;
using Hipstr.Core.Comparers;
using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
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
using Hipstr.Client.Views.Dialogs.AddRoomDialog;

namespace Hipstr.Client.Views.Rooms
{
	[UsedImplicitly]
	public class RoomsViewModel : ViewModelBase
	{
		public event EventHandler<ObservableGroupedRoomsCollection> RoomGroupScrollToHeaderRequest;

		private readonly ObservableCollection<Room> _rooms;
		public ObservableCollection<ObservableGroupedRoomsCollection> GroupedRooms { get; }
		public ICommand NavigateToMessagesViewCommand { get; }
		public ICommand JumpToHeaderCommand { get; }
		public ICommand RefreshRoomsCommand { get; }
		public ICommand NewRoomCommand { get; }

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

		private IReadOnlyList<Team> _teams;

		private readonly IHipChatService _hipChatService;
		private readonly ITeamService _teamService;

		public RoomsViewModel(IHipChatService hipChatService, ITeamService teamService) : base("Rooms")
		{
			_hipChatService = hipChatService;
			_teamService = teamService;

			LoadingRooms = false;

			_rooms = new ObservableCollection<Room>();
			GroupedRooms = new ObservableCollection<ObservableGroupedRoomsCollection>();
			_teams = new List<Team>();

			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView, IMessageSource>(room => room != null);
			JumpToHeaderCommand = new RelayCommandAsync(JumpToHeaderAsync);
			RefreshRoomsCommand = new RelayCommandAsync(() => RefreshRoomsAsync(), () => !LoadingRooms, this, nameof(LoadingRooms));
			NewRoomCommand = new RelayCommandAsync(AddRoomAsync, () => !LoadingRooms, this, nameof(LoadingRooms));
		}

		public override void Initialize()
		{
			base.Initialize();

			_rooms.CollectionChanged += RoomsOnCollectionChanged;
			RefreshTitle();
		}

		public override async Task InitializeAsync()
		{
			if (!GroupedRooms.Any())
			{
				await RefreshRoomsAsync(HipChatCacheBehavior.LoadFromCache);
			}
		}

		private void RoomsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			GroupedRooms.Clear();
			GroupedRooms.AddRange(OrderAndGroupRooms(_rooms));
		}

		private async Task AddRoomAsync()
		{
			var dialog = new AddRoomDialogView();
			DialogResult<RoomCreationRequest> roomCreation = await dialog.ShowAsync(_teams);
			if (!roomCreation.Cancelled)
			{
				// TODO: Call HipChatService to create room, then navigate
			}
		}

		private async Task RefreshRoomsAsync(HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.RefreshCache)
		{
			try
			{
				LoadingRooms = true;
				_teams = await _teamService.GetTeamsAsync();
				_rooms.Clear();
				foreach (Team team in _teams)
				{
					IEnumerable<Room> rooms = await _hipChatService.GetRoomsForTeamAsync(team, cacheBehavior);

					// Because AddRange is an extension method that just calls Add for each element in the input collection,
					// it will cause the CollectionChanged event to be fired for each element in the input collection. This is
					// REALLY REALLY slow because we reorder and regroup the list whenever that event gets fired. Until the
					// built-in ObservableList supports AddRange properly (only throwing the event once), we can either subclass
					// ObservableList and do it ourselves, or just not OrderAndGroup until we add the last element. For now,
					// we'll do the latter. Note that the former is probably more correct, but subclassing native implementations
					// is ugh...
					// TODO: Stop modifying event subscriptions once we have a better way of handling this
					Room lastRoom = rooms.Last();
					_rooms.CollectionChanged -= RoomsOnCollectionChanged;
					_rooms.AddRange(rooms);
					_rooms.CollectionChanged += RoomsOnCollectionChanged;
					_rooms.Add(lastRoom);
				}
			}
			finally
			{
				LoadingRooms = false;
			}
		}

		// TODO: Commonize GroupBy/OrderBy logic into base class or service
		private static IEnumerable<ObservableGroupedRoomsCollection> OrderAndGroupRooms(IEnumerable<Room> rooms)
		{
			IList<ObservableGroupedRoomsCollection> roomGroups = rooms.OrderBy(room => room.Name, RoomNameComparer.Instance)
				.GroupBy(room => DetermineGroupHeader(room.Name), room => room,
					(group, groupedRooms) => new ObservableGroupedRoomsCollection(group, groupedRooms)).ToList();

			char[] groupNames = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ?".ToCharArray();
			for (var i = 0; i < groupNames.Length; i++)
			{
				string groupName = groupNames[i].ToString();
				if (!roomGroups.Any(rg => rg.Header.Equals(groupName)))
				{
					roomGroups.Insert(i, new ObservableGroupedRoomsCollection(groupName));
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
			var dialog = new ListGroupJumpDialogView();
			DialogResult<string> headerText = await dialog.ShowAsync(GroupedRooms.Select(rg => new JumpHeader(rg.Header, rg.Any())));
			if (!headerText.Cancelled)
			{
				RoomGroupScrollToHeaderRequest?.Invoke(this, GroupedRooms.Where(rg => rg.Header == headerText.Result).Single());
			}
		}
	}
}