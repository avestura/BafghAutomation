using Dashboard.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public static class Extensions
    {
        public static DataInfoPresenter GetFirstUnsend(this ObservableCollection<DataInfoPresenter> collection)
        {
            if (collection == null) return null;
            return collection.FirstOrDefault(item => !item.Sent);
        }

        public static void Forget(this Task task) { }

        public static string RemoveLast(this string s, ushort k) => s.Substring(0, s.Length - k);

        public static Uri GetPrintImageUri()
        {
            Uri imageAddr = null;

            if (App.GetApp().AppConfiguration.PrintBackgroundImageAddress?.Length == 0)
            {
                imageAddr = new Uri("pack://application:,,,/Dashboard;component/Resources/Images/ReportDefault - NO.png", UriKind.Absolute);
            } else
            {
                try
                {
                    imageAddr = new Uri(App.GetApp().AppConfiguration.PrintBackgroundImageAddress, UriKind.Absolute);
                } catch
                {
                    imageAddr = new Uri("pack://application:,,,/Dashboard;component/Resources/Images/ReportDefault - NO.png", UriKind.Absolute);
                }
            }

            return imageAddr;
        }
    }
}
