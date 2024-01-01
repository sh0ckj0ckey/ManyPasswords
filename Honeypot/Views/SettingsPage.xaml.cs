using System;
using System.Diagnostics;
using Honeypot.Core.Utils;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainViewModel _viewModel = null;
        private string _appVersion = string.Empty;

        public SettingsPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;
            _appVersion = $"v{AppVersionUtil.GetAppVersion()}";
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoToStoreRate(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }
            catch { }
        }

        /// <summary>
        /// 查看数据库目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickDbPath(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = UserDataPaths.GetDefault().Documents;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                var dbNoMewingFolder = await folder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var dbFolder = await dbNoMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
                await Launcher.LaunchFolderAsync(dbFolder);
            }
            catch { }
        }

        /// <summary>
        /// 访问 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Honeypot"));
            }
            catch { }
        }

        /// <summary>
        /// 访问朱雀 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoZhuque(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/TrionesType/zhuque"));
            }
            catch { }
        }

        /// <summary>
        /// 查看 Windows Hello 的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoWindowsHelloSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
            }
            catch { }
        }

        /// <summary>
        /// 切换 Windows Hello 锁定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToggleWindowHello(object sender, RoutedEventArgs e)
        {
            SetWindowsHelloEnable(WindowsHelloToggleSwitch.IsOn);
        }

        /// <summary>
        /// 开关WindowsHello
        /// </summary>
        /// <param name="on"></param>
        private async void SetWindowsHelloEnable(bool on)
        {
            try
            {
                if (MainViewModel.Instance.AppSettings.EnableLock != on)
                {
                    Debug.WriteLine("Verifying Windows Hello...");

                    switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                    {
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                            MainViewModel.Instance.AppSettings.EnableLock = on;
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            MainViewModel.Instance.ShowTipsContentDialog("无法验证身份", "当前识别设备未配置或被系统策略禁用");
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            MainViewModel.Instance.ShowTipsContentDialog("无法验证身份", "当前识别设备不可用");
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            MainViewModel.Instance.ShowTipsContentDialog("无法验证身份", "验证失败，请重试");
                            break;
                        default:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            break;
                    }
                }
            }
            catch { }
        }
    }
}
