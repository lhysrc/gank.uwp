using GankIO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace GankIO.Resources
{
    class ReadableDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime)value;
            return date.ReadableDate();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }


    class SmallImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var url = value as string;
            if (string.IsNullOrWhiteSpace(url)) return null;

            url = $"{url}?imageView2/0/w/{parameter}";

            return new BitmapImage(new Uri(url));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    class IconPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = value as string;
            if (string.IsNullOrWhiteSpace(type)) return null;

            var filePath = $"Assets/Icons/{type.ToLower()}.png";
            if (!System.IO.File.Exists(filePath))
                filePath = $"Assets/Icons/default.png";

            return new Uri($"ms-appx:///{filePath}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
