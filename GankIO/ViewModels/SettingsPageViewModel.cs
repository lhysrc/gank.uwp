using GankIO.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Template10.Utils;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace GankIO.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await SettingsPartViewModel.ReloadCacheSize();
            

            await Task.CompletedTask;
        }
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                IsLoading = true;
            }
            
            _settings = Services.SettingsService.Instance;
            
        }

        bool _IsLoading = default(bool);
        public bool IsLoading { get { return _IsLoading; } set { Set(ref _IsLoading, value); } }


        //public bool ShowHamburgerButton
        //{
        //    get { return _settings.ShowHamburgerButton; }
        //    set
        //    {
        //        _settings.ShowHamburgerButton = value; base.RaisePropertyChanged();
        //        Views.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        public bool NeedToNofity
        {
            get { return _settings.TileSetting.NeedToNofity; }
            set
            {
                _settings.TileSetting.NeedToNofity = value;
                base.RaisePropertyChanged();                
            }
        }

        public bool ShowDayResult
        {
            get { return _settings.TileSetting.ShowDayResult; }
            set
            {
                _settings.TileSetting.ShowDayResult = value;
                base.RaisePropertyChanged();

                RegisterTileTask();
            }
        }

        private async void RegisterTileTask()
        {
            IsLoading = true;

            // 重新注册后台任务
            BackgroundTaskRegistration.AllTasks.ForEach(t =>
            {
                if (t.Value.Name == App.LIVETILETASK)
                {
                    t.Value.Unregister(true);
                }
            });
            await App.RegisterLiveTileTask();
            await TileAndToast.Show(true);

            IsLoading = false;
        }

        private long _CacheSize;
        public long CacheSize
        {
            get { return _CacheSize; }
            private set
            {
                Set(ref _CacheSize, value);
            }
        }

        //public bool IsFullScreen
        //{
        //    get { return _settings.IsFullScreen; }
        //    set
        //    {
        //        _settings.IsFullScreen = value;
        //        base.RaisePropertyChanged();
        //        if (value)
        //        {
        //            ShowHamburgerButton = false;
        //        }
        //        else
        //        {
        //            ShowHamburgerButton = true;
        //        }
        //        Views.Shell.HamburgerMenu.IsFullScreen = value;
        //    }
        //}

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set
            {
                _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark;
                base.RaisePropertyChanged();
                Views.Shell.HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            }
        }

        //private string _BusyText = "Please wait...";
        //public string BusyText
        //{
        //    get { return _BusyText; }
        //    set
        //    {
        //        Set(ref _BusyText, value);
        //        _ShowBusyCommand.RaiseCanExecuteChanged();
        //    }
        //}

        DelegateCommand _ClearCacheCommand;
        public DelegateCommand ClearCacheCommand
            => _ClearCacheCommand ?? (_ClearCacheCommand = new DelegateCommand(async () =>
            {
                IsLoading = true;

                await ApplicationData.Current.ClearAsync(ApplicationDataLocality.LocalCache);
                await ApplicationData.Current.ClearAsync(ApplicationDataLocality.Temporary);
                await ReloadCacheSize();

                IsLoading = false;
            }, () => CacheSize > 0));

        internal async Task<long> ReloadCacheSize()
        {
            IsLoading = true;
            //读取缓存大小
            await Task.Run(() =>
            {
                var cachePath = ApplicationData.Current.LocalCacheFolder.Path;
                var cacheSize = Utils.DirectorySize(new DirectoryInfo(cachePath));
                var tempPath = ApplicationData.Current.TemporaryFolder.Path;
                var tempSize = Utils.DirectorySize(new DirectoryInfo(tempPath));
                CacheSize = cacheSize + tempSize;
            });           

            ClearCacheCommand.RaiseCanExecuteChanged();

            IsLoading = false;
            return CacheSize;

        }
        //DelegateCommand _ShowBusyCommand;
        //public DelegateCommand ShowBusyCommand
        //    => _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () =>
        //    {
        //        Views.Busy.SetBusy(true, _BusyText);
        //        await Task.Delay(5000);
        //        Views.Busy.SetBusy(false);
        //    }, () => !string.IsNullOrEmpty(BusyText)));
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}
