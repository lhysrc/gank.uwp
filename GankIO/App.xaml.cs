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
using Microsoft.Services.Store.Engagement;

using Windows.Security.ExchangeActiveSyncProvisioning;

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


        #region 全局异常处理
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ShowExceptionDetailMessageDialog(e.Exception, "Unobserved Task Exception :(");
        }
        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowExceptionDetailMessageDialog(e.Exception, "Application Unhandled Exception :(");
        }
        private void SynchronizationContext_UnhandledException(object sender, Common.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowExceptionDetailMessageDialog(e.Exception, "Synchronization Context Unhandled Exception :(");
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
        
        ////简化异步异常堆栈信息
        //private string GetExceptionDetailMessage(Exception ex)
        //{
        //    return $"可将以下信息发送给开发者：\r\n{ex.Message}\r\n{ex.StackTraceEx()}";
        //}


        private async void ShowExceptionDetailMessageDialog(Exception ex, string title)
        {
            //简化异步异常堆栈信息
            var stackTrace = ex.StackTrace == null ? String.Empty : ex.StackTraceEx();
            var msg = $"{ex.Message}\r\n{stackTrace}";
            
            var dialog = new MessageDialog("可将以下信息发送给开发者：\r\n\r\n" + msg, title);
            
            dialog.Commands.Add(new UICommand("发送", async cmd =>
            {
                var appName = Windows.ApplicationModel.Package.Current.DisplayName;
                var body = GetEmailBody(title + msg);
                var uri = new Uri($"mailto:linhongyuan@outlook.com?subject=《{appName}》错误报告&body={body}", UriKind.Absolute);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }));
            dialog.Commands.Add(new UICommand("关闭"));

            await dialog.ShowAsync();
        }

        private static string GetEmailBody(string msg)
        {
            var deviceInfo = new EasClientDeviceInformation();

            string body = $"错误信息：{msg}  " +
                          $"（程序版本：{Utils.GetAppVersion()}, ";

            body += $"设备名：{deviceInfo.FriendlyName}, " +
                    $"操作系统：{deviceInfo.OperatingSystem} {Utils.GetOSVersion()}, " +
                    $"SKU：{deviceInfo.SystemSku}, " +
                    $"产品名称：{deviceInfo.SystemProductName}, " +
                    $"制造商：{deviceInfo.SystemManufacturer}, " +
                    $"固件版本：{deviceInfo.SystemFirmwareVersion}, " +
                    $"硬件版本：{deviceInfo.SystemHardwareVersion}）";

            body += "\r\n";
            return body;
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

            //throw new Exception("Test Exception.");

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

                //注册从开发人员中心接收通知
                var engagementManager = StoreServicesEngagementManager.GetDefault();
                await engagementManager.RegisterNotificationChannelAsync();
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
            if (args.Kind == ActivationKind.ToastNotification)
            {

                var toastActivationArgs = args as ToastNotificationActivatedEventArgs;

                var engagementManager = StoreServicesEngagementManager.GetDefault();
                string originalArgs = engagementManager.ParseArgumentsAndTrackAppLaunch(toastActivationArgs.Argument);

                // 使用通知传递来的参数进行下一步
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

            taskBuilder.SetTrigger(new TimeTrigger(30, false));
            taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
            taskBuilder.Register();

        }
        #endregion
    }
}
