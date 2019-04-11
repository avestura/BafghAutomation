using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.UI.NotifyIcon
{
    public partial class NotifyIconHolder
    {
        private void OpenApp_Click(object sender, System.Windows.RoutedEventArgs e) => App.Current.MainWindow.Show();

        private async void Shutdown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.TryClosingPorts();
            await Task.Delay(100);
            App.Current.Shutdown();
        }
    }
}
