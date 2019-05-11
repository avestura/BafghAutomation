using BafghAutomation.Engine.Models;
using Dashboard.DataBase;
using Dashboard.Helpers;
using Dashboard.Models;
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
        private readonly int id;
        private readonly string length;
        private readonly string weight;
        private readonly string stdNo;
        private readonly string proc;
        private readonly string grade;
        private readonly string dia;
        private readonly string barcodeData;

        public PrintPage(int id, string len, string weight, string stdNo, string proc, string grade, string dia, string barcodeData)
        {
            InitializeComponent();

            this.id = id;
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

            DocViewer.PrintCompleted += DocViewer_PrintCompleted;
        }

        private void DocViewer_PrintCompleted(object sender, Controls.PrintCompletedEventArgs e)
        {
            try
            {
                if(DataBaseHelper.Entities.Packs.Find(id) is Pack p)
                {
                    p.IsPrinted = true;
                    p.NumberOfPrints++;
                    DataBaseHelper.Entities.SaveChanges();
                    App.CurrentApp.MainPage.PackViewRefreshRequest();
                }
            }
            catch { }
        }

        public void InitializeResourcePage()
        {
            var DESIGN_FixedPage = DocumentHelper.GetFixedPage(new PageReportModel
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
