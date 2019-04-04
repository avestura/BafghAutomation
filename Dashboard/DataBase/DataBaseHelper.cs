using Dashboard.UI.Controls;
using Microsoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.DataBase
{
    public static class DataBaseHelper
    {
        public static LocalHistoryEntities Entities { get; set; } = new LocalHistoryEntities();

        public static void AddSentItemToDataBase(
            string content,
            string year,
            string month,
            string day,
            string hour,
            string minute,
            string second)
        {
            Entities.SentItems.Add(new SentItems()
            {
                Content = content,
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute,
                Second = second
            });
            Entities.SaveChanges();
        }

        public static void AddSentItemToDatabase(this DataInfoPresenter info)
        {
            AddSentItemToDataBase(info.Content.ToString(), info.NowYear.ToString(),
                info.NowMonth.ToString(), info.NowDay.ToString(), info.NowHour.ToString(),
                info.NowMinutes.ToString(), info.NowSeconds.ToString());
        }

        public static ObservableCollection<DataInfoPresenter> GetSentItemsForThisDate(this DatePicker dp)
        {
            var pc = new PersianCalendar();
            string year = pc.GetYear(dp.SelectedDate.Value).ToString();
            string month = pc.GetMonth(dp.SelectedDate.Value).ToString();
            string day = pc.GetDayOfMonth(dp.SelectedDate.Value).ToString();

            return GetSentItems(year, month, day);
        }

        public static ObservableCollection<DataInfoPresenter> GetSentItems(string year, string month, string day)
        {
            var result = new ObservableCollection<DataInfoPresenter>();

            var extractedData = from e in Entities.SentItems where ((e.Year == year) && (e.Month == month) && (e.Day == day)) select e;

            foreach (var item in extractedData)
            {
                try
                {
                    var dataPresenter = new DataInfoPresenter(false,
                        item.Year,
                        item.Month,
                        item.Day,
                        item.Hour,
                        item.Minute,
                        item.Second)
                    {
                        Content = item.Content,
                        Height = 30,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(2, 3, 2, 3),
                        Waiting = false,
                        DataInfoStatus = DataInfoPresenterEnum.PresenterStatus.FileSent,
                    };

                    result.Insert(0, dataPresenter);
                }
                catch { continue; }
            }

            return result;
        }

        public static ObservableCollection<PackView> GetPackItemsForThisDate(this DatePicker dp)
        {
            var pc = new PersianCalendar();
            string year = pc.GetYear(dp.SelectedDate.Value).ToString("0000");
            string month = pc.GetMonth(dp.SelectedDate.Value).ToString("00");
            string day = pc.GetDayOfMonth(dp.SelectedDate.Value).ToString("00");

            return GetPackItems(year, month, day);
        }

        public static ObservableCollection<PackView> GetPackItems(string year, string month, string day)
        {
            var result = new ObservableCollection<PackView>();

            string dateToFind = $"{year}{month}{day}";

            var extractedData = from e in Entities.Packs where ( e.Date == dateToFind ) select e;

            foreach (var item in extractedData)
            {
                try
                {
                    var itemCodeInfo = (from ic in Entities.ItemCodes where ( ic.ItemCode == item.ItemCode ) select ic).First();

                    string itemHour = "";
                    string itemMin = "";

                    try
                    {
                        itemHour = item.Time.Substring(0, 2);
                        itemMin = item.Time.Substring(2, 2);
                    } catch { }

                    var viewPresenter = new PackView(
                        PackNumber: item.PackNo,
                        DateAndTime: $"{year}/{month}/{day} at {itemHour}:{itemMin}",
                        Weight: item.Weight + " kg",
                        ItemCode: item.ItemCode,
                        Length: itemCodeInfo.Length + " meter",
                        Diameter: itemCodeInfo.Diameter,
                        Grade: itemCodeInfo.SignID)
                    {
                        Margin = new Thickness(10,10,10,5)
                    }
                    ;

                    result.Insert(0, viewPresenter);
            } catch {  }
        }

            return result;
        }
    }
}
