using BafghAutomation.Engine.Models;
using Dashboard.DataBase;
using Dashboard.Helpers;
using Dashboard.IO;
using Dashboard.UI.Pages;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
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

        public static App CurrentApp => (App)Current;

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

        private void Application_Exit(object sender, ExitEventArgs e) => TaskbarIcon.Dispose();

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

        public static void TryClosingPorts()
        {
            var threadsToAbort = new List<Thread>
            {
                PortManager.ReaderThread,
                PortManager.ParserThread,
                PortManager.ElementsSender,
                PortManager.ItemCreatorThread
            };
            threadsToAbort.ForEach(t => new Action(() => t.Abort()).Try());

            if (PortManager.GlobalPort?.IsOpen == true)
            {
                new Action(() => PortManager.GlobalPort.Close()).Try();
            }
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e) => TryClosingPorts();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppInitializer.CheckNumberOfInstances(programInstancesCount);

            AppInitializer.InitializeDomain();
            AppInitializer.InitialzeTaskbarIcon((s, ev) => Current.MainWindow.Show());
            AppInitializer.InitializeCulture();
            AppInitializer.InitializeConfig();
            AppInitializer.InitializeFileWatcher(async (s, ev) =>
            {
                FileWatcher.EnableRaisingEvents = false;
                await RecivePackDetailsDataAsync();
                FileWatcher.EnableRaisingEvents = true;
            });
        }

        public async Task RecivePackDetailsDataAsync()
        {
            try
            {
                string content = string.Empty;

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

                    DataBaseHelper.Entities.Packs.Add(new Pack(
                        id: 0,
                        itemCode: itemCode,
                        weight: weight,
                        time: timeString,
                        date: dateString,
                        packNo: packNumber));

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
