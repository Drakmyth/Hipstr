﻿using Hipstr.Client.Commands;
using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
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
		public ICommand SelectEmoticonCommand { get; }
		public ObservableCollection<Message> Messages { get; }
		public ObservableCollection<Emoticon> Emoticons { get; }

		private IMessageSource _messageSource;

		public IMessageSource MessageSource
		{
			get { return _messageSource; }
			set
			{
				_messageSource = value;
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

		private bool _loadingEmoticons;

		public bool LoadingEmoticons
		{
			get { return _loadingEmoticons; }
			set
			{
				_loadingEmoticons = value;
				OnPropertyChanged();
			}
		}

		private bool _pollingForMessages;
		private readonly IHipChatService _hipChatService;

		public MessagesViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;

			Messages = new ObservableCollection<Message>();
			Emoticons = new ObservableCollection<Emoticon>();
			_loadingMessages = false;
			_sendingMessage = false;
			_messageDraft = string.Empty;
			_pollingForMessages = true;

			ReloadMessagesCommand = new RelayCommandAsync(ReloadMessagesAsync, () => !LoadingMessages, this, nameof(LoadingMessages));
			SendMessageCommand = new RelayCommandAsync(SendMessageAsync, () => !SendingMessage && !string.IsNullOrWhiteSpace(MessageDraft), this, nameof(SendingMessage), nameof(MessageDraft));
			SelectEmoticonCommand = new RelayCommand<Emoticon>(SelectEmoticon);
		}

		public override async Task InitializeAsync()
		{
			await ReloadMessagesAsync();
			await LoadEmoticonsAsync();

			while (_pollingForMessages)
			{
				await Task.Delay(TimeSpan.FromSeconds(30));
				await CheckForNewMessagesAsync();
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			_pollingForMessages = false;
		}

		private async Task ReloadMessagesAsync()
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

		private void SelectEmoticon(Emoticon emoticon)
		{
			MessageDraft += $"({emoticon.Shortcut})";
		}

		private async Task CheckForNewMessagesAsync()
		{
			IEnumerable<Message> messages = await _messageSource.GetMessagesAsync();

			if (Messages.Any())
			{
				DateTime latestDate = Messages.Last().Date;
				messages = messages.Where(message => message.Date > latestDate).ToList();
			}

			Messages.AddRange(messages);
		}

		private async Task LoadEmoticonsAsync()
		{
			try
			{
				LoadingEmoticons = true;
				IReadOnlyList<Emoticon> emoticons = await _hipChatService.GetEmoticonsForTeamAsync(MessageSource.Team);
				Emoticons.Clear();
				Emoticons.AddRange(emoticons);
			}
			finally
			{
				LoadingEmoticons = false;
			}
		}
	}
}