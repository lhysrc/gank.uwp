using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using Template10.Utils;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using GankIO.BackgroundTask;
using GankIO.Common;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Core;
using GankIO.Services;
using Windows.ApplicationModel.Core;

namespace GankIO
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            SplashFactory = (e) => new Views.Splash(e);
            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;

            #endregion            

            installCommand();
            
        }
        #region 安装小娜命令
        private async void installCommand()
        {
            var storageFile =
          await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
            new Uri("ms-appx:///Assets/CortanaCommands.xml"));
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager
                .InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }

        #endregion
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }


        #region 全局异常处理

        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Application Unhandled Exception:\r\n" + 
                GetExceptionDetailMessage(e.Exception), "爆了 :(").ShowAsync();
        }

        /// <summary>
        /// Should be called from OnActivated and OnLaunched
        /// </summary>
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            Common.ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private async void SynchronizationContext_UnhandledException(object sender, Common.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Synchronization Context Unhandled Exception:\r\n" + 
                GetExceptionDetailMessage(e.Exception), "爆了 :(").ShowAsync();
        }

        //简化异步异常堆栈信息
        private string GetExceptionDetailMessage(Exception ex)
        {
            return $"{ex.Message}\r\n{ex.StackTraceEx()}";
        }







        #endregion




        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        private void setTitleBar()
        {
            //var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = false;            
            //Window.Current.SetTitleBar(null);
            
            var view = ApplicationView.GetForCurrentView();

            if(view.TitleBar.ForegroundColor == Colors.White) return;
            // active
            //view.TitleBar.BackgroundColor = Colors.DarkBlue;
            view.TitleBar.ForegroundColor = Colors.White;

            // inactive
            view.TitleBar.InactiveBackgroundColor = Colors.LightGray;
            view.TitleBar.InactiveForegroundColor = Colors.Gray;


            // button
            //view.TitleBar.ButtonBackgroundColor = Colors.DodgerBlue;
            view.TitleBar.ButtonForegroundColor = Colors.White;

            //view.TitleBar.ButtonHoverBackgroundColor = Colors.LightSkyBlue;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;

            //view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 0, 0, 120);
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;

            view.TitleBar.ButtonInactiveBackgroundColor = Colors.LightGray;
            view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
        }
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        { 
            RegisterExceptionHandlingSynchronizationContext();
            //setTitleBar();


            //Window.Current.CoreWindow.Activated += (s, a) =>
            //{
            //    if (a.WindowActivationState != CoreWindowActivationState.Deactivated)
            //        setTitleBar();
            //    //Debug.WriteLine("Activated. Theme = {0}", Application.Current.RequestedTheme);
            //};



            if (startKind == StartKind.Launch)
            {
                await RegisterLiveTileTask();
            }


            if (args.Kind == ActivationKind.VoiceCommand)
            {
                //小娜激活
                var commandArgs = args as VoiceCommandActivatedEventArgs;

                var speechRecognitionResult = commandArgs.Result;
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;

                switch (voiceCommandName)
                {
                    case "fuli":
                        await NavigationService.NavigateAsync(typeof(Views.PhotosPage), true);
                        break;
                    case "category":
                        await NavigationService.NavigateAsync(typeof(Views.CategorysPage), true);
                        break;
                    default:return;
                }
                
                //var color = speechRecognitionResult.SemanticInterpretation.Properties["color"][0];

                return;
            }

            
            await NavigationService.NavigateAsync(typeof(Views.MainPage));
            //await NavigationService.NavigateAsync(typeof(Views.SettingsPage));
        }

        #region 后台任务

        public const string LIVETILETASK = nameof(LIVETILETASK);
        public static async Task RegisterLiveTileTask()
        {
            BackgroundExecutionManager.RemoveAccess();
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified ||
                status == BackgroundAccessStatus.Denied)
                //status == BackgroundAccessStatus.DeniedBySystemPolicy || //是否需要单独处理？
                //status == BackgroundAccessStatus.DeniedByUser)
            {
                return;
            }


            if (BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == LIVETILETASK))
            {
                //已注册
                return;
            }            

            var taskBuilder = new BackgroundTaskBuilder
            {
                Name = LIVETILETASK,
                TaskEntryPoint = typeof(LiveTileTask).FullName
            };
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

            //await TileAndToast.Show();

            taskBuilder.SetTrigger(new TimeTrigger(240, false));
            taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
            taskBuilder.Register();

        }
        #endregion
    }
}
