using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.Helpers
{
    public static class AppInitializer
    {
        public static void InitializeDomain() => AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

        public static void InitialzeTaskbarIcon(RoutedEventHandler doubleClickHandler)
        {
            ((App)Application.Current).TaskbarIcon = (TaskbarIcon)Application.Current.FindResource("Application.Global.NotifyIcon");
            ((App)Application.Current).TaskbarIcon.TrayMouseDoubleClick += doubleClickHandler;
        }

        public static void InitializeCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fa-IR");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");
        }

        public static void InitializeConfig()
        {
            Config.InitializeLocalFolder();
            Config.LoadSettingsFromFile();
        }

        public static void InitializeFileWatcher(FileSystemEventHandler fileChangedHandler)
        {
            var FileWatcher = App.CurrentApp.FileWatcher;
            if (App.CurrentApp.AppConfiguration.PackDetailsFileAddress.Trim() != string.Empty)
            {
                try
                {
                    FileWatcher.Changed += fileChangedHandler;
                    FileWatcher.Path = Path.GetDirectoryName(App.CurrentApp.AppConfiguration.PackDetailsFileAddress);
                    FileWatcher.Filter = Path.GetFileName(App.CurrentApp.AppConfiguration.PackDetailsFileAddress);
                    FileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                    FileWatcher.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something wrong with Watcher: " + ex.Message);
                }
            }
        }

        public static void CheckNumberOfInstances(int programInstancesCount)
        {
            if (programInstancesCount > 1)
            {
                MessageBox.Show("Currently an instance of application is running. Use other instances, or close other instance before using this instance."
                    , "Instance", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

                App.CurrentApp.Shutdown();
            }
        }
    }
}
