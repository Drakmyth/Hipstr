using Hipstr.Client.Commands.ViewCommands;
using Hipstr.Client.Views.Teams;
using Hipstr.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Hipstr.Client.Views.MainPage
{
	public class MainPageViewModel : ViewModelBase
	{
		public ICommand NavigateToTeamsViewCommand { get; }
		public ICommand NavigateToRoomsViewCommand { get; }
		public ICommand NavigateToUsersViewCommand { get; }

		private FrameworkElement _frameContent;
		public FrameworkElement FrameContent
		{
			get
			{
				return _frameContent;
			}
			set
			{
				OnPropertyChanging();
				_frameContent = value;
				OnPropertyChanged();
			}
		}

		private string _title;
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				OnPropertyChanging();
				_title = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<FilterItem> _filters;
		public ObservableCollection<FilterItem> Filters
		{
			get
			{
				return _filters;
			}
			set
			{
				OnPropertyChanging();
				OnPropertyChanging(nameof(ShowFilters));
				_filters = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ShowFilters));
			}
		}
		public bool ShowFilters => _filters.Any();

		public MainPageViewModel()
		{
			NavigateToTeamsViewCommand = new NavigateToTeamsViewCommand();
			NavigateToRoomsViewCommand = new NavigateToRoomsViewCommand();
			NavigateToUsersViewCommand = new NavigateToUsersViewCommand();
			_filters = new ObservableCollection<FilterItem>();
			PropertyChanging += OnFrameContentChanging;
			PropertyChanged += OnFrameContentChanged;
		}

		private void OnContentTitleChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(ITitled.Title)) return;

			ITitled titled = FrameContent?.DataContext as ITitled;
			if (titled != null)
			{
				Title = titled.Title;
			}
		}

		private void OnFiltersChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(IFilterable.Filters)) return;

			IFilterable filterable = FrameContent?.DataContext as IFilterable;
			if (filterable != null)
			{
				Filters = new ObservableCollection<FilterItem>(filterable.Filters);
			}
		}

		private void OnFrameContentChanging(object sender, PropertyChangingEventArgs e)
		{
			if (e.PropertyName != nameof(FrameContent)) return;

			ViewModelBase vm = FrameContent?.DataContext as ViewModelBase;
			if (vm != null)
			{
				vm.PropertyChanged -= OnContentTitleChanged;
				vm.PropertyChanged -= OnFiltersChanged;
			}
		}

		private void OnFrameContentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(FrameContent)) return;

			// TODO: Consolidate with OnContentTitleChanged
			ITitled titled = FrameContent?.DataContext as ITitled;
			if (titled != null)
			{
				Title = titled.Title;
			}
			else
			{
				Title = "";
			}

			// TODO: Consolidate with OnFiltersChanged
			IFilterable filterable = FrameContent?.DataContext as IFilterable;
			if (filterable != null)
			{
				Filters = new ObservableCollection<FilterItem>(filterable.Filters);
			}
			else
			{
				Filters = new ObservableCollection<FilterItem>();
			}

			INotifyPropertyChanged inpc = FrameContent?.DataContext as INotifyPropertyChanged;
			if (inpc != null)
			{
				inpc.PropertyChanged += OnContentTitleChanged;
				inpc.PropertyChanged += OnFiltersChanged;
			}
		}
	}
}