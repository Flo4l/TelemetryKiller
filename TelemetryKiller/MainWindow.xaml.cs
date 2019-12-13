using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace TelemetryKiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Label log;
        private Button button;
        private Button exitButton;
        
        public MainWindow() {
            InitializeComponent();
            log = (Label) this.FindName("Log");
            button = (Button) this.FindName("Button");
            exitButton = (Button) this.FindName("ExitButton");
            //ImageSource imageSource = new BitmapImage(new Uri("/icon.png"));
            //((Image) this.FindName("image")).Source = imageSource;
            if (!IsUserAdministrator()) {
                log.Foreground = Brushes.Crimson;
                log.Content = "Not run as Administrator";
                button.IsEnabled = false;
                return;
            }
            CheckWin10();
        }

        private bool IsUserAdministrator() {
            bool isAdmin;
            try {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex) {
                isAdmin = false;
            }
            catch (Exception ex) {
                isAdmin = false;
            }
            return isAdmin;
        }
        
        private void CheckWin10()
        {
            if (PatchManager.CheckWin10())
            {
                log.Content = "Windows 10 Detected";
            }
            else
            {
                log.Foreground = Brushes.Crimson;
                log.Content = "No Windows 10 Detected";
                button.IsEnabled = false;
            }
        }

        private void Exit(object sender, RoutedEventArgs e) {
            Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
            if (mouseButtonEventArgs.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void StartPatch(object sender, RoutedEventArgs e)
        {
            try {
                bool blockTelemetry = ((CheckBox) this.FindName("BlockTelemetry")).IsChecked.Value;
                bool disableWebsearch = ((CheckBox) this.FindName("DisableWebsearch")).IsChecked.Value;
                bool disableCortana = ((CheckBox) this.FindName("DisableCortana")).IsChecked.Value;
                bool disableSystemAds = ((CheckBox) this.FindName("DisableSystemAds")).IsChecked.Value;
                bool disableOneDrive = ((CheckBox) this.FindName("DisableOneDrive")).IsChecked.Value;
                bool disableWifiShare = ((CheckBox) this.FindName("DisableWifiShare")).IsChecked.Value;
                bool disableGeolocation = ((CheckBox) this.FindName("DisableGeolocation")).IsChecked.Value;
                bool disableAccountSync = ((CheckBox) this.FindName("DisableAccountSync")).IsChecked.Value;

                PatchManager manager = new PatchManager(log, disableCortana, disableWebsearch, blockTelemetry,
                    disableSystemAds, disableOneDrive, disableWifiShare, disableGeolocation, disableAccountSync);
                ExitButton.IsEnabled = false;
                manager.StartPatch();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally {
                ExitButton.IsEnabled = true;
            }
        }
    }
}