using GankIO.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using GankIO.Models;
using System;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;

namespace GankIO.Views
{
    public sealed partial class CategorysPage : Page
    {
        private all _lastSelectedItem;

        public CategorysPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            //MainPivot.Items.Remove(AllPivotItem);
            MainWebView.WebClosed += (s, e) =>
            {
                _lastSelectedItem = null;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            if(_lastSelectedItem!=null &&
               AdaptiveVisualStateGroup.CurrentState != VisualStateNarrow)
            {
                MainWebView.WebViewUri = new Uri(_lastSelectedItem.url);
            }
        }

        private void LayoutRoot_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var item = MainPivot.SelectedItem as PivotItem;
            if (item == null) return;

            var list = item.FindDescendant<ListView>();
            if (list != null)
                list.SelectedItem = _lastSelectedItem;
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (all)e.ClickedItem;
            _lastSelectedItem = clickedItem;

            if (AdaptiveVisualStateGroup.CurrentState == VisualStateNarrow)
            {
                ViewModel.NavigationService.Navigate(typeof(WebViewPage), _lastSelectedItem.url, new DrillInNavigationTransitionInfo());

            }
            else
            {
                MainWebView.WebViewUri = new Uri(_lastSelectedItem.url);
            }
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, Windows.UI.Xaml.VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }
        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == VisualStateNarrow;

            if (isNarrow && oldState != VisualStateNarrow && _lastSelectedItem != null)
            {
                // Resize down to the detail item. Don't play a transition.
                //Frame.Navigate(typeof(WebViewPage), _lastSelectedItem.url, new SuppressNavigationTransitionInfo());
                ViewModel.NavigationService.Navigate(typeof(WebViewPage), _lastSelectedItem.url, new SuppressNavigationTransitionInfo());
            }
            EntranceNavigationTransitionInfo.SetIsTargetElement(MainPivot, isNarrow);
            if (MainWebView != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(MainWebView, !isNarrow);
            }
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var item = (PivotItem)e.AddedItems[0];
                var panel = item.Header as StackPanel;
                panel.FindDescendant<BitmapIcon>().Visibility = Visibility.Visible;
            }
            if (e.RemovedItems.Count > 0)
            {
                var item = (PivotItem)e.RemovedItems[0];
                var panel = item.Header as StackPanel;
                panel.FindDescendant<BitmapIcon>().Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainPivot.SelectedItem as PivotItem)
                ?.DataContext as IRefresh)
                ?.Refresh();
        }

        //private object tmpItem = null;
        //private void AppBarButton_Click(object sender, RoutedEventArgs e)
        //{            
        //    if (MainPivot.Items.Contains(AllPivotItem))//AllPivotItem.Visibility == Visibility.Visible)
        //    {
        //        //AllPivotItem.Visibility = Visibility.Collapsed;
        //        MainPivot.SelectedItem = tmpItem;
        //        MainPivot.IsLocked = false;
        //        MainPivot.Items.Remove(AllPivotItem);
        //    }
        //    else
        //    {
        //        //AllPivotItem.Visibility = Visibility.Visible;                
        //        MainPivot.Items.Add(AllPivotItem);
        //        tmpItem = MainPivot.SelectedItem;
        //        MainPivot.SelectedItem = AllPivotItem;
        //        MainPivot.IsLocked = true;
        //    }
        //}
    }
}
