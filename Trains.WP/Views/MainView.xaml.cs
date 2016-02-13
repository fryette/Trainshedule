﻿using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Trains.Core.ViewModels;
using Trains.Infrastructure;

namespace Trains.WP.Views
{
	public sealed partial class MainView
	{
		static int _lastPivotIndex;
		public MainView()
		{
			InitializeComponent();
			MainPivot.SelectedIndex = _lastPivotIndex;
			NavigationCacheMode = NavigationCacheMode.Enabled;
		}

		private void Pivot_OnPivotItemLoaded(Pivot sender, PivotItemEventArgs args)
		{
			_lastPivotIndex = MainPivot.SelectedIndex;
			if (args.Item == MainPivotItem)
			{
				SetAppBarVisibility(false, true);
			}
			else if (args.Item == LastPivot)
			{
				SetAppBarVisibility(true);
			}
			else
			{
				SetAppBarVisibility();
			}

			((MainViewModel)ViewModel).RaisePropertyChanged("LastUpdateTime");
		}

		private void SetAppBarVisibility(bool updateAppBar = false, bool swapAppBar = false)
		{
			UpdateAppBar.Visibility = updateAppBar ? Visibility.Visible : Visibility.Collapsed;
			SwapAppBar.Visibility = swapAppBar ? Visibility.Visible : Visibility.Collapsed;
		}

		private void AutoSuggestBoxSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			CoreApplication.GetCurrentView().CoreWindow.IsInputEnabled = false;
			CoreApplication.GetCurrentView().CoreWindow.IsInputEnabled = true;
			SetVisibility(Visibility.Visible);
			SetVisibilityAutossugestBox(Visibility.Visible, Visibility.Visible);
			ComboBoxSelectionChanged(null, null);
		}


		private void TrainListOnTapped(object sender, TappedRoutedEventArgs e)
		{
			CommandButton.Command?.Execute(TrainList.SelectedItem);
		}

		private void AutoSuggestBoxManipulationStarted(object sender, RoutedEventArgs e)
		{
			SetVisibility(Visibility.Collapsed);
			DataPicker.Visibility = Visibility.Collapsed;
			if (sender as AutoSuggestBox == From)
				SetVisibilityAutossugestBox(Visibility.Visible, Visibility.Collapsed);
			else
				SetVisibilityAutossugestBox(Visibility.Collapsed, Visibility.Visible);

		}

		private void AutoSuggestBoxManipulationCompleted(object sender, RoutedEventArgs e)
		{
			SetVisibility(Visibility.Visible);
			SetVisibilityAutossugestBox(Visibility.Visible, Visibility.Visible);
			ComboBoxSelectionChanged(null, null);
		}

		private void SetVisibility(Visibility visibility)
		{
			comboBox.Visibility = visibility;
			SearchButton.Visibility = visibility;
			Routes.Visibility = visibility;
			CommandBar.Visibility = visibility;
		}

		void SetVisibilityAutossugestBox(Visibility from, Visibility to)
		{
			From.Visibility = from;
			To.Visibility = to;
		}

		private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
		{
			var senderElement = sender as FrameworkElement;
			var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
			flyoutBase.ShowAt(senderElement);
		}

		private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataPicker.Visibility = comboBox.SelectedItem?.ToString() == Dependencies.LocalizationService.GetString("OnDay")
				? Visibility.Visible
				: Visibility.Collapsed;
		}
	}
}
