using Dashboard.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
            this.stdNo = stdNo;
            this.proc = proc;
            this.grade = grade;
            this.dia = dia;
            this.barcodeData = barcodeData;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeResourcePage();
        }

        public void InitializeResourcePage()
        {
            var DESIGN_FixedPage = DocumentHelper.GetFixedPage(new PageReportData
            {
                Dia = dia,
                Grade = grade,
                Length = length,
                Proc = proc,
                StdNo = stdNo,
                Weight = weight,
                BarCodeData = barcodeData
            },
            reverse: false,
            noBackground: false);
            // False because PrintPage handles it on its own, using documnet viewer print function


            Document.Pages.Add(new PageContent()
            {
                Child = DESIGN_FixedPage
            });

            DocViewer.FitToMaxPagesAcross();
        }


    }
}
