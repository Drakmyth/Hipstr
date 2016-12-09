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
	public class RoomsViewModel : ViewModelBase, ITitled, IFilterable
	{
		private readonly IHipChatService _hipChatService;

		public string Title => "Rooms";
		public ObservableCollection<Room> Rooms { get; set; }
		public ObservableCollection<FilterItem> Filters { get; set; }
		public ICommand NavigateToMessagesViewCommand { get; }

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>())
		{
		}

		public RoomsViewModel(IHipChatService hipChatService)
		{
			Rooms = new ObservableCollection<Room>();
			Filters = new ObservableCollection<FilterItem>();
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();

			_hipChatService = hipChatService;
		}

		public async Task UpdateRoomsAsync()
		{
			IEnumerable<Room> rooms = await _hipChatService.GetRoomsAsync();
			Rooms.Clear();
			Rooms.AddRange(rooms);
		}

		public void UpdateFilters()
		{
			IEnumerable<FilterItem> filters = Rooms.GroupBy(room => room.Team.Name).Select(group => new FilterItem {DisplayName = group.Key}).ToList();
			Filters.Clear();
			Filters.AddRange(filters);
		}
	}
}