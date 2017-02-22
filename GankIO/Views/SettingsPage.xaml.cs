using GankIO.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace GankIO.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;
        }

        private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            Debug.WriteLine($"ErrorCode:{e.ErrorCode}, ErrorMessage:{e.ErrorMessage}");
        }

        private void AdControl_AdRefreshed(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine($"AdRefreshed");
        }
    }

    class DirSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var size = (long)value;
            return Utils.ReadableSize(size);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}