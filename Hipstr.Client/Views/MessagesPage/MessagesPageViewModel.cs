using Hipstr.Core.Models;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.MessagesPage
{
	public class MessagesPageViewModel
	{
		public ObservableCollection<Message> Messages { get; set; }
	}
}