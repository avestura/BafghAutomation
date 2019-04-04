using System;
using System.Collections.Generic;
using System.Linq;
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
using Dashboard.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections.ObjectModel;
using Dashboard.UI.Controls;
using System.Net;
using static Dashboard.UI.Controls.DataInfoPresenterEnum;
using Dashboard.DataBase;
using System.Globalization;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Runtime.InteropServices.ComTypes;

namespace Dashboard.UI.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>

    public partial class MainPage : Page
    {
        public MainPage()
        {
            InfoPresenters = new ObservableCollection<DataInfoPresenter>();
            SentInfoPresenters = new ObservableCollection<DataInfoPresenter>();
            PackPresenters = new ObservableCollection<PackView>();

            App.CurrentApp.MainPage = this;

            InitializeComponent();
        }

        private readonly PersianCalendar Calendar = new PersianCalendar();

        public StringBuilder RecivedData { get; } = new StringBuilder();
        public string LastAdded { get; set; } = "";

        public string CurrentWeight { get; set; } = "0";

        private const int tooHighThersold = 10000;

        public bool IsWeightTooHigh
        {
            get
            {
                bool result = int.TryParse(CurrentWeight, out int weightValue);
                bool valueIsTooHigh = false;
                if (result && weightValue > tooHighThersold)
                    valueIsTooHigh = true;

                return valueIsTooHigh;
            }
        }

        public bool CheckIfWeightSpanValid(string weight)
        {
            //bool result = int.TryParse(weight, out int weightValue);
            //bool valueIsTooHigh = false;
            //if (result && weightValue > tooHighThersold)
            //    valueIsTooHigh = true;

            //return valueIsTooHigh;

            return weight.Length > 2;
        }

        private WebClient GeneralWebClient { get; } = new WebClient();

        public ObservableCollection<DataInfoPresenter> InfoPresenters
        {
            get { return (ObservableCollection<DataInfoPresenter>)GetValue(InfoPresentersProperty); }
            set { SetValue(InfoPresentersProperty, value); }
        }

        public static readonly DependencyProperty InfoPresentersProperty =
            DependencyProperty.Register("InfoPresenters", typeof(ObservableCollection<DataInfoPresenter>), typeof(MainPage), null);

        public ObservableCollection<DataInfoPresenter> SentInfoPresenters
        {
            get { return (ObservableCollection<DataInfoPresenter>)GetValue(SentInfoPresentersProperty); }
            set { SetValue(SentInfoPresentersProperty, value); }
        }

        public static readonly DependencyProperty SentInfoPresentersProperty =
            DependencyProperty.Register("SentInfoPresenters", typeof(ObservableCollection<DataInfoPresenter>), typeof(MainPage), null);

        public ObservableCollection<PackView> PackPresenters
        {
            get { return (ObservableCollection<PackView>)GetValue(PackPresentersProperty); }
            set { SetValue(PackPresentersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PackPresenters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PackPresentersProperty =
            DependencyProperty.Register("PackPresenters", typeof(ObservableCollection<PackView>), typeof(MainPage), null);

        private static bool FirstTimeActions = true;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FirstTimeActions)
            {
                FirstTimeActions = false;

                PortManager.GlobalPort.DataReceived += GlobalPort_DataReceived;

                PortManager.ReaderThread = new Thread(CheckData) { Priority = ThreadPriority.BelowNormal };
                PortManager.ParserThread = new Thread(TryParsingData) { Priority = ThreadPriority.Highest };
                PortManager.ElementsSender = new Thread(TrySendingDataToServer) { Priority = ThreadPriority.Normal };
                PortManager.ItemCreatorThread = new Thread(ItemCreator) { Priority = ThreadPriority.Normal };

                string[] portNames = SerialPort.GetPortNames();

                string comPortMonitor = App.CurrentApp.AppConfiguration.COMPortName;
                int baudRate = App.CurrentApp.AppConfiguration.BaudRate;
                var parity = App.CurrentApp.AppConfiguration.ParityType;
                int databits = App.CurrentApp.AppConfiguration.DataBits;
                var stopbits = App.CurrentApp.AppConfiguration.StopBits;

                if (portNames.Contains(comPortMonitor))
                {
                    //red color : #FFFF5858
                    //green color: #FF04B163

                    try
                    {
                        PortManager.GlobalPort = new SerialPort(comPortMonitor, baudRate, parity, databits, stopbits);
                    } catch (Exception ex)
                    {
                        MessageBox.Show($"Something is wrong with serial port: {ex.Message}");
                        return;
                    }

                    MonitoringPortNameBlock.Text = $"{comPortMonitor} is available";
                    MonitoringPortNameBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF04B163"));
                    StatusBlock.Text = ( PortManager.GlobalPort.IsOpen ) ? "Open" : "Not Open";
                    try
                    {
                        if (!PortManager.GlobalPort.IsOpen)
                            PortManager.GlobalPort.Open();
                        StatusBlock.Text = ( PortManager.GlobalPort.IsOpen ) ? "Open" : "Not Open";

                        PortManager.ReaderThread.Start();
                        PortManager.ParserThread.Start();
                        PortManager.ElementsSender.Start();
                        PortManager.ItemCreatorThread.Start();
                    } catch (Exception ex)
                    {
                        MessageBox.Show($"Error occured:\n{ex.Message}\n\nApplication will shutdown.\nClose any other applications that are using this port and try again.");
                        if (MessageBox.Show("Shutdown application?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            Application.Current.Shutdown();
                    }
                } else
                {
                    MonitoringPortNameBlock.Text = $"{comPortMonitor} is not available";
                    MonitoringPortNameBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF5858"));
                }

                SentInfoDatePicker.SelectedDate = DateTime.Now;
                PackDatePicker.SelectedDate = DateTime.Now;

                DataBaseHelper.Entities.Goods.LoadAsync();

                ItemsCodeDataGrid.ItemsSource = DataBaseHelper.Entities.Goods.Local;

                PackPresenters_Update();
            }
        }

        private void CheckData()
        {
            try
            {
                while (true)
                {
                    lock (RecivedData)
                    {
                        RecivedData.Append(PortManager.GlobalPort.ReadExisting().Replace(Environment.NewLine, ""));
                    }

                    Thread.Sleep(100);
                }
            } catch { }
        }

        /// <summary>
        /// Trims junk from <see cref="RecivedData"/>
        /// </summary>
        /// <returns>Reruns if junks cleaned</returns>
        private bool CleanStartFromJunks()
        {
            int parsablePosition = RecivedData.ToString().IndexOf("P+");
            if (parsablePosition > 0)
            {
                lock (RecivedData)
                {
                    RecivedData.Remove(0, parsablePosition);
                }
            }

            return parsablePosition >= 0;
        }

        private void TryParsingData()
        {
            try
            {
                while (true)
                {
                    while (!CleanStartFromJunks())
                    {
                        Thread.Sleep(10);
                    }

                    if (RecivedData.Length >= 8)
                    {
                        // Get First 8 digits
                        string stringValue = RecivedData
                            .ToString()
                            .Substring(0, 8)
                            .Replace("P+", "")
                            .TrimStart();

                        var success = uint.TryParse(stringValue, out uint number);

                        // Trim start recived data
                        lock (RecivedData)
                        {
                            RecivedData.Remove(0, 8);
                        }

                        lock (CurrentWeight)
                        {
                            CurrentWeight = (success) ? number.ToString() : "0";
                        }

                        // Update UI
                        CurrentWeightBlock.Dispatcher.Invoke(() => CurrentWeightBlock.Text = $"{CurrentWeight} kg");
                    }

                    Thread.Sleep(50);
                }
            } catch { }
        }

        private async void TrySendingDataToServer()
        {
            const int retryDelay = 2000;

            int failedSending = 0;
            int nullFirstUnsend = 0;

            bool showErr = false;
            while (true)
            {
                try
                {
                    DataInfoPresenter firstUnsend = null;

                    Dispatcher.Invoke(() => firstUnsend = InfoPresenters.GetFirstUnsend());

                    if (firstUnsend != null)
                    {
                        string response = "";

                        firstUnsend.Dispatcher.Invoke(() =>
                        {
                            firstUnsend.Waiting = true;
                            firstUnsend.DataInfoStatus = PresenterStatus.FileSending;
                        });

                        await firstUnsend.Dispatcher.Invoke(async () => response = await firstUnsend.SendDataToServer());

                        try
                        {
                            if (response == "1")
                            {
                                firstUnsend.Dispatcher.Invoke(() =>
                                {
                                    firstUnsend.Waiting = false;
                                    firstUnsend.Sent = true;
                                    firstUnsend.DataInfoStatus = PresenterStatus.FileSent;
                                    firstUnsend.AddSentItemToDatabase();

                                    // Why commented? because data fills with database
                                    if (firstUnsend.NowYear == Calendar.GetYear(SentInfoDatePicker.DisplayDate)
                                        && firstUnsend.NowMonth == Calendar.GetMonth(SentInfoDatePicker.DisplayDate)
                                        && firstUnsend.NowDay == Calendar.GetDayOfMonth(SentInfoDatePicker.DisplayDate))
                                    {
                                        SentInfoPresenters.Insert(0, firstUnsend.CreatePureCopy());
                                        SentInfoItemsCountBlock.Text = SentInfoPresenters.Count + " items";
                                        try
                                        {
                                            InfoPresenters_Update();
                                        }
                                        catch { }
                                    }
                                });

                                Task.Factory.StartNew(async () =>
                                {
                                    await Task.Delay(10000);

                                    await Dispatcher.Invoke(async () =>
                                    {
                                        await firstUnsend.HideUsingLinearAnimationAsync();
                                        InfoPresenters.Remove(firstUnsend);
                                        try
                                        {
                                            InfoPresenters_Update();
                                        }
                                        catch { }
                                    });
                                }).Forget();
                            }
                            else
                            {
                                firstUnsend.Dispatcher.Invoke(() =>
                                {
                                    firstUnsend.Waiting = false;
                                    firstUnsend.DataInfoStatus = PresenterStatus.ErrorSending;
                                });
                                failedSending++;
                                Thread.Sleep(retryDelay);
                            }
                        }
                        catch (Exception ex)
                        {
                            firstUnsend.Dispatcher.Invoke(() =>
                            {
                                firstUnsend.Waiting = false;
                                firstUnsend.DataInfoStatus = PresenterStatus.ErrorSending;
                            });
                            if (showErr)
                            {
                                MessageBox.Show(ex.Message);
                                showErr = false;
                            }

                            failedSending++;
                            Thread.Sleep(retryDelay);
                        }
                    }
                    else
                    {
                        nullFirstUnsend++;
                    }
                } catch (Exception ex)
                {
                    if (showErr)
                    {
                        MessageBox.Show(ex.Message);
                        showErr = false;
                    }
                }

                if (failedSending == 5 || nullFirstUnsend == 50)
                {
                    failedSending = 0;
                    nullFirstUnsend = 0;

                    Thread.Sleep(15000);
                }
                Thread.Sleep(100);
            }
        }

        private void ItemCreator()
        {
            while (true)
            {
                uint waitTime = App.CurrentApp.AppConfiguration.StabilityTime;
                if (waitTime > 30000)
                    waitTime = 30000;
                if (waitTime < 500)
                    waitTime = 500;

                uint delayTime = App.CurrentApp.AppConfiguration.RepetitiveDelayTime;
                if (delayTime > 50000)
                    delayTime = 50000;
                if (delayTime < 500)
                    delayTime = 500;

                // Weight consistency check duration in milliseconds
                const int WeightConsistencyCheckDuration = 50;

                string validWeight = "0";
                while (true)
                {
                    string firstWeight = CurrentWeight;
                    validWeight = "0";
                    bool weightAccepted = true;

                    for (int i = 0; i < (int)waitTime; i += WeightConsistencyCheckDuration)
                    {
                        Thread.Sleep(WeightConsistencyCheckDuration);

                        string secondWeight = CurrentWeight;

                        if (firstWeight != secondWeight)
                        {
                            weightAccepted = false;
                            break;
                        }
                    }

                    if (weightAccepted)
                    {
                        validWeight = firstWeight;
                        break;
                    }
                }

                if (validWeight != "0" && CheckIfWeightSpanValid(validWeight))
                {
                    LastAdded = validWeight;
                    WriteLastWeight(validWeight);

                    Dispatcher.Invoke(() =>
                    {
                        var newInfoPresenter = new DataInfoPresenter()
                        {
                            Content = validWeight + " kg",
                            Height = 30,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(2, 3, 2, 3),
                            Waiting = false,
                            DataInfoStatus = DataInfoPresenterEnum.PresenterStatus.FileSending
                        };

                        InfoPresenters.Add(newInfoPresenter);
                    });

                    Thread.Sleep((int)delayTime);
                }
            }
        }

        public void WriteLastWeight(string weight)
        {
            string path = App.CurrentApp.AppConfiguration.LastWeightFileAddress;
            if (path.Trim() != "")
            {
                try
                {
                    File.WriteAllText(path, weight);
                } catch { }
            }
        }

        private void GlobalPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
        }

        private void CurrentWeightBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var random = new Random();

            //for (int i = 0; i < 100; i++)
            //{

            //    PresenterStatus status = new PresenterStatus();

            //    switch (random.Next(4))
            //    {
            //        case 0:
            //            status = PresenterStatus.ErrorSending; break;
            //        case 1:
            //            status = PresenterStatus.FileSending; break;
            //        case 2:
            //            status = PresenterStatus.FileSent; break;
            //        case 3:
            //            status = PresenterStatus.Info; break;
            //        default:
            //            break;
            //    }

            //    var d = new DataInfoPresenter()
            //    {
            //        Content = random.Next(800, 5000) + " kg",
            //        Height = 30,
            //        VerticalContentAlignment = VerticalAlignment.Center,
            //        Margin = new Thickness(2, 3, 2, 3),
            //        Waiting = random.Next(2) == 1,
            //        DataInfoStatus = status
            //    };

            //    InfoPresenters.Add(d);
            //}
        }

        private void SentInfoDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SentInfoPresenters = SentInfoDatePicker.GetSentItemsForThisDate();
                SentInfoItemsCountBlock.Text = SentInfoPresenters.Count + " items";
            } catch (Exception ex)
            {
                App.WriteLog(ex);
            }
        }

        private void InfoPresenters_Update()
        {
            if (InfoPresenters.Count > 0)
                InfoPresenterNoItemWarn.Visibility = Visibility.Collapsed;
            else
                InfoPresenterNoItemWarn.Visibility = Visibility.Visible;
        }

        private void ItemsCodeDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                DataBaseHelper.Entities.SaveChanges();
            } catch /*(DbEntityValidationException ex)*/
            {
                //foreach (var entityValidationErrors in ex.EntityValidationErrors)
                //{
                //    foreach (var validationError in entityValidationErrors.ValidationErrors)
                //    {
                //        MessageBox.Show("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                //    }
                //}
            }
        }

        private void ItemsCodeDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                DataBaseHelper.Entities.SaveChanges();
            } catch /*(DbEntityValidationException ex)*/
            {
                //foreach (var entityValidationErrors in ex.EntityValidationErrors)
                //{
                //    foreach (var validationError in entityValidationErrors.ValidationErrors)
                //    {
                //        MessageBox.Show("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                //    }
                //}
            }
        }

        private void ItemsCodeSaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBaseHelper.Entities.SaveChanges();
            } catch /*(DbEntityValidationException ex)*/
            {
                //foreach (var entityValidationErrors in ex.EntityValidationErrors)
                //{
                //    foreach (var validationError in entityValidationErrors.ValidationErrors)
                //    {
                //        MessageBox.Show("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                //    }
                //}
            }
        }

        private void AddNewItemCodeButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentApp.AppWindow.MainFrame.Navigate(new AddItemCodePage());
        }

        #region Pack View Refersh
        private void PackDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PackViewRefreshRequest();
        }

        private void PackViewRefresh_Click(object sender, RoutedEventArgs e)
        {
            PackViewRefreshRequest();
        }

        public void PackViewRefreshRequest()
        {
            PackPresenters = PackDatePicker.GetPackItemsForThisDate();
            PackItemsCountBlock.Text = PackPresenters.Count + " items";
            PackPresenters_Update();
        }

        private void PackPresenters_Update()
        {
                if (PackPresenters.Count > 0)
                    PackPrintNoItemWarn.Visibility = Visibility.Collapsed;
                else
                    PackPrintNoItemWarn.Visibility = Visibility.Visible;
        }

        #endregion Pack View Refresh

        private async void PackDetailsManualAdd_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show(
                $"Do you want to manual fetch data from \"{App.CurrentApp.AppConfiguration.PackDetailsFileAddress}\"?",
                "Warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No,
                MessageBoxOptions.None) == MessageBoxResult.Yes)
            {
                await App.CurrentApp.RecivePackDetailsDataAsync();
            }
        }
    }
}
