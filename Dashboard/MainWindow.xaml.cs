using Dashboard.DataBase;
using Dashboard.IO;
using Dashboard.UI.Controls;
using Dashboard.UI.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
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

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            App.GetApp().AppWindow = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Collapsed;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.TryClosingPort();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                MaximizeRestoreImage.Icon = FontAwesome.WPF.FontAwesomeIcon.WindowRestore;
            }
            else
            {
                MaximizeRestoreImage.Icon = FontAwesome.WPF.FontAwesomeIcon.WindowMaximize;
            }
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            if (ClearHistoryButton.Tag.ToString()?.Length == 0)
            {
                ClearHistoryButton.Tag = "deleteConfirm";
                ClearHistoryButtonMessage.Text = "All data will be lost. Are you sure? (Click to erase)";
            } else
            {
                DataBaseHelper.Entities.Database.ExecuteSqlCommand("DELETE FROM [SentItems]");
                DataBaseHelper.Entities.Database.ExecuteSqlCommand("DELETE FROM [Packs]");

                App.GetApp().MainPage.SentInfoPresenters.Clear();
                App.GetApp().MainPage.PackPresenters.Clear();
                App.GetApp().MainPage.PackViewRefreshRequest();

                ClearHistoryButton.Tag = "";
                ClearHistoryButtonMessage.Text = "Clear all history data for this instance of application";
            }
        }

        private void SensitiveSetting_TextChanged(object sender, TextChangedEventArgs e)
        {
            SensitiveData_Changeded();
        }

        private void SensitiveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SensitiveData_Changeded();
        }

        private void SensitiveData_Changeded()
        {
            if (IsLoaded)
            {
                // Tag = Do restart?
                SaveButton.Tag = true;

                SaveButtonText.Text = "Save and Restart";
                SaveButtonIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Repeat;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AliasTextBox.Text.Trim().Length < 1)
            {
                SettingsAlert.Content = "Choose another alias name";
                SettingsAlert.AlertType = Alert.AlertTypes.Warning;
                SettingsAlert.ShowUsingLinearAnimation();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    SettingsAlert.Dispatcher.Invoke(() => SettingsAlert.HideUsingLinearAnimation());
                }).Forget();
                return;
            }

            uint stableDelay = 0;
            try
            {
                stableDelay = uint.Parse(StableDelayTextbox.Text);
                if (stableDelay < 500)
                    throw new Exception();
            } catch
            {
                SettingsAlert.Content = "Please enter a possitive number more than 500 in stable delay field";
                SettingsAlert.AlertType = Alert.AlertTypes.Warning;
                SettingsAlert.ShowUsingLinearAnimation();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    SettingsAlert.Dispatcher.Invoke(() => SettingsAlert.HideUsingLinearAnimation());
                }).Forget();
                return;
            }

            uint repetitiveDelay = 0;
            try
            {
                repetitiveDelay = uint.Parse(RepetitiveDelayTextBox.Text);
                if (repetitiveDelay < 500)
                    throw new Exception();
            } catch
            {
                SettingsAlert.Content = "Please enter a possitive number more than 500 in repeatitive delay field";
                SettingsAlert.AlertType = Alert.AlertTypes.Warning;
                SettingsAlert.ShowUsingLinearAnimation();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    SettingsAlert.Dispatcher.Invoke(() => SettingsAlert.HideUsingLinearAnimation());
                }).Forget();
                return;
            }

            try
            {
                App.GetApp().AppConfiguration.COMPortName =
                    ( App.GetApp().AppConfiguration.COMPortName.Trim() != string.Empty ) ? ComPortTextBox.Text : "No Name";

                App.GetApp().AppConfiguration.BaudRate = int.Parse(BaudRateComboBox.Text);
                App.GetApp().AppConfiguration.ParityType = (Parity)ParityComboBox.SelectedIndex;
                App.GetApp().AppConfiguration.DataBits = int.Parse(DatabitsTextbox.Text);
                App.GetApp().AppConfiguration.StopBits = (StopBits)StopbitsCombobox.SelectedIndex;

                App.GetApp().AppConfiguration.AliasName = AliasTextBox.Text;
                App.GetApp().AppConfiguration.StabilityTime = stableDelay;
                App.GetApp().AppConfiguration.RepetitiveDelayTime = repetitiveDelay;

                App.GetApp().AppConfiguration.LastWeightFileAddress = LastWeightAddressTextbox.Text;
                App.GetApp().AppConfiguration.PackDetailsFileAddress = PackAddressTextbox.Text;
                App.GetApp().AppConfiguration.PrintStdNo = PrintStdNoTextbox.Text;
                App.GetApp().AppConfiguration.PrintProProcedure = PrintProcedureTextbox.Text;
                App.GetApp().AppConfiguration.PrintBackgroundImageAddress = BackgroundImageUriTextbox.Text;

                bool valid = double.TryParse(PrintSizeScaleFactorTextbox.Text, out double tempScale);
                App.GetApp().AppConfiguration.ScaleFactor = (!valid || tempScale > 1000 || tempScale < 0.001) ? 1 : tempScale;
                PrintSizeScaleFactorTextbox.Text = App.GetApp().AppConfiguration.ScaleFactor.ToString();

                App.GetApp().AppConfiguration.EndTrimLength = ushort.Parse(LengthEndTrimTextBox.Text);

                App.GetApp().AppConfiguration.SaveSettingsToFile();

                SettingsAlert.Content = "Settings file successfully saved and program behavior updated";
                SettingsAlert.AlertType = Alert.AlertTypes.Success;
                SettingsAlert.ShowUsingLinearAnimation();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    SettingsAlert.Dispatcher.Invoke(() => SettingsAlert.HideUsingLinearAnimation());
                }).Forget();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SettingsAlert.Content = "Settings file could not be saved";
                SettingsAlert.AlertType = Alert.AlertTypes.Failed;
                SettingsAlert.ShowUsingLinearAnimation();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    SettingsAlert.Dispatcher.Invoke(() => SettingsAlert.HideUsingLinearAnimation());
                }).Forget();
                return;
            }

            if ((bool)SaveButton.Tag)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);

                App.TryClosingPort();
                await Task.Delay(100);
                Application.Current.Shutdown();
            }
        }

        private bool settingClosedClickedOnce;

        private async void SettingsClose_Click(object sender, RoutedEventArgs e)
        {
            if (!settingClosedClickedOnce)
            {
                settingClosedClickedOnce = true;

                App.GetApp().AppConfiguration.SaveSettingsToFile();

                await Overlay.HideUsingLinearAnimationAsync(250);
                settingClosedClickedOnce = false;

            }
        }

        private readonly bool settingOpenedClickedOnce;

        private async void SettingsOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!settingOpenedClickedOnce)
            {
                Config.LoadSettingsFromFile();
                AliasTextBox.Text = App.GetApp().AppConfiguration.AliasName;

                StableDelayTextbox.Text = App.GetApp().AppConfiguration.StabilityTime.ToString();
                RepetitiveDelayTextBox.Text = App.GetApp().AppConfiguration.RepetitiveDelayTime.ToString();

                ComPortTextBox.Text = App.GetApp().AppConfiguration.COMPortName;
                BaudRateComboBox.SelectedIndex = ConvertBaudrateToIndex(App.GetApp().AppConfiguration.BaudRate);
                ParityComboBox.SelectedIndex = (int)App.GetApp().AppConfiguration.ParityType;
                DatabitsTextbox.Text = App.GetApp().AppConfiguration.DataBits.ToString();
                StopbitsCombobox.SelectedIndex = (int)App.GetApp().AppConfiguration.StopBits;

                LastWeightAddressTextbox.Text = App.GetApp().AppConfiguration.LastWeightFileAddress;
                PackAddressTextbox.Text = App.GetApp().AppConfiguration.PackDetailsFileAddress;

                PrintStdNoTextbox.Text = App.GetApp().AppConfiguration.PrintStdNo;
                PrintProcedureTextbox.Text = App.GetApp().AppConfiguration.PrintProProcedure ;
                PrintSizeScaleFactorTextbox.Text = App.GetApp().AppConfiguration.ScaleFactor.ToString();
                BackgroundImageUriTextbox.Text = App.GetApp().AppConfiguration.PrintBackgroundImageAddress;

                LengthEndTrimTextBox.Text = App.GetApp().AppConfiguration.EndTrimLength.ToString();

                // Tag : Do restart?
                SaveButton.Tag = false;

                SaveButtonText.Text = "Save";
                SaveButtonIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;

                ClearHistoryButton.Tag = "";
                ClearHistoryButtonMessage.Text = "Clear all history data for this instance of application";

                settingClosedClickedOnce = true;
                await Overlay.ShowUsingLinearAnimationAsync(250);
                settingClosedClickedOnce = false;
            }
        }

        private int ConvertBaudrateToIndex(int baudRate)
        {
            switch (baudRate)
            {
                case 110: return 0;
                case 300: return 1;
                case 600: return 2;
                case 1200: return 3;
                case 2400: return 4;
                case 4800: return 5;
                case 9600: return 6;
                case 14400: return 7;
                case 19200: return 8;
                case 28800: return 9;
                case 38400: return 10;
                case 56000: return 11;
                case 57600: return 12;
                case 115200: return 13;
            }

            return -1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Hide();
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
        }

        private void MainContainer_Loaded(object sender, RoutedEventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Version ver = assembly.GetName().Version;

            VersionString.Text = ver.ToString();
        }

        private void ReadWriteToAddressSelect(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button)?.Tag.ToString();

            string title = "";
            string filter = "";

            switch (tag)
            {
                case "Write":
                    title = "Write File Select"; break;
                case "Read":
                    title = "Read File Select"; break;
                case "Image":
                    title = "Background Image Photo Select"; break;
            }

            switch (tag)
            {
                case "Write":
                case "Read":
                    filter = "Text File (*.txt)|*.txt"; break;
                case "Image":
                    filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png"; break;
            }

            var openDia = new OpenFileDialog()
            {
                Filter = filter,
                Multiselect = false,
                Title = title
            };

            if (openDia.ShowDialog() == true)
            {
                if (tag == "Write")
                    LastWeightAddressTextbox.Text = openDia.FileName;
                else if (tag == "Read")
                    PackAddressTextbox.Text = openDia.FileName;
                else if (tag == "Image")
                    BackgroundImageUriTextbox.Text = openDia.FileName;
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                MainFrame.MarginFadeInAnimation(new Thickness(20,0,0,0), new Thickness(1,0,1,0), TimeSpan.FromMilliseconds(500));
            } catch  {  }
        }

        private void HomePageClick(object sender, RoutedEventArgs e)
        {
            App.GetApp().AppWindow.MainFrame.Navigate(App.GetApp().MainPage);
        }

        private void RegenerateItemCodesButton_Click(object sender, RoutedEventArgs e)
        {
            if (RegenerateItemCodesButton.Tag.ToString()?.Length == 0)
            {
                RegenerateItemCodesButton.Tag = "regenConfirm";
                RegenerateItemCodesButtonMessage.Text = "All data will be lost and new data will be replaced. Are you sure? (Click to regenerate)";
            } else
            {
                DataBaseHelper.Entities.Database.ExecuteSqlCommand("DELETE FROM [ItemCodes]");
                #region Data ReGenerate Region
                DataBaseHelper.Entities.ItemCodes.Add(
                    new ItemCodes()
                    {
                        Diameter = "8",
                        ItemCode = "7080130001",
                        Length = "12",
                        SignID = "A3"
                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "10",
                                        ItemCode = "7080130002",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "12",
                                        ItemCode = "7080130003",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "14",
                                        ItemCode = "7080130004",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "16",
                                        ItemCode = "7080130005",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "18",
                                        ItemCode = "7080130006",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "20",
                                        ItemCode = "7080130007",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "22",
                                        ItemCode = "7080130008",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "25",
                                        ItemCode = "7080130009",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "28",
                                        ItemCode = "70801300010",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "30",
                                        ItemCode = "70801300011",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "32",
                                        ItemCode = "70801300012",
                                        Length = "12",
                                        SignID = "A3"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "8",
                                        ItemCode = "7080150001",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "10",
                                        ItemCode = "7080150002",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "12",
                                        ItemCode = "7080150003",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "14",
                                        ItemCode = "7080150004",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "16",
                                        ItemCode = "7080150005",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "18",
                                        ItemCode = "7080150006",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "20",
                                        ItemCode = "7080150007",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "22",
                                        ItemCode = "7080150008",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "25",
                                        ItemCode = "7080150009",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "28",
                                        ItemCode = "70801500010",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "30",
                                        ItemCode = "70801500011",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "32",
                                        ItemCode = "70801500012",
                                        Length = ">4",
                                        SignID = "A3-N"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "32",
                                        ItemCode = "7080160001",
                                        Length = ">4",
                                        SignID = "A3-G2"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "25",
                                        ItemCode = "7080160002",
                                        Length = ">4",
                                        SignID = "A3-G2"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "14",
                                        ItemCode = "7080160003",
                                        Length = ">4",
                                        SignID = "A3-G2"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "10",
                                        ItemCode = "7120520002",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "12",
                                        ItemCode = "7120520003",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "14",
                                        ItemCode = "7120520004",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "16",
                                        ItemCode = "7120520005",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "18",
                                        ItemCode = "7120520006",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "20",
                                        ItemCode = "7120520007",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "22",
                                        ItemCode = "7120520008",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "25",
                                        ItemCode = "7120520009",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "28",
                                        ItemCode = "71205200010",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                DataBaseHelper.Entities.ItemCodes.Add(
                                    new ItemCodes()
                                    {
                                        Diameter = "32",
                                        ItemCode = "71205200011",
                                        Length = ">4",
                                        SignID = "Waste"
                                    });
                #endregion
                DataBaseHelper.Entities.SaveChanges();
                RegenerateItemCodesButton.Tag = "";
                RegenerateItemCodesButtonMessage.Text = "Regenerate Item Codes with it's default values";
            }
        }
    }
}
