using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace GankIO.Models
{

    public class HistoryDates
    {
        public bool error { get; set; }
        public DateTime[] results { get; set; }
    }
    public class PostResult
    {
        public bool error { get; set; }
        public string msg { get; set; }
    }

    public class FuliResult
    {
        public bool error { get; set; }
        public 福利[] results { get; set; }
    }
    public class CategoryRetuls<T> where T : all
    {
        public bool error { get; set; }
        public T[] results { get; set; }
    }


    //public class VariableSizedStyleSelector : StyleSelector
    //{
    //    public Style NormalStyle
    //    {
    //        get; set;
    //    }
    //    public Style DoubleWidthStyle { get; set; }

    //    protected override Style SelectStyleCore(object item, DependencyObject container)
    //    {
    //        if (this.NormalStyle == null || this.DoubleWidthStyle == null)
    //            return base.SelectStyleCore(item, container);

    //        if (item is 福利)
    //        {
    //            if (((福利)item).publishedAt.Date == DateTime.Now.Date)

    //                return DoubleWidthStyle;
    //            else
    //                return NormalStyle;

    //        }


    //        return base.SelectStyleCore(item, container);
    //    }
    //}
    //public class DateToSpanConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, string language)
    //    {
    //        if(value is DateTime && ((DateTime)value).Date == DateTime.Now.Date)
    //        {
    //            return 2;
    //        }
    //        return 1;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, string language)
    //    {
    //        return null;
    //    }
    //}
}
