using System;
using GankIO.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;
using GankIO.Models;

namespace GankIO.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        private void calendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            //if (args.AddedDates.Any())
            //    pageHeader.Text = args.AddedDates[0].ToString("yyyy-MM-dd");
            calendarFlyout.Hide();
        }

        //private void calendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        //{

        //    //if (HistoryDates == null && sender.Tag!=null) HistoryDates = sender.Tag as DateTime[];

        //    //if(HistoryDates.Contains(args.Item.Date.Date))
        //    //    args.Item.IsBlackout = true;
        //    //Debug.WriteLine(args.Item.Date.ToString());
        //    ////args.Item.IsBlackout = true;
        //}

        //DateTime[] HistoryDates=null;        

        //private void calendarView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    HistoryDates = (sender as FrameworkElement).Tag as DateTime[];
        //}



        #region master detail
        private all _lastSelectedItem;
        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
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
            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            if (MainWebView != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(MainWebView, !isNarrow);
            }
        }



        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (all)e.ClickedItem;
            _lastSelectedItem = clickedItem;

            if (AdaptiveVisualStateGroup.CurrentState == VisualStateNarrow)
            {
                ViewModel.NavigationService.Navigate(typeof(WebViewPage), _lastSelectedItem.url, new DrillInNavigationTransitionInfo());
                // changeWidthStoryboard.Begin();


            }
            else
            {

                MainWebView.WebViewUri = new Uri(_lastSelectedItem.url);
            }
            
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            MasterListView.SelectedItem = _lastSelectedItem;
        }

        //private void EnableContentTransitions()
        //{
        //    DetailContentPresenter.ContentTransitions.Clear();
        //    DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
        //}

        //private void DisableContentTransitions()
        //{
        //    if (DetailContentPresenter != null)
        //    {
        //        DetailContentPresenter.ContentTransitions.Clear();
        //    }
        //}
        #endregion

        private async void Add2GankButton_Click(object sender, RoutedEventArgs e)
        {
            await new PostGankDialog().ShowAsync();
        }
    }
}