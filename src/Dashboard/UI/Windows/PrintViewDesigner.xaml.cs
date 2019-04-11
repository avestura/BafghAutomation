using Dashboard.UI.Controls.DesignViewControls;
using Dashboard.UI.Handlers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xaml;
using System.Xml;

namespace Dashboard.UI.Windows
{
    /// <summary>
    /// Interaction logic for PrintViewDesigner.xaml
    /// </summary>
    public partial class PrintViewDesigner : Window
    {
        private BasicDesignControlDragDropHandler dragDropHandler;

        public PrintViewDesigner()
        {
            DataContext = new
            {
                StdNo = "(Standard Number)",
                Dia = "(Normal Dia)",
                Grade = "(Grade)",
                Weight = "(Parcel Weight)",
                Proc = "(Pro Procedure)",
                Length = "(Length)",
                BarCodeData = "[0000 1111 2222 3333]",
                BarCodeUI = "123456789",
                TextEditEnabled = true
            };

            InitializeComponent();
        }

        private void ImageBrowser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png",
                Title = "Open Image",
                Multiselect = false
            };

            if(dialog.ShowDialog() == true)
            {
                var url = dialog.FileName;
                var image = new BitmapImage(new Uri(url));
                var height = image.Height;
                var width = image.Width;

                DESIGN_FixedPage.Width = DESIGN_Image.Width = DESIGN_Canvas.Width = width;
                DESIGN_FixedPage.Height = DESIGN_Image.Height = DESIGN_Canvas.Height = height;
                dragDropHandler.RemoveEventHandlers();
                dragDropHandler = new BasicDesignControlDragDropHandler(DESIGN_Canvas)
                {
                    ConstraintArea = new Point(width, height),
                    ConstraintToBounds = true
                };
                DESIGN_Image.Source = image;
                DESIGN_Canvas.Children.Clear();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSavedDesign();
        }

        public void LoadSavedDesign()
        {
            /// TODO: Defined XAML for this designer is as same as the one in
            /// <see cref="Dashboard.UI.Assets.PrintAssets.PrintResource"/>. Make them a
            /// Single one to handle it better. See Bookmarks with tag #XAML_DESIGN_DUP for more info

            var conf = App.CurrentApp.AppConfiguration.DesignModel;
            bool isDefaultImage = string.IsNullOrEmpty(conf.ImageBackgroundSource);
            var imgUri = isDefaultImage ?
                "pack://application:,,,/Dashboard;component/Resources/Images/ReportDefault - NO.png" : conf.ImageBackgroundSource;
            var image = new BitmapImage(new Uri(imgUri));
            var height = image.Height;
            var width = image.Width;
            DESIGN_FixedPage.Width = DESIGN_Image.Width = DESIGN_Canvas.Width = width;
            DESIGN_FixedPage.Height = DESIGN_Image.Height = DESIGN_Canvas.Height = height;

            DESIGN_Image.Source = image;
            dragDropHandler = new BasicDesignControlDragDropHandler(DESIGN_Canvas)
            {
                ConstraintToBounds = true,
                ConstraintArea = new Point(DESIGN_Image.Width, DESIGN_Image.Height)
            };

            foreach (var item in conf.Textboxes)
            {
                var textblock = new BindableTextBlock(item.Type)
                {
                    Width = item.Width,
                    Height = item.Height,
                    Tag = (item.IsBound) ? $"BINDTO:{item.BindingTag}" : "",
                    Text = item.Text,
                    TextFontWeight = item.Type == BindableTextType.Normal ? FontWeights.Bold : FontWeights.Normal,
                    Designing = true
                };
                if (item.IsBound)
                {
                    SetBinding(BindableTextBlock.TextProperty, item.BindingTag);
                }
                DESIGN_Canvas.Children.Add(textblock);
                Canvas.SetLeft(textblock, item.CanvasLeft);
                Canvas.SetTop(textblock, item.CanvasTop);
            }

            if (isDefaultImage) Slider.Value = 60;
        }

        private void SaveDesign_Click(object sender, RoutedEventArgs e)
        {
            var conf = App.CurrentApp.AppConfiguration.DesignModel;
            conf.ImageBackgroundSource = (DESIGN_Image.Source as BitmapImage)?.UriSource.AbsoluteUri;
            conf.Textboxes.Clear();
            foreach(var item in DESIGN_Canvas.Children)
            {
                if(item is BindableTextBlock tblock)
                {
                    conf.Textboxes.Add(new Models.BindableTextboxSaveModel
                    {
                        Width = tblock.ActualWidth,
                        Height = tblock.ActualHeight,
                        CanvasTop = Canvas.GetTop(tblock),
                        CanvasLeft = Canvas.GetLeft(tblock),
                        FontWeight = tblock.FontWeight,
                        IsBound = tblock.Tag?.ToString().StartsWith("BINDTO:") ?? false,
                        BindingTag = tblock.Tag?.ToString().Replace("BINDTO:", "") ?? "",
                        RotationDegree = (tblock.RenderTransform as RotateTransform)?.Angle ?? 0,
                        Text = tblock.Text,
                        Type = tblock.Type
                    });
                }
            }

            App.CurrentApp.AppConfiguration.SaveSettingsToFile();
            Close();
        }

        private void DiscardDesign_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you Sure? all design will be lost.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void AddTextBlockToDesign(object sender, MouseButtonEventArgs e)
        {
            var textblock = new BindableTextBlock(BindableTextType.Normal) {
                Designing = true,
                Height = 50
            };
            DESIGN_Canvas.Children.Add(textblock);
            Canvas.SetLeft(textblock, 10);
            Canvas.SetTop(textblock, 10);
        }

        private void AddBarcodeToDesign_Click(object sender, MouseButtonEventArgs e)
        {
            var textblock = new BindableTextBlock(BindableTextType.BarCode) {
                Designing = true,
                Height = 30
            };
            DESIGN_Canvas.Children.Add(textblock);
            Canvas.SetLeft(textblock, 10);
            Canvas.SetTop(textblock, 10);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {   if (dragDropHandler != null && DESIGN_TopGrid != null)
            {
                dragDropHandler.ZoomLevel = Slider.Value / 100f;
                DESIGN_TopGrid.LayoutTransform = new ScaleTransform(Slider.Value / 100f, Slider.Value / 100f);
            }
        }
    }
}
