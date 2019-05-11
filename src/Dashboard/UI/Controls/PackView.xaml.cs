using Dashboard.DataBase;
using Dashboard.Helpers;
using Dashboard.Models;
using Dashboard.UI.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;

namespace Dashboard.UI.Controls
{
    /// <summary>
    /// Interaction logic for PackView.xaml
    /// </summary>
    public partial class PackView : UserControl
    {
        public int Id { get; }

        public string DateAndTime
        {
            get => (string)GetValue(DateAndTimeProperty);
            set => SetValue(DateAndTimeProperty, value);
        }

        public static readonly DependencyProperty DateAndTimeProperty =
            DependencyProperty.Register("DateAndTime", typeof(string), typeof(PackView), null);

        public string Weight
        {
            get => (string)GetValue(WeightProperty);
            set => SetValue(WeightProperty, value);
        }

        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(string), typeof(PackView), null);

        public string PackNumber
        {
            get => (string)GetValue(PackNumberProperty);
            set => SetValue(PackNumberProperty, value);
        }

        public static readonly DependencyProperty PackNumberProperty =
            DependencyProperty.Register("PackNumber", typeof(string), typeof(PackView), null);

        public string ItemCode
        {
            get => (string)GetValue(ItemCodeProperty);
            set => SetValue(ItemCodeProperty, value);
        }

        public static readonly DependencyProperty ItemCodeProperty =
            DependencyProperty.Register("ItemCode", typeof(string), typeof(PackView), null);

        public string Diameter
        {
            get => (string)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(string), typeof(PackView), null);

        public string Grade
        {
            get => (string)GetValue(GradeProperty);
            set => SetValue(GradeProperty, value);
        }

        public static readonly DependencyProperty GradeProperty =
            DependencyProperty.Register("Grade", typeof(string), typeof(PackView), null);

        public string Length
        {
            get => (string)GetValue(LengthProperty);
            set => SetValue(LengthProperty, value);
        }

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(string), typeof(PackView), null);


        private static Color BackgroundDefaultColor => (Color)ColorConverter.ConvertFromString("#FFF0F0F0");
        private static Color BackgroundPrintedColor => (Color)ColorConverter.ConvertFromString("#FFFFC5C5");
        private static SolidColorBrush BackgroundDefaultBrush => new SolidColorBrush(BackgroundDefaultColor);
        private static SolidColorBrush BackgroundPrintedBrush => new SolidColorBrush(BackgroundPrintedColor);

        public bool IsPrinted
        {
            get => (bool)GetValue(IsPrintedProperty);
            set
            {
                SetValue(IsPrintedProperty, value);
                if (value)
                {
                    SetValue(BackgroundBrushProperty, BackgroundPrintedBrush);
                }
                else
                {
                    SetValue(BackgroundBrushProperty, BackgroundDefaultBrush);
                }
            }
        }

        public static readonly DependencyProperty IsPrintedProperty =
            DependencyProperty.Register("IsPrinted", typeof(bool), typeof(PackView), new PropertyMetadata(false));

        public SolidColorBrush BackgroundBrush => (SolidColorBrush)GetValue(BackgroundBrushProperty);

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(SolidColorBrush), typeof(PackView), new PropertyMetadata(BackgroundDefaultBrush));

        public PackView(
            int id,
            string PackNumber = "",
            string DateAndTime = "",
            string Weight = "",
            string ItemCode = "",
            string Length = "",
            string Diameter = "",
            string Grade = "",
            bool IsPrinted = false)
        {
            InitializeComponent();

            this.Id = id;
            this.PackNumber = PackNumber;
            this.DateAndTime = DateAndTime;
            this.Weight = Weight;
            this.ItemCode = ItemCode;
            this.Diameter = Diameter;
            this.Grade = Grade;
            this.Length = Length;
            this.IsPrinted = IsPrinted;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentApp.AppWindow.MainFrame.Navigate(
                new PrintPage(
                    Id,
                    Length,
                    Weight,
                    App.CurrentApp.AppConfiguration.PrintStdNo,
                    App.CurrentApp.AppConfiguration.PrintProProcedure,
                    Grade,
                    Diameter,
                    PackNumber
                    )
                );
        }

        private void FastPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fpage = DocumentHelper.GetFixedPage(new PageReportModel
                {
                    Length = Length,
                    Weight = Weight,
                    StdNo = App.CurrentApp.AppConfiguration.PrintStdNo,
                    Proc = App.CurrentApp.AppConfiguration.PrintProProcedure,
                    Grade = Grade,
                    Dia = Diameter,
                    BarCodeData = PackNumber
                },
                reverse: App.CurrentApp.AppConfiguration.PrintReversed,
                noBackground: App.CurrentApp.AppConfiguration.PrintWithRemovedBackground);

                var doc = new FixedDocument();
                doc.Pages.Add(new PageContent { Child = fpage});

                var printDialog = new PrintDialog();
                bool? pdResult = printDialog.ShowDialog();
                if (pdResult != null && pdResult.Value)
                {
                    printDialog.PrintDocument(doc.DocumentPaginator, "HOMATEC");
                    IsPrinted = true;
                    DataBaseHelper.Entities.Packs.Find(Id).IsPrinted = true;
                    DataBaseHelper.Entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't print or save changes into database. Reason:\n{ex.Message}");
            }
        }
    }
}
