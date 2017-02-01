using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models
{
	public interface IMessageBuilder
	{
		Message Build();
		IMessageBuilder WithDate(DateTime date);
		IMessageBuilder WithEdits(IEnumerable<Message> edits);
		IMessageBuilder WithImages(IEnumerable<MessageImage> images);
		IMessageBuilder WithLinks(IEnumerable<MessageLink> links);
		IMessageBuilder WithPostedBy(User user);
		IMessageBuilder WithText(string text);
		IMessageBuilder WithTwitterStatuses(IEnumerable<MessageTwitterStatus> twitterStatuses);
		IMessageBuilder WithTwitterUsers(IEnumerable<MessageTwitterUser> twitterUsers);
		IMessageBuilder WithVideos(IEnumerable<MessageVideo> videos);
		IMessageBuilder WithFile(MessageFile file);
	}
}