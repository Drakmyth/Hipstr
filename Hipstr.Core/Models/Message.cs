using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hipstr.Core.Models
{
	public class Message
	{
		public User PostedBy { get; }
		public DateTime Date { get; }
		public string Text { get; }
		public IReadOnlyList<Message> Edits { get; }

		public Message(User postedBy, DateTime date, string text) : this(postedBy, date, text, new Message[] {})
		{
		}

		public Message(User postedBy, DateTime date, string text, IEnumerable<Message> edits)
		{
			PostedBy = postedBy;
			Date = date;
			Text = text;
			Edits = new ReadOnlyCollection<Message>(edits.ToList());
		}
	}
}