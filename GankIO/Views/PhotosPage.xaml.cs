using GankIO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI;
using GankIO.Models;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Popups;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace GankIO.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PhotosPage : Page
    {
        public PhotosPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Loaded += PhotosPage_Loaded;
            Unloaded += PhotosPage_Unloaded;


        }
        private void PhotosPage_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PhotosPage_Loaded(object sender, RoutedEventArgs args)
        {
            //var scrollViewer = ItemsGridView.GetScrollViewer();
            //if (scrollViewer == null) return;
            //scrollViewer.LayoutUpdated += (s, _) =>
            //{
            //    Debug.WriteLine(scrollViewer.VerticalOffset);
            //    //scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset //到底
            //};

            //return;
            var scrollViewer = ItemsGridView.FindDescendant<ScrollViewer>();
            var scrollbars = scrollViewer.GetDescendantsOfType<ScrollBar>().ToList();
            var verticalBar = scrollbars.FirstOrDefault(x => x.Orientation == Orientation.Vertical);
            scrollViewer.ViewChanged += (_, e) =>
            {
                if (e.IsIntermediate) return;
                TopAppBarButton.Visibility =
                    scrollViewer.VerticalOffset == 0 ?
                    Visibility.Collapsed :
                    Visibility.Visible;
            };
            TopAppBarButton.Click += (s, e) =>
            {
                scrollViewer.ChangeView(null, 0, null, false);
            };
            RefreshAppBarButton.Click += AppBarButton_Click;
        }

        private void photosGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var grid = (sender as VariableSizedWrapGrid);
            var grid = (sender as ItemsWrapGrid);
            //if (grid.Width == double.NaN) return;




            if (AdaptiveVisualStateGroup.CurrentState == VisualStateNarrow)
            {
                grid.ItemWidth = e.NewSize.Width / 2;
            }

            else if (AdaptiveVisualStateGroup.CurrentState == VisualStateNormal)
            {
                grid.ItemWidth = e.NewSize.Width / 3;
            }

            else if (AdaptiveVisualStateGroup.CurrentState == VisualStateWide)
            {
                grid.ItemWidth = e.NewSize.Width / 4;
            }
            grid.ItemHeight = grid.ItemWidth * 3 / 4;
        }

        private void FlipView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pt = e.GetPosition(imageFlipView);

            if (pt.X < 50 || pt.Y < 50 ||
               imageFlipView.ActualWidth - pt.X < 50 ||
                imageFlipView.ActualHeight - pt.Y < 50) return;
            //fadeOutStoryboard.Begin();

            hideFlipView();
        }

        private async void hideFlipView()
        {
            await FlipGrid.Fade(0).StartAsync();
            FlipGrid.Visibility = Visibility.Collapsed;
        }

        private async void ItemsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //fadeInStoryboard.Begin();            
            FlipGrid.Visibility = Visibility.Visible;
            await FlipGrid.Fade(1).StartAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ////获取DataTemplate里的控件。
            //var item = imageFlipView.ContainerFromItem(imageFlipView.SelectedItem) as FlipViewItem;
            //var img = item.FindDescendant<Microsoft.Toolkit.Uwp.UI.Controls.ImageEx>();

            var btn = (AppBarButton)sender;
            if (((SymbolIcon)btn.Icon).Symbol != Symbol.Save) return;
            var srcIcon = btn.Icon;
            var srcBool = appBarToggleButton.IsChecked;
            appBarToggleButton.IsChecked = true;


            var f = (imageFlipView.SelectedItem as 福利);
            if (f == null) return;
            var img = await ImageCache.Instance.GetFromCacheAsync(new Uri(f.url), true);
            img.UriSource = img.UriSource ?? new Uri(f.url);

            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("干货福利", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync($"{f.createdAt:yyyyMMdd}.jpg", CreationCollisionOption.GenerateUniqueName);

            try
            {
                await img.SaveToFileAsync(file);                
                btn.Icon = new SymbolIcon(Symbol.Accept);                
                
                await Task.Delay(1500);

                btn.Icon = srcIcon;
                appBarToggleButton.IsChecked = srcBool;
            }
            catch (Exception ex)
            {
                new MessageDialog(ex.Message, "保存失败");
            }
            
        }

        
        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            var f = (imageFlipView.SelectedItem as 福利);
            if (f != null)
                await TileAndToast.UpdateTileByPhotos(new[] { f });
        }

        private void ImageEx_ImageExOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            //var img = (sender as Microsoft.Toolkit.Uwp.UI.Controls.ImageEx);
            //var grid = ((Grid)img.Parent);
            //var border = grid.Children.OfType<Border>()?.First();
            //if (border != null) border.Visibility = Visibility.Visible;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            hideFlipView();
        }

        private void ItemsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.OfType<福利>().FirstOrDefault();
            if(item!=null)
                ItemsGridView.ScrollIntoView(item);
        }



        //private async void scrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        //{
        //    var scrollViewer = sender as ScrollViewer;
        //    var doubleTapPoint = e.GetPosition(scrollViewer);
        //    if (scrollViewer.ZoomFactor != 1)
        //    {
        //        //scrollViewer.ZoomToFactor(1);
        //        scrollViewer.ChangeView(null,null, 1);
        //    }
        //    else if (scrollViewer.ZoomFactor == 1)
        //    {
        //        scrollViewer.ChangeView(doubleTapPoint.X, doubleTapPoint.Y,2);
        //        //var dispatcher = Window.Current.CoreWindow.Dispatcher;
        //        //await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
        //        //{
        //        //    scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
        //        //    scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
        //        //});
        //    }
        //}
    }
}
