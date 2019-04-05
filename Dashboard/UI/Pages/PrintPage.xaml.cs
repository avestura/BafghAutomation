using Dashboard.UI.Assets.PrintAssets;
using Dashboard.UI.Controls;
using Dashboard.UI.Controls.DesignViewControls;
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
            var DESIGN_FixedPage = new PrintResource().Resources["fPage"] as FixedPage;
            var DESIGN_Image = DESIGN_FixedPage.Children[0] as Image;
            var DESIGN_Canvas = DESIGN_FixedPage.Children[1] as Canvas;

            DESIGN_Canvas.DataContext = new PageReportData() {
                   Dia = dia,
                   Grade = grade,
                   Length = length,
                   Proc = proc,
                   StdNo = stdNo,
                   Weight = weight,
                   BarCodeData = barcodeData
            } ;

            LoadDesign(DESIGN_FixedPage, DESIGN_Canvas, DESIGN_Image);
            Document.Pages.Add(new PageContent()
            {
                Child = DESIGN_FixedPage
            });

            DocViewer.FitToMaxPagesAcross();
        }

        public void LoadDesign(FixedPage DESIGN_FixedPage, Canvas DESIGN_Canvas, Image DESIGN_Image)
        {

            var conf = App.CurrentApp.AppConfiguration.DesignModel;
            bool isDefaultImage = string.IsNullOrEmpty(conf.ImageBackgroundSource) || conf.ImageBackgroundSource == "pack://application:,,,/Dashboard;component/Resources/Images/ReportDefault - NO.png";
            var imgUri = isDefaultImage ?
                "pack://application:,,,/Dashboard;component/Resources/Images/ReportDefault - NO.png" : conf.ImageBackgroundSource;
            var image = new BitmapImage(new Uri(imgUri));
            var height = image.Height;
            var width = image.Width;
            DESIGN_FixedPage.Width = DESIGN_Image.Width = DESIGN_Canvas.Width = width;
            DESIGN_FixedPage.Height = DESIGN_Image.Height = DESIGN_Canvas.Height = height;

            DESIGN_Image.Source = image;

            foreach (var item in conf.Textboxes)
            {
                var textblock = new BindableTextBlock(item.Type, disableContextMenu: true)
                {
                    Width = item.Width,
                    Height = item.Height,
                    Tag = (item.IsBound) ? $"BINDTO:{item.BindingTag}" : "",
                    Designing = false
                };
                if (item.IsBound)
                {
                    textblock.SetBinding(BindableTextBlock.TextProperty, item.BindingTag);
                }
                else if (item.Type == BindableTextType.BarCode)
                {
                    textblock.SetBinding(BindableTextBlock.TextProperty, "BarCodeUI");
                }
                else
                {
                    textblock.Text = item.Text;
                }
                DESIGN_Canvas.Children.Add(textblock);
                Canvas.SetLeft(textblock, item.CanvasLeft);
                Canvas.SetTop(textblock, item.CanvasTop);
            }
        }
    }
}
