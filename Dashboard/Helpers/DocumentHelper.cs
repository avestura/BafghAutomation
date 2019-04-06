using Dashboard.UI.Assets.PrintAssets;
using Dashboard.UI.Controls.DesignViewControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dashboard.Helpers
{


    public static class DocumentHelper
    {
        public static FixedPage GetFixedPage(PageReportData data, bool reverse, bool noBackground)
        {
            // TODO: Remove HardCoded UI Query
            var DESIGN_FixedPage = new PrintResource().Resources["fPage"] as FixedPage;
            var DESIGN_Image = DESIGN_FixedPage.Children[0] as System.Windows.Controls.Image;
            var DESIGN_Canvas = DESIGN_FixedPage.Children[1] as Canvas;

            DESIGN_Canvas.DataContext = new PageReportData()
            {
                Dia = data.Dia,
                Grade = data.Grade,
                Length = data.Length,
                Proc = data.Proc,
                StdNo = data.StdNo,
                Weight = data.Weight,
                BarCodeData = data.BarCodeData
            };

            LoadDesign(DESIGN_FixedPage, DESIGN_Canvas, DESIGN_Image);

            if (reverse)
            {
                DESIGN_FixedPage.LayoutTransform = new RotateTransform(180);
            }
            if (noBackground)
            {
                DESIGN_Image.Visibility = Visibility.Hidden;
            }

            return DESIGN_FixedPage;
        }

        private static void LoadDesign(FixedPage DESIGN_FixedPage, Canvas DESIGN_Canvas, Image DESIGN_Image)
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
                    Designing = false,
                    FontSize = 20,
                    FontFamily = (item.Type == BindableTextType.Normal) ? new FontFamily("Times New Roman")  : App.Current.Resources["BarcodeExtended"] as FontFamily
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
