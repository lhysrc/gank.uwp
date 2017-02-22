using GankIO.Views;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GankIO.UserControls
{
    public sealed partial class WebUserControl : UserControl
    {
        static readonly Uri defaultUri = new Uri("ms-appx-web:///Assets/default.html");
        public WebUserControl()
        {
            this.InitializeComponent();
            MainWebView.Navigate(defaultUri);

            Loaded += (s, e) =>
            {
                MainCommandBar.IsOpen = false;
            };
        }
        public event Action<WebUserControl,EventArgs> WebClosed;
        public Uri WebViewUri
        {
            get { return (Uri)GetValue(WebViewUriProperty); }
            set { SetValue(WebViewUriProperty, value); }
        }
        
        public static readonly DependencyProperty WebViewUriProperty =
            DependencyProperty.Register(nameof(WebViewUri), typeof(Uri), typeof(WebUserControl), new PropertyMetadata(defaultUri,onUriChanged));

        private static void onUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webuc = (WebUserControl)d;
            //webuc.MainWebView.NavigateToString("");
            if (e.NewValue.Equals(e.OldValue)) return;

            var url = (Uri)e.NewValue;
            //webuc.BackButton.IsEnabled = url != defaultUri;
            webuc.MainWebView.Navigate(url);
        }

        public async Task NavigateToDefaultPageAsync()
        {
            WebViewUri = defaultUri;
            //MainWebView.Navigate(defaultUri);
            
            await WebView.ClearTemporaryWebDataAsync();
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            MyProgressRing.Visibility = Visibility.Visible;
        }

        private void WebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWebView.CanGoBack)
            {
                MainWebView.GoBack();
            }
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToDefaultPageAsync();

            WebClosed?.Invoke(this, EventArgs.Empty);
        }

        private async void OpenInBrowerButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebViewUri == defaultUri) return;
            await Windows.System.Launcher.LaunchUriAsync(MainWebView.Source);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebViewUri == defaultUri) return;
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(MainWebView.Source.ToString());
            Clipboard.SetContent(dataPackage);
        }
    }
}
