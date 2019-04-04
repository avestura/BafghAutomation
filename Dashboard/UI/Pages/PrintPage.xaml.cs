using Dashboard.UI.Assets.PrintAssets;
using Dashboard.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PrintPage.xaml
    /// </summary>
    public partial class PrintPage : Page
    {
        private string length;
        private string weight;
        private string stdNo;
        private string proc;
        private string grade;
        private string dia;
        private string barcodeData;

        public PrintPage(string len, string weight, string stdNo, string proc, string grade, string dia, string barcodeData)
        {
            InitializeComponent();

            this.length = len;
            this.weight = weight;
            this.stdNo  = stdNo;
            this.proc   = proc;
            this.grade  = grade;
            this.dia    = dia;
            this.barcodeData = barcodeData;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fPage = new PrintResource().Resources["fPage"] as FixedPage;
            fPage.DataContext = new PageReportData() {
                   Dia = dia,
                   Grade = grade,
                   Length = length,
                   Proc = proc,
                   StdNo = stdNo,
                   Weight = weight,
                   ImageSource = Extensions.GetPrintImageUri(),
                   BarCodeData = barcodeData
            } ;

            Document.Pages.Add(new PageContent()
            {
                Child = fPage
            });

            DocViewer.FitToMaxPagesAcross();
        }
    }
}
