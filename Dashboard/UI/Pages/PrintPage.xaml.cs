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
        private readonly string length;
        private readonly string weight;
        private readonly string stdNo;
        private readonly string proc;
        private readonly string grade;
        private readonly string dia;
        private readonly string barcodeData;

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
            // Those 'false'es are because PrintPage handles it on its own, using documnet viewer print function
            // doesn're require getting info from settings

            Document.Pages.Add(new PageContent()
            {
                Child = DESIGN_FixedPage
            });

            DocViewer.FitToMaxPagesAcross();
        }


    }
}
