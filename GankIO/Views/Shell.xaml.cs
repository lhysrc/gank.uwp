using System.ComponentModel;
using System.Linq;
using System;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Mvvm;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace GankIO.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        Services.SettingsService _settings;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = Services.SettingsService.Instance;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            //HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            //HamburgerMenu.HamburgerButtonVisibility = _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }

        //private async void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    var hb = (HyperlinkButton)sender;
        //    hb.nav
        //    await Windows.System.Launcher.LaunchUriAsync(new Uri("http://gank.io"));
        //}

        public async void ShowMessage(string msg, double seconds)
        {
            MessageBlock.Text = msg;
            MessageBorder.Visibility = Visibility.Visible;
            await MessageBorder.Fade(1).StartAsync();
            await MessageBorder.Fade(delay: seconds * 1e3).StartAsync();
            MessageBorder.Visibility = Visibility.Collapsed;
        }
    }
}
