using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GankIO.UserControls
{
    public sealed partial class CategoryUserControl : UserControl
    {
        private Models.all Item { get { return DataContext as Models.all; } }
        public CategoryUserControl()
        {
            this.InitializeComponent();
            DataContextChanged += (s, e) => 
            {
                Bindings.Update();
                if (Item?.images != null) FindName(nameof(SmallImage));
                //if (Item == null) return;
                //IconBox.Children.Add(Resources["AndroidIcon"] as Path);
            };

            Loaded += (s, e) =>
            {
                var args = new VisualStateChangedEventArgs
                {
                    NewState = AdaptiveVisualStateGroup.CurrentState,
                };
                AdaptiveVisualStateGroup_CurrentStateChanged(this, args);
            };
        }


        public Uri PathIcon
        {
            get { return (Uri)GetValue(PathIconProperty); }
            set
            {
                SetValue(PathIconProperty, value);
            }
        }

        public static readonly DependencyProperty PathIconProperty =
            DependencyProperty.Register(nameof(PathIcon), typeof(Uri), typeof(CategoryUserControl), new PropertyMetadata(null));//, iconChanged));

        private static void iconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var uc = d as CategoryUserControl;
            //if (uc != null)
            //    uc.TypeIcon.UriSource = e.NewValue as Uri;
        }

        private void AdaptiveVisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (SmallImage == null)
            {
                SmallImageColumn.Width = new GridLength(0);
                return;
            }

            if (e.NewState == VisualStateNormal)
            {
                SmallImageColumn.Width = new GridLength(80);
            }
            else
            {
                SmallImageColumn.Width = GridLength.Auto;
            }
        }
    }
}
