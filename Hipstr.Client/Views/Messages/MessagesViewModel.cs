using Hipstr.Client.Commands;
using Hipstr.Client.Services;
using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System;
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
		public ICommand ReloadMessagesCommand { get; }
		public ICommand SendMessageCommand { get; }
		public ObservableCollection<Message> Messages { get; }

		private IMessageSource _messageSource;

		public IMessageSource MessageSource
		{
			get { return _messageSource; }
			set
			{
				_messageSource = value;
				_mainPageService.Title = _messageSource.Name;
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

		private bool _sendingMessage;

		public bool SendingMessage
		{
			get { return _sendingMessage; }
			private set
			{
				_sendingMessage = value;
				OnPropertyChanged();
			}
		}

		private string _messageDraft;

		public string MessageDraft
		{
			get { return _messageDraft; }
			set
			{
				_messageDraft = value;
				OnPropertyChanged();
			}
		}

		private readonly IMainPageService _mainPageService;

		public MessagesViewModel(IMainPageService mainPageService)
		{
			_mainPageService = mainPageService;

			Messages = new ObservableCollection<Message>();
			_mainPageService.Title = "Messages";
			_loadingMessages = false;

			ReloadMessagesCommand = new RelayCommandAsync(ReloadMessagesAsync, () => !LoadingMessages, this, nameof(LoadingMessages));
			SendMessageCommand = new RelayCommandAsync(SendMessageAsync, () => !SendingMessage, this, nameof(SendingMessage));
		}

		public async Task ReloadMessagesAsync()
		{
			try
			{
				LoadingMessages = true;
				IEnumerable<Message> messages = await _messageSource.GetMessagesAsync();
				Messages.Clear();
				Messages.AddRange(messages);
			}
			finally
			{
				LoadingMessages = false;
			}
		}

		private async Task SendMessageAsync()
		{
			try
			{
				SendingMessage = true;
				await _messageSource.SendMessageAsync(MessageDraft);
				MessageDraft = string.Empty;
			}
			finally
			{
				SendingMessage = false;
			}
		}

		public async Task CheckForNewMessages()
		{
			try
			{
				LoadingMessages = true;
				IEnumerable<Message> messages = await _messageSource.GetMessagesAsync();

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