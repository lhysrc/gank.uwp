using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Template10.Services.SerializationService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace GankIO.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WebViewPage : Page
    {
        public WebViewPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            MainWebView.WebClosed += (s, e) =>
            {                
                if (Frame.CanGoBack)
                    Frame.GoBack();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            var url = SerializationService
                        .Json.Deserialize<string>(e.Parameter?.ToString());

            //MainWebView.Navigate(new Uri(url));
            if (MainWebView.WebViewUri.ToString() != url)
                MainWebView.WebViewUri = new Uri(url);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if(e.OldState == VisualStateNarrow &&
               e.NewState != VisualStateNarrow &&
               Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
