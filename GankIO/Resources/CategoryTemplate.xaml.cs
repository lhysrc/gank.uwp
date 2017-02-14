using GankIO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace GankIO.Resources
{
    public partial class CategoryTemplate
    {
        public CategoryTemplate()
        {

            InitializeComponent();

        }
    }



    public class DayResultsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AndroidTemplate { get; set; }
        public DataTemplate iOSTemplate { get; set; }
        public DataTemplate FuliTemplate { get; set; }
        public DataTemplate AllTemplate { get; set; }
        public DataTemplate NonImageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object o, DependencyObject container)
        {
            if(!(o is all)) return base.SelectTemplateCore(o, container);

            var item = (all)o;

            //if (item.type == nameof(Android))
            //    return AndroidTemplate;
            //else if (item.type == nameof(iOS))
            //    return iOSTemplate;
            //else 
            if (item.type == nameof(福利))
                return FuliTemplate;
            //else if (item.images == null)
            //    return NonImageTemplate;
            else
                return AllTemplate;

            
        }
    }
}
