using GankIO.Models;
using GankIO.Services;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace GankIO.Views
{
    public sealed partial class PostGankDialog : ContentDialog
    {      
        private string url, desc, who, type;
        
        public PostGankDialog()
        {
            this.InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
            who = SettingsService.Instance.Publisher;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {            
            PostingProgressBar.Visibility = Visibility.Visible;
            IsSecondaryButtonEnabled = false;

            var deferral = args.GetDeferral();
            SettingsService.Instance.Publisher = who;

            var res = await GankService.PostGank(url, desc, who, type
#if DEBUG
                , debug: true
#endif
                );

            PostingProgressBar.Visibility = Visibility.Collapsed;            
            if (res.error)
            {
                args.Cancel = true;
                MsgText.Text = res.msg;
            }
            else
            {                
                MsgText.Text = "提交成功";
                await Task.Delay(1500);
            }
            IsSecondaryButtonEnabled = true;
            deferral.Complete();

            

        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (TypeComboBox.Items.Count > 0) return;

            all.GetAllTypeNames().ForEach(TypeComboBox.Items.Add);
        }
    }
}
