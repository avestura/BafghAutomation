using BafghAutomation.Engine.Models;
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

using static BafghAutomation.Engine.Utils;

namespace Dashboard.DataBase
{
    public static class DataBaseHelper
    {
        public static AppDataContext Entities { get; set; } = new AppDataContext();

        public static void AddSentItemToDataBase(
            string content,
            string year,
            string month,
            string day,
            string hour,
            string minute,
            string second)
        {
            Entities.SentItems.Add(new SentItem(
                id: 0,
                second: second,
                minute: minute,
                hour: hour,
                day: day,
                month: month,
                year: year,
                content: content
                ));
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

            return GetPackItems(false, (year, month, day));
        }

        public static ObservableCollection<PackView> GetAllPackViews() => GetPackItems(true, ("", "", ""));

        public static ObservableCollection<PackView> GetPackItems(bool getAll, (string year, string month, string day) date)
        {
            var result = new ObservableCollection<PackView>();

            string dateToFind = date.year + date.month + date.day;

            var extractedData = getAll ? Entities.Packs : from e in Entities.Packs where ( e.Date == dateToFind ) select e;

            foreach (var item in extractedData)
            {
                try
                {
                    var itemCodeInfo = (from ic in Entities.Goods where ( ic.ItemCode == item.ItemCode ) select ic).First();

                    (string year, string month, string day) =
                        getAll ? GetDateFromString(item.Date) : (date.year, date.month, date.day);

                    (string hour, string min) = GetTimeFromString(item.Time);

                    var viewPresenter = new PackView(
                        id: item.Id,
                        PackNumber: item.PackNo,
                        DateAndTime: $"{year}/{month}/{day} at {hour}:{min}",
                        Weight: item.Weight,
                        ItemCode: item.ItemCode,
                        Length: itemCodeInfo.Length,
                        Diameter: itemCodeInfo.Diameter,
                        Grade: itemCodeInfo.SignId,
                        IsPrinted: item.IsPrinted,
                        NumberOfPrints: item.NumberOfPrints)
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
