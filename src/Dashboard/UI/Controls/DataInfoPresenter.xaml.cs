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
using System.Globalization;
using static Dashboard.UI.Controls.DataInfoPresenterEnum;
using System.Net.Http;
using System.Diagnostics.Contracts;

namespace Dashboard.UI.Controls
{
    /// <summary>
    /// Interaction logic for DataInfoPresenter.xaml
    /// </summary>
    public partial class DataInfoPresenter : UserControl
    {
        public DataInfoPresenter(bool automaticTimeAndDateSet = true,
                                 string year = "",
                                 string month = "",
                                 string day = "",
                                 string hour = "",
                                 string minute = "",
                                 string second = "")
        {
            if (automaticTimeAndDateSet)
            {
                NowYear = Calendar.GetYear(DateTime.Now);
                NowMonth = Calendar.GetMonth(DateTime.Now);
                NowDay = Calendar.GetDayOfMonth(DateTime.Now);

                NowHour = DateTime.Now.Hour;
                NowMinutes = DateTime.Now.Minute;
                NowSeconds = DateTime.Now.Second;
            }
            else
            {
                NowYear = int.Parse(year);
                NowMonth = int.Parse(month);
                NowDay = int.Parse(day);

                NowHour = int.Parse(hour);
                NowMinutes = int.Parse(minute);
                NowSeconds = int.Parse(second);
            }

            DataDetails = $"{NowYear}/{NowMonth}/{NowDay} at {NowHour}:{NowMinutes}:{NowSeconds}";

            InitializeComponent();
        }

        private static readonly HttpClient client = new HttpClient();

        public bool Sent { get; set; } = false;

        public static PersianCalendar Calendar = new PersianCalendar();

        public PresenterStatus DataInfoStatus
        {
            get => (PresenterStatus)GetValue(DataInfoStatusProperty);
            set => SetValue(DataInfoStatusProperty, value);
        }

        public static readonly DependencyProperty DataInfoStatusProperty =
            DependencyProperty.Register("DataInfoStatus", typeof(PresenterStatus), typeof(DataInfoPresenter), null);

        public string DataDetails
        {
            get => (string)GetValue(DataInfoStatusProperty);
            private set => SetValue(DataDetailsProperty, value);
        }

        public static readonly DependencyProperty DataDetailsProperty =
            DependencyProperty.Register("DataDetails", typeof(string), typeof(DataInfoPresenter), null);

        public bool Waiting
        {
            get => (bool)GetValue(WaitingProperty);
            set => SetValue(WaitingProperty, value);
        }

        public static readonly DependencyProperty WaitingProperty =
            DependencyProperty.Register("Waiting", typeof(bool), typeof(DataInfoPresenter), null);

        public int NowYear
        {
            get => (int)GetValue(NowYearProperty);
            set => SetValue(NowYearProperty, value);
        }

        public static readonly DependencyProperty NowYearProperty =
            DependencyProperty.Register("NowYear", typeof(int), typeof(DataInfoPresenter), null);

        public int NowMonth
        {
            get => (int)GetValue(NowMonthProperty);
            set => SetValue(NowMonthProperty, value);
        }

        public static readonly DependencyProperty NowMonthProperty =
            DependencyProperty.Register("NowMonth", typeof(int), typeof(DataInfoPresenter), null);

        public int NowDay
        {
            get => (int)GetValue(NowDayProperty);
            set => SetValue(NowDayProperty, value);
        }

        public static readonly DependencyProperty NowDayProperty =
            DependencyProperty.Register("NowDay", typeof(int), typeof(DataInfoPresenter), null);

        public int NowHour
        {
            get => (int)GetValue(NowHourProperty);
            set => SetValue(NowHourProperty, value);
        }

        public static readonly DependencyProperty NowHourProperty =
            DependencyProperty.Register("NowHour", typeof(int), typeof(DataInfoPresenter), null);

        public int NowMinutes
        {
            get => (int)GetValue(NowMinutesProperty);
            set => SetValue(NowMinutesProperty, value);
        }

        public static readonly DependencyProperty NowMinutesProperty =
            DependencyProperty.Register("NowMinutes", typeof(int), typeof(DataInfoPresenter), null);

        public int NowSeconds
        {
            get => (int)GetValue(NowSecondsProperty);
            set => SetValue(NowSecondsProperty, value);
        }

        public static readonly DependencyProperty NowSecondsProperty =
            DependencyProperty.Register("NowSeconds", typeof(int), typeof(DataInfoPresenter), null);

        public string GetFormatedUploadUrl()
        {
            string date = $"{NowYear}{NowMonth.ToString("00")}{NowDay.ToString("00")}";
            string time = $"{NowHour.ToString("00")}{NowMinutes.ToString("00")}{NowSeconds.ToString("00")}";
            // TODO: Move hard-coded url to some safe place, also separate logic
            const string url = "http://honar-e-mandegar.ir/bafgh/bafgh-label/insert";

            return $"{url}?data=" + '{' + $"\"datas\":[\"{Content.ToString().Replace(" kg", "")},{date},{time}\"]" + '}';
        }

        public async Task<string> SendDataToServer()
        {
            string date = $"{NowYear}{NowMonth.ToString("00")}{NowDay.ToString("00")}";
            string time = $"{NowHour.ToString("00")}{NowMinutes.ToString("00")}{NowSeconds.ToString("00")}";

            // TODO: Move hard-coded url to some safe place, also separate logic
            const string url = "http://honar-e-mandegar.ir/bafgh/bafgh-label/insert";
            var postData = new Dictionary<string, string>()
            {
                { "data", '{' + $"\"datas\":[\"{Content.ToString().Replace(" kg", "")},{date},{time}\"]" + '}' },
                { "machine" , App.CurrentApp.AppConfiguration.AliasName }
            };

            var content = new FormUrlEncodedContent(postData);

            var response = await client.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync();
        }

        [Pure]
        public DataInfoPresenter CreatePureCopy()
        {
            return new DataInfoPresenter()
            {
                Waiting = Waiting,
                NowYear = NowYear,
                NowMonth = NowMonth,
                NowDay = NowDay,
                NowHour = NowHour,
                NowMinutes = NowMinutes,
                NowSeconds = NowSeconds,
                DataInfoStatus = DataInfoStatus,
                Content = Content,
                Width  = Width,
                Height = Height,
                Margin = Margin,
                VerticalAlignment = VerticalAlignment,
                VerticalContentAlignment = VerticalContentAlignment
            };
        }
    }
}
