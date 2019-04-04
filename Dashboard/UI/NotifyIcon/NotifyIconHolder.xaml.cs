﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.UI.NotifyIcon
{
    public partial class NotifyIconHolder
    {
        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Current.MainWindow.Show();
        }

        private async void MenuItem_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            App.TryClosingPort();
            await Task.Delay(100);
            App.Current.Shutdown();
        }
    }
}
