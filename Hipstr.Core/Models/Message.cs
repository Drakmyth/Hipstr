using System;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Core.Models
{
	public class Message
	{
		public User PostedBy { get; private set; }
		public DateTime Date { get; private set; }
		public string Text { get; private set; }
		public IReadOnlyList<Message> Edits { get; private set; }
		public IReadOnlyList<MessageImage> Images { get; private set; }
		public IReadOnlyList<MessageLink> Links { get; private set; }
		public IReadOnlyList<MessageTwitterUser> TwitterUsers { get; private set; }
		public IReadOnlyList<MessageTwitterStatus> TwitterStatuses { get; private set; }
		public IReadOnlyList<MessageVideo> Videos { get; private set; }

		private Message(User postedBy, DateTime date, string text)
		{
			PostedBy = postedBy;
			Date = date;
			Text = text;
		}

		public static IMessageBuilder Builder(Message message)
		{
			return new MessageBuilder(message);
		}

		public static IMessageBuilder Builder(User postedBy, DateTime date, string text)
		{
			return new MessageBuilder(postedBy, date, text);
		}

		private class MessageBuilder : Message, IMessageBuilder
		{
			public MessageBuilder(Message message) : base(message.PostedBy, message.Date, message.Text)
			{
				WithEdits(message.Edits);
				WithImages(message.Images);
				WithLinks(message.Links);
				WithTwitterUsers(message.TwitterUsers);
				WithTwitterStatuses(message.TwitterStatuses);
				WithVideos(message.Videos);
			}

			public MessageBuilder(User postedBy, DateTime date, string text) : base(postedBy, date, text)
			{
				WithEdits(new Message[] {});
				WithImages(new MessageImage[] {});
				WithLinks(new MessageLink[] {});
				WithTwitterUsers(new MessageTwitterUser[] {});
				WithTwitterStatuses(new MessageTwitterStatus[] {});
				WithVideos(new MessageVideo[] {});
			}

			public IMessageBuilder WithPostedBy(User user)
			{
				PostedBy = user;
				return this;
			}

			public IMessageBuilder WithDate(DateTime date)
			{
				Date = date;
				return this;
			}

			public IMessageBuilder WithText(string text)
			{
				Text = text;
				return this;
			}

			public IMessageBuilder WithEdits(IEnumerable<Message> edits)
			{
				Edits = edits.ToList().AsReadOnly();
				return this;
			}

			public IMessageBuilder WithImages(IEnumerable<MessageImage> images)
			{
				Images = images.ToList().AsReadOnly();
				return this;
			}

			public IMessageBuilder WithLinks(IEnumerable<MessageLink> links)
			{
				Links = links.ToList().AsReadOnly();
				return this;
			}

			public IMessageBuilder WithTwitterUsers(IEnumerable<MessageTwitterUser> twitterUsers)
			{
				TwitterUsers = twitterUsers.ToList().AsReadOnly();
				return this;
			}

			public IMessageBuilder WithTwitterStatuses(IEnumerable<MessageTwitterStatus> twitterStatuses)
			{
				TwitterStatuses = twitterStatuses.ToList().AsReadOnly();
				return this;
			}

			public IMessageBuilder WithVideos(IEnumerable<MessageVideo> videos)
			{
				Videos = videos.ToList().AsReadOnly();
				return this;
			}

			public Message Build()
			{
				return new Message(PostedBy, Date, Text)
				{
					Edits = Edits,
					Images = Images,
					Links = Links,
					TwitterUsers = TwitterUsers,
					TwitterStatuses = TwitterStatuses,
					Videos = Videos
				};
			}
		}
	}
}