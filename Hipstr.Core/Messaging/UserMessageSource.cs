﻿using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Messaging
{
	public class UserMessageSource : IMessageSource
	{
		public string Name => User.Name;

		private readonly IHipChatService _hipChatService;
		public User User { get; }

		public UserMessageSource(IHipChatService hipChatService, User user)
		{
			_hipChatService = hipChatService;
			User = user;
		}

		public void SendMessage()
		{
			throw new System.NotImplementedException();
		}

		public async Task<IReadOnlyList<Message>> GetMessagesAsync()
		{
			return await _hipChatService.GetMessagesForUserAsync(User);
		}
	}
}