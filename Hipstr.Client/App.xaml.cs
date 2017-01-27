﻿using Hipstr.Client.Views;
using Hipstr.Client.Views.MainPage;
using Hipstr.Client.Views.Teams;
using Hipstr.Core.Services;
using Hipstr.Core.Utility;
using System;
using System.Diagnostics;
using System.ServiceModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		public static Frame Frame { get; private set; }
		public static IAppSettings Settings { get; private set; }

		private readonly IToastService _toastService;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			IoCContainer.Build();

			InitializeComponent();
			Suspending += OnSuspending;
			UnhandledException += OnUnhandledException;

			_toastService = IoCContainer.Resolve<IToastService>();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (Debugger.IsAttached)
			{
				DebugSettings.EnableFrameRateCounter = true;
			}
#endif

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (Window.Current.Content == null)
			{
				Settings = IoCContainer.Resolve<IAppSettings>();

				// Create a Frame to act as the navigation context and navigate to the first page
				Frame = new Frame();

				Frame.NavigationFailed += OnNavigationFailed;
				Frame.Navigating += OnNavigating;
				Frame.Navigated += OnNavigated;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Create a MainPageView to hold the frame and place it in the current Window
				Window.Current.Content = new MainPageView(Frame);

				// Register a handler for BackRequested events and set the visibility of the Back button
				SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
				UpdateBackButtonVisibility();
			}

			if (Frame.Content == null)
			{
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				Frame.Navigate(typeof(TeamsView), e.Arguments);
			}
			// Ensure the current window is active
			Window.Current.Activate();

			// Make Status Bar match app aesthetic
			SetStatusBarColors();
		}

		private static void SetStatusBarColors()
		{
			if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) return;

			StatusBar statusBar = StatusBar.GetForCurrentView();
			statusBar.BackgroundColor = (Current.Resources["HipChatControl"] as SolidColorBrush)?.Color;
			statusBar.ForegroundColor = Colors.White;
			statusBar.BackgroundOpacity = 1;
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
		}

		private static void OnNavigating(object sender, NavigatingCancelEventArgs e)
		{
			var dataContext = (Frame.Content as Page)?.DataContext as IDisposable;
			dataContext?.Dispose();
		}

		private static async void OnNavigated(object sender, NavigationEventArgs e)
		{
			// Each time a navigation event occurs, update the Back button's visibility
			UpdateBackButtonVisibility();

			var dataContext = (Frame.Content as Page)?.DataContext as ViewModelBase;
			dataContext?.Initialize();
			// ReSharper disable once PossibleNullReferenceException
			await dataContext?.InitializeAsync();
		}

		private static void UpdateBackButtonVisibility()
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack ?
				AppViewBackButtonVisibility.Visible :
				AppViewBackButtonVisibility.Collapsed;
		}

		private static void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			if (!Frame.CanGoBack) return;

			e.Handled = true;
			Frame.GoBack();
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private static void OnSuspending(object sender, SuspendingEventArgs e)
		{
			SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			if (args.Exception is CommunicationException)
			{
				_toastService.ShowCommunicationErrorToast(args.Message);
				args.Handled = true;
				return;
			}

			// TODO: We should probably have a better way of handling "Something terribly has gone wrong"
			_toastService.ShowUnknownErrorToast(args.Message);
			args.Handled = true;
		}
	}
}