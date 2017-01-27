using Hipstr.Client.Commands;
using Hipstr.Core.Messaging;
using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
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
		public ICommand ToggleEmoticonPaneCommand { get; }

		private bool _inputPaneVisible;

		public bool IsInputPaneVisible
		{
			get { return _inputPaneVisible; }
			private set
			{
				_inputPaneVisible = value;
				ShowEmoticonPane = ShowEmoticonPane; // Refresh ShowEmoticonPane in case IsInputPanelVisible changed
				OnPropertyChanged();
			}
		}

		private bool _showEmoticonPane;

		public bool ShowEmoticonPane
		{
			get { return _showEmoticonPane; }
			set
			{
				_showEmoticonPane = value && IsInputPaneVisible;
				OnPropertyChanged();
			}
		}

		public MessagesView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<MessagesViewModel>();
			_inputPaneVisible = false;
			_showEmoticonPane = false;
			ToggleEmoticonPaneCommand = new RelayCommand(() => ShowEmoticonPane = !ShowEmoticonPane);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			InputPane.GetForCurrentView().Showing += InputPane_OnShowing;
			InputPane.GetForCurrentView().Hiding += InputPane_OnHiding;

			ViewModel.MessageSource = (IMessageSource)e.Parameter;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			InputPane.GetForCurrentView().Showing -= InputPane_OnShowing;
			InputPane.GetForCurrentView().Hiding -= InputPane_OnHiding;
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