using Dashboard.UI.Assets.PrintAssets;
using Dashboard.UI.Pages;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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

namespace Dashboard.UI.Controls
{
    /// <summary>
    /// Interaction logic for PackView.xaml
    /// </summary>
    public partial class PackView : UserControl
    {
        public string DateAndTime
        {
            get { return (string)GetValue(DateAndTimeProperty); }
            set { SetValue(DateAndTimeProperty, value); }
        }

        public static readonly DependencyProperty DateAndTimeProperty =
            DependencyProperty.Register("DateAndTime", typeof(string), typeof(PackView), null);

        public string Weight
        {
            get { return (string)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(string), typeof(PackView), null);

        public string PackNumber
        {
            get { return (string)GetValue(PackNumberProperty); }
            set { SetValue(PackNumberProperty, value); }
        }

        public static readonly DependencyProperty PackNumberProperty =
            DependencyProperty.Register("PackNumber", typeof(string), typeof(PackView), null);

        public string ItemCode
        {
            get { return (string)GetValue(ItemCodeProperty); }
            set { SetValue(ItemCodeProperty, value); }
        }

        public static readonly DependencyProperty ItemCodeProperty =
            DependencyProperty.Register("ItemCode", typeof(string), typeof(PackView), null);

        public string Diameter
        {
            get { return (string)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(string), typeof(PackView), null);

        public string Grade
        {
            get { return (string)GetValue(GradeProperty); }
            set { SetValue(GradeProperty, value); }
        }

        public static readonly DependencyProperty GradeProperty =
            DependencyProperty.Register("Grade", typeof(string), typeof(PackView), null);

        public string Length
        {
            get { return (string)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(string), typeof(PackView), null);

        public PackView(
            string PackNumber = "",
            string DateAndTime = "",
            string Weight = "",
            string ItemCode = "",
            string Length = "",
            string Diameter = "",
            string Grade = "")
        {
            InitializeComponent();

            this.PackNumber = PackNumber;
            this.DateAndTime = DateAndTime;
            this.Weight = Weight;
            this.ItemCode = ItemCode;
            this.Diameter = Diameter;
            this.Grade = Grade;
            this.Length = Length;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentApp.AppWindow.MainFrame.Navigate(
                new PrintPage(
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
                var fPage = new PrintResource().Resources["fPage"] as FixedPage;
                fPage.DataContext = new PageReportData()
                {
                    Dia = Diameter,
                    Grade = Grade,
                    Length = Length,
                    Proc = App.CurrentApp.AppConfiguration.PrintProProcedure,
                    StdNo = App.CurrentApp.AppConfiguration.PrintStdNo,
                    Weight = Weight,
                    ImageSource = Extensions.GetPrintImageUri(),
                    BarCodeData = PackNumber
                };

                var fDoc = new FixedDocument();
                fDoc.Pages.Add(new PageContent() { Child = fPage });

                var dialog = new PrintDialog();

                dialog.PrintVisual(fDoc.DocumentPaginator.GetPage(0).Visual, "Homatec Print Page, Yazd Co.");

                // Guide ====
                // Other method of printing can be:
                // dialog.PrintDocument(fDoc.DocumentPaginator, "HomatecCOMPPrint");
                // Guide ====

            } catch(Exception ex)
            {
                MessageBox.Show($"Can't print. Reason:\n{ex.Message}");
            }
        }
    }
}
