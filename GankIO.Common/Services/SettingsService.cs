using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace GankIO.Services
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
            TileSetting = new TileSetting(_helper);
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                //Views.Shell.HamburgerMenu.RefreshStyles(value, true);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        /// <summary>
        /// 提交干货的提交人
        /// </summary>
        public string Publisher
        {
            get { return _helper.Read(nameof(Publisher), ""); }
            set
            {
                _helper.Write(nameof(Publisher), value);
            }
        }

        //public bool ShowHamburgerButton
        //{
        //    get { return _helper.Read<bool>(nameof(ShowHamburgerButton), true); }
        //    set
        //    {
        //        _helper.Write(nameof(ShowHamburgerButton), value);
        //        //Views.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}
        //public bool IsFullScreen
        //{
        //    get { return _helper.Read<bool>(nameof(IsFullScreen), false); }
        //    set
        //    {
        //        _helper.Write(nameof(IsFullScreen), value);
        //        //Views.Shell.HamburgerMenu.IsFullScreen = value;
        //    }
        //}

        //public bool ShowDayResult
        //{
        //    get { return _helper.Read<bool>(nameof(ShowDayResult), true); }
        //    set
        //    {
        //        _helper.Write(nameof(ShowDayResult), value);
        //    }
        //}


        ///// <summary>
        ///// 是否通知
        ///// </summary>
        //public bool IsNofity
        //{
        //    get { return _helper.Read<bool>(nameof(IsNofity), true); }
        //    set
        //    {
        //        _helper.Write(nameof(IsNofity), value);
        //    }
        //}

        //public DateTime LastNofityDate
        //{
        //    get { return _helper.Read(nameof(LastNofityDate), DateTime.MinValue); }
        //    set
        //    {
        //        _helper.Write(nameof(LastNofityDate), value);
        //    }
        //}

        public TileSetting TileSetting { get; private set; } 
        //{
        //    get { return _helper.Read(nameof(TileSetting),); }
        //    set
        //    {
        //        _helper.Write(nameof(TileSetting), value);
        //    }
        //}
    }

    public class TileSetting
    {
        Template10.Services.SettingsService.ISettingsHelper _helper;
        internal TileSetting(Template10.Services.SettingsService.ISettingsHelper helper)
        {
            _helper = helper;
        }


        /// <summary>
        /// 显示每日更新
        /// </summary>
        public bool ShowDayResult
        {
            get { return _helper.Read<bool>(nameof(ShowDayResult), true); }
            set
            {
                _helper.Write(nameof(ShowDayResult), value);
            }
        }


        /// <summary>
        /// 是否通知
        /// </summary>
        public bool NeedToNofity
        {
            get { return _helper.Read<bool>(nameof(NeedToNofity), true); }
            set
            {
                _helper.Write(nameof(NeedToNofity), value);
            }
        }

        public DateTime LastDayResultTime
        {
            get { return _helper.Read(nameof(LastDayResultTime), DateTime.MinValue); }
            set
            {
                _helper.Write(nameof(LastDayResultTime), value);
            }
        }

        /// <summary>
        /// 最后更新日期
        /// </summary>
        public DateTime LastUpdateTime
        {
            get { return _helper.Read(nameof(LastUpdateTime), DateTime.MinValue); }
            set
            {
                _helper.Write(nameof(LastUpdateTime), value);
            }
        }

    }
}
