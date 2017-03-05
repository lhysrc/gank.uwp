using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Template10.Common;
using Template10.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GankIO.Views
{
    public sealed partial class Message : UserControl
    {
        public Message()
        {
            InitializeComponent();
        }

        public string BusyText
        {
            get { return (string)GetValue(BusyTextProperty); }
            set { SetValue(BusyTextProperty, value); }
        }
        public static readonly DependencyProperty BusyTextProperty =
            DependencyProperty.Register(nameof(BusyText), typeof(string), typeof(Message), new PropertyMetadata("Please wait..."));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(Message), new PropertyMetadata(false));

        // hide and show busy dialog
        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var view = modal.ModalContent as Message;
                if (view == null)
                    modal.ModalContent = view = new Message();
                modal.IsModal = view.IsBusy = busy;
                view.BusyText = text;
            });
        }

        public static void Show(string text,double delay = 1)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var shell = modal.Content as Shell;
                
                shell.ShowMessage(text, delay);
            });
        }
    }
}
