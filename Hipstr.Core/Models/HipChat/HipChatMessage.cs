using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessage
	{
		public DateTime Date { get; set; }
		public HipChatUser From { get; set; }
		public string Id { get; set; }
		public HipChatUser[] Mentions { get; set; }
		public string Message { get; set; }
		public string Type { get; set; }
	}
}