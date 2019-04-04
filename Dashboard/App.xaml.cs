using Dashboard.DataBase;
using Dashboard.IO;
using Dashboard.UI.Pages;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Number of application process. Used to run only one instance
        /// </summary>
        private static readonly int programInstancesCount = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length;

        public MainWindow AppWindow { get; set; }
        public TaskbarIcon TaskbarIcon { get; set; }
        public MainPage MainPage { get; set; }

        public static Random GlobalRandom { get; set; } = new Random();

        public FileSystemWatcher FileWatcher { get; set; } = new FileSystemWatcher()
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size,
            IncludeSubdirectories = false
        };

        public Config AppConfiguration { get; set; }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        private void Taskbar_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TaskbarIcon.Dispose();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string messageText = "An unhandled exception occured in Com Port data manager application.\n\nException details:\n\n";

            messageText += $"Exception Message: {e.Exception.Message}\nTarget Site: {e.Exception.TargetSite.Name}\n\n";

            Exception inner = e.Exception.InnerException;

            for (int i = 1; i < 6 && inner != null; i++)
            {
                messageText += $"Inner Exception {i} Message: {e.Exception.Message}\nTarget Site: {e.Exception.TargetSite.Name}\n\n";
                inner = inner.InnerException;
            }
            messageText += "We suggest you to close the application and report data to developers.";
        }

        public static void TryClosingPort()
        {
            try
            {
                try
                {
                    PortManager.ReaderThread.Abort();
                }
                catch { }

                try
                {
                    PortManager.ParserThread.Abort();
                }
                catch { }

                try
                {
                    PortManager.ElementsSender.Abort();
                }
                catch { }

                try
                {
                    PortManager.ItemCreatorThread.Abort();
                }
                catch { }

                if (PortManager.GlobalPort != null)
                {
                    if (PortManager.GlobalPort.IsOpen)
                        PortManager.GlobalPort.Close();
                }
            }
            catch { }
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            TryClosingPort();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (programInstancesCount > 1)
            {
                MessageBox.Show("Currently an instance of application is running. Use other instances, or close other instance before using this instance."
                    , "Instance", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

                Shutdown();
            }

            TaskbarIcon = (TaskbarIcon)Current.FindResource("Application.Global.NotifyIcon");
            TaskbarIcon.TrayMouseDoubleClick += Taskbar_TrayMouseDoubleClick;

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fa-IR");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");

            Config.InitializeLocalFolder();
            Config.LoadSettingsFromFile();

            if (AppConfiguration.PackDetailsFileAddress.Trim() != string.Empty)
            {
                try
                {
                    FileWatcher.Changed += FileWatcher_Changed;
                    FileWatcher.Path = Path.GetDirectoryName(AppConfiguration.PackDetailsFileAddress);
                    FileWatcher.Filter = Path.GetFileName(AppConfiguration.PackDetailsFileAddress);
                    FileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                    FileWatcher.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something wrong with Watcher: " + ex.Message);
                }
            }
        }

        private async void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileWatcher.EnableRaisingEvents = false;

            await RecivePackDetailsDataAsync();

            FileWatcher.EnableRaisingEvents = true;
        }

        public async Task RecivePackDetailsDataAsync()
        {
            try
            {
                string content = "";

                await Task.Delay(500);

                using (var fs = new FileStream(AppConfiguration.PackDetailsFileAddress, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    content = sr.ReadToEnd();
                }

                //string content = File.ReadAllText(AppConfiguration.PackDetailsFileAddress);
                content = content.Replace("\r", string.Empty);
                content = content.Replace("\n", string.Empty);
                string[] splitedData = content.Split(',');

                if (splitedData.Length >= 5)
                {
                    string packNumber = splitedData[0];
                    string dateString = splitedData[1];
                    string timeString = splitedData[2];
                    string weight = splitedData[3];
                    string itemCode = splitedData[4];

                    DataBaseHelper.Entities.Packs.Add(new Packs()
                    {
                        ItemCode = itemCode,
                        Date = dateString,
                        PackNo = packNumber,
                        Time = timeString,
                        Weight = weight
                    });

                    DataBaseHelper.Entities.SaveChanges();

                    try
                    {
                        MainPage.Dispatcher.Invoke(() => MainPage.PackViewRefreshRequest());
                    }
                    catch { }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error occured with new pack: " + ex.Message); }
        }

        public static App GetApp() => (App)Current;

        [Conditional("DEBUG")]
        public static void WriteLog(Exception ex)
        {
            string exceptionDetail = ex.Message + "\n\n";
            string innerException = "No inner exeption";
            if (ex.InnerException != null)
                innerException = ex.InnerException.Message;

            string dirPath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Logs";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + $"Logs\\log{DateTime.Now.Millisecond}_{GlobalRandom.Next(1, 1000)}.txt";

            File.WriteAllText(filePath, exceptionDetail + innerException);
        }
    }
}
