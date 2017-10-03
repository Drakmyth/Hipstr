using Hipstr.Core.Messaging;
using JetBrains.Annotations;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Dialogs.JoinChatDialog
{
	[UsedImplicitly]
	public class JoinChatDialogViewModel : ViewModelBase
	{
		public ObservableCollection<MessageSourceGroup> MessageSources { get; }

		private IMessageSource _selectedMessageSource;

		public IMessageSource SelectedMessageSource
		{
			get => _selectedMessageSource;
			set
			{
				if (_selectedMessageSource == value) return;

				_selectedMessageSource = value;
				OnPropertyChanged();
			}
		}

		public JoinChatDialogViewModel()
		{
			MessageSources = new ObservableCollection<MessageSourceGroup>();
		}
	}
}