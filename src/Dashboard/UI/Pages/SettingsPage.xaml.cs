﻿using Dashboard.DataBase;
using Dashboard.UI.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace Dashboard.UI.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            if (ClearHistoryButton.Tag.ToString()?.Length == 0)
            {
                ClearHistoryButton.Tag = "deleteConfirm";
                ClearHistoryButtonMessage.Text = "All data will be lost. Are you sure? (Click to erase)";
            }
            else
            {
                DataBaseHelper.Entities.Database.ExecuteSqlCommand("DELETE FROM [SentItems]");
                DataBaseHelper.Entities.Database.ExecuteSqlCommand("DELETE FROM [Packs]");

                App.CurrentApp.MainPage.SentInfoPresenters.Clear();
                App.CurrentApp.MainPage.PackPresenters.Clear();
                App.CurrentApp.MainPage.PackViewRefreshRequest();

                ClearHistoryButton.Tag = "";
                ClearHistoryButtonMessage.Text = "Clear all history data for this instance of application";
            }
        }

        public async Task SaveSettingsAsync()
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
            }
            catch
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
            }
            catch
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
                App.CurrentApp.AppConfiguration.COMPortName =
                    (App.CurrentApp.AppConfiguration.COMPortName.Trim() != string.Empty) ? ComPortTextBox.Text : "No Name";

                App.CurrentApp.AppConfiguration.BaudRate = int.Parse(BaudRateComboBox.Text);
                App.CurrentApp.AppConfiguration.ParityType = (Parity)ParityComboBox.SelectedIndex;
                App.CurrentApp.AppConfiguration.DataBits = int.Parse(DatabitsTextbox.Text);
                App.CurrentApp.AppConfiguration.StopBits = (StopBits)StopbitsCombobox.SelectedIndex;

                App.CurrentApp.AppConfiguration.AliasName = AliasTextBox.Text;
                App.CurrentApp.AppConfiguration.StabilityTime = stableDelay;
                App.CurrentApp.AppConfiguration.RepetitiveDelayTime = repetitiveDelay;

                App.CurrentApp.AppConfiguration.LastWeightFileAddress = LastWeightAddressTextbox.Text;
                App.CurrentApp.AppConfiguration.PackDetailsFileAddress = PackAddressTextbox.Text;
                App.CurrentApp.AppConfiguration.PrintStdNo = PrintStdNoTextbox.Text;
                App.CurrentApp.AppConfiguration.PrintProProcedure = PrintProcedureTextbox.Text;
                App.CurrentApp.AppConfiguration.PrintReversed = PrintReversed.IsChecked.Value;
                App.CurrentApp.AppConfiguration.PrintWithRemovedBackground = RemoveBackground.IsChecked.Value;
                //App.CurrentApp.AppConfiguration.PrintBackgroundImageAddress = BackgroundImageUriTextbox.Text;

                bool valid = double.TryParse(PrintSizeScaleFactorTextbox.Text, out double tempScale);
                App.CurrentApp.AppConfiguration.ScaleFactor = (!valid || tempScale > 1000 || tempScale < 0.001) ? 1 : tempScale;
                PrintSizeScaleFactorTextbox.Text = App.CurrentApp.AppConfiguration.ScaleFactor.ToString();

                App.CurrentApp.AppConfiguration.EndTrimLength = ushort.Parse(LengthEndTrimTextBox.Text);

                App.CurrentApp.AppConfiguration.SaveSettingsToFile();

                SettingsAlert.Content = "Settings file successfully saved and program behavior updated";
                SettingsAlert.AlertType = Alert.AlertTypes.Success;
                SettingsAlert.ShowUsingLinearAnimation();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    SettingsAlert.Dispatcher.Invoke(() => SettingsAlert.HideUsingLinearAnimation());
                }).Forget();
            }
            catch (Exception ex)
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

                App.TryClosingPorts();
                await Task.Delay(100);
                Application.Current.Shutdown();
            }
        }

        public void LoadSettings()
        {
                Config.LoadSettingsFromFile();
                AliasTextBox.Text = App.CurrentApp.AppConfiguration.AliasName;

                StableDelayTextbox.Text = App.CurrentApp.AppConfiguration.StabilityTime.ToString();
                RepetitiveDelayTextBox.Text = App.CurrentApp.AppConfiguration.RepetitiveDelayTime.ToString();

                ComPortTextBox.Text = App.CurrentApp.AppConfiguration.COMPortName;
                BaudRateComboBox.SelectedIndex = ConvertBaudrateToIndex(App.CurrentApp.AppConfiguration.BaudRate);
                ParityComboBox.SelectedIndex = (int)App.CurrentApp.AppConfiguration.ParityType;
                DatabitsTextbox.Text = App.CurrentApp.AppConfiguration.DataBits.ToString();
                StopbitsCombobox.SelectedIndex = (int)App.CurrentApp.AppConfiguration.StopBits;

                LastWeightAddressTextbox.Text = App.CurrentApp.AppConfiguration.LastWeightFileAddress;
                PackAddressTextbox.Text = App.CurrentApp.AppConfiguration.PackDetailsFileAddress;

                PrintStdNoTextbox.Text = App.CurrentApp.AppConfiguration.PrintStdNo;
                PrintProcedureTextbox.Text = App.CurrentApp.AppConfiguration.PrintProProcedure;
                PrintSizeScaleFactorTextbox.Text = App.CurrentApp.AppConfiguration.ScaleFactor.ToString();
                PrintReversed.IsChecked = App.CurrentApp.AppConfiguration.PrintReversed;
                RemoveBackground.IsChecked = App.CurrentApp.AppConfiguration.PrintWithRemovedBackground;
                //BackgroundImageUriTextbox.Text = App.CurrentApp.AppConfiguration.PrintBackgroundImageAddress;

                LengthEndTrimTextBox.Text = App.CurrentApp.AppConfiguration.EndTrimLength.ToString();

                // Tag : Do restart?
                SaveButton.Tag = false;

                SaveButtonText.Text = "Save";
                SaveButtonIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;

                ClearHistoryButton.Tag = "";
                ClearHistoryButtonMessage.Text = "Clear all history data for this instance of application";
        }

        private void SensitiveData_Changed()
        {
            if (IsLoaded)
            {
                // Tag = Do restart?
                SaveButton.Tag = true;

                SaveButtonText.Text = "Save and Restart";
                SaveButtonIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Repeat;
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
                //else if (tag == "Image")
                //    BackgroundImageUriTextbox.Text = openDia.FileName;
            }
        }

        private void RegenerateItemCodesButton_Click(object sender, RoutedEventArgs e)
        {
            if (RegenerateItemCodesButton.Tag.ToString()?.Length == 0)
            {
                RegenerateItemCodesButton.Tag = "regenConfirm";
                RegenerateItemCodesButtonMessage.Text = "All data will be lost and new data will be replaced. Are you sure? (Click to regenerate)";
            }
            else
            {
                DataBaseHelper.Entities.Database.ExecuteSqlCommand("DELETE FROM [Goods]");
                BafghAutomation.Engine.DefaultGoods.Goods.ToList().ForEach(g => DataBaseHelper.Entities.Goods.Add(g));
                DataBaseHelper.Entities.SaveChanges();
                RegenerateItemCodesButton.Tag = "";
                RegenerateItemCodesButtonMessage.Text = "Regenerate Item Codes with it's default values";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveSettingsAsync();
        }

        private void SensitiveSetting_TextChanged(object sender, TextChangedEventArgs e) => SensitiveData_Changed();

        private void SensitiveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => SensitiveData_Changed();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }
    }
}
