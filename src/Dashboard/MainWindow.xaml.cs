using Dashboard.DataBase;
using Dashboard.Helpers;
using Dashboard.UI.Controls;
using Dashboard.UI.Pages;
using Dashboard.UI.Windows;
using Microsoft.Win32;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool settingOpenedClickedOnce;
        private bool settingClosedClickedOnce;

        public MainWindow()
        {
            App.CurrentApp.AppWindow = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => Overlay.Visibility = Visibility.Collapsed;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => App.TryClosingPorts();

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


        private async void SettingsClose_Click(object sender, RoutedEventArgs e)
        {
            if (!settingClosedClickedOnce)
            {
                settingOpenedClickedOnce = false;
                settingClosedClickedOnce = true;

                App.CurrentApp.AppConfiguration.SaveSettingsToFile();

                await Overlay.HideUsingLinearAnimationAsync(250);
                settingClosedClickedOnce = false;

            }
        }

        private async void SettingsOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!settingOpenedClickedOnce)
            {
                SettingsView.Navigate(new SettingsPage());

                settingOpenedClickedOnce = true;
                await Overlay.ShowUsingLinearAnimationAsync(250);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => App.Current.MainWindow.Hide();

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

        private void MainFrame_Navigated(object sender, NavigationEventArgs e) =>
            new Action(() =>
            MainFrame.MarginFadeInAnimation(new Thickness(20, 0, 0, 0), new Thickness(1, 0, 1, 0), TimeSpan.FromMilliseconds(500))
            ).Try();

        private void HomePageClick(object sender, RoutedEventArgs e) => App.CurrentApp.AppWindow.MainFrame.Navigate(App.CurrentApp.MainPage);

        private void Open_PrintViewDesigner(object sender, RoutedEventArgs e)
        {
            new PrintViewDesigner().Show();
        }
    }
}
