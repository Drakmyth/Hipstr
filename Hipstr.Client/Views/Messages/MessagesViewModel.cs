using System;
using Hipstr.Client.Commands;
using Hipstr.Client.Services;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Messages
{
	[UsedImplicitly]
	public class MessagesViewModel : ViewModelBase
	{
		public ICommand ReloadRoomCommand { get; }
		public ObservableCollection<Message> Messages { get; }

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

		private bool _loadingMessages;

		public bool LoadingMessages
		{
			get { return _loadingMessages; }
			private set
			{
				_loadingMessages = value;
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
			_loadingMessages = false;

			ReloadRoomCommand = new RelayCommandAsync(ReloadMessagesAsync, () => !LoadingMessages, this, nameof(LoadingMessages));
		}

		public async Task ReloadMessagesAsync()
		{
			try
			{
				LoadingMessages = true;
				IEnumerable<Message> messages = await _hipChatService.GetMessagesAsync(_room);
				Messages.Clear();
				Messages.AddRange(messages);
			}
			finally
			{
				LoadingMessages = false;
			}
		}

		public async Task CheckForNewMessages()
		{
			try
			{
				LoadingMessages = true;
				IEnumerable<Message> messages = await _hipChatService.GetMessagesAsync(_room);

				DateTime latestDate = Messages.Last().Date;
				Messages.AddRange(messages.Where(message => message.Date > latestDate));
			}
			finally
			{
				LoadingMessages = false;
			}
		}
	}
}