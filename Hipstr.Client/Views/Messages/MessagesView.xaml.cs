using Hipstr.Core.Messaging;
using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Messages
{
	public sealed partial class MessagesView : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public MessagesViewModel ViewModel => (MessagesViewModel)DataContext;
		private bool _pollForMessages;

		private bool _inputPaneVisible;

		public bool IsInputPaneVisible
		{
			get { return _inputPaneVisible; }
			private set
			{
				_inputPaneVisible = value;
				OnPropertyChanged();
			}
		}

		public MessagesView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<MessagesViewModel>();
			_inputPaneVisible = false;
			_pollForMessages = true;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			InputPane.GetForCurrentView().Showing += InputPane_OnShowing;
			InputPane.GetForCurrentView().Hiding += InputPane_OnHiding;

			ViewModel.MessageSource = (IMessageSource)e.Parameter;
			await ViewModel.ReloadMessagesAsync();
			await ViewModel.LoadEmoticons();

			while (_pollForMessages)
			{
				await Task.Delay(TimeSpan.FromSeconds(30));
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await ViewModel.CheckForNewMessages(); });
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			InputPane.GetForCurrentView().Showing -= InputPane_OnShowing;
			InputPane.GetForCurrentView().Hiding -= InputPane_OnHiding;

			_pollForMessages = false;
		}

		private void InputPane_OnShowing(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			IsInputPaneVisible = true;
		}

		private void InputPane_OnHiding(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			IsInputPaneVisible = false;
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}