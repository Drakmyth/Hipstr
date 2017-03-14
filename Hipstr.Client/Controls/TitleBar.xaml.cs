using Hipstr.Client.Commands;
using Hipstr.Client.Utility;
using Hipstr.Client.Views.MainPage;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Controls
{
	public sealed partial class TitleBar : UserControl
	{
		private static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TitleBar), new PropertyMetadata("Hipstr"));
		private static readonly DependencyProperty FiltersSourceProperty = DependencyProperty.Register("FiltersSource", typeof(object), typeof(TitleBar), new PropertyMetadata(null));
		private static readonly DependencyProperty SelectedFilterProperty = DependencyProperty.Register("SelectedFilter", typeof(object), typeof(TitleBar), new PropertyMetadata(null));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public object FiltersSource
		{
			get { return GetValue(FiltersSourceProperty); }
			set { SetValue(FiltersSourceProperty, value); }
		}

		public object SelectedFilter
		{
			get { return GetValue(SelectedFilterProperty); }
			set { SetValue(SelectedFilterProperty, value); }
		}

		private ICommand ToggleMenuPaneCommand { get; }

		public TitleBar()
		{
			InitializeComponent();

			ToggleMenuPaneCommand = new RelayCommand(() =>
			{
				bool isMenuOpen = this.GetFirstAncestorOfType<MainPageView>().ViewModel.MenuIsOpen;
				this.GetFirstAncestorOfType<MainPageView>().ViewModel.MenuIsOpen = !isMenuOpen;
			});
		}
	}
}