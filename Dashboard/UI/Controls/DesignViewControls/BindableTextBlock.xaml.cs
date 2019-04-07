using Dashboard.Helpers;
using Dashboard.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
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

namespace Dashboard.UI.Controls.DesignViewControls
{
    /// <summary>
    /// Interaction logic for BindableTextBlock.xaml
    /// </summary>
    public partial class BindableTextBlock : UserControl
    {
        public bool Designing
        {
            get => (bool)GetValue(DesigningProperty);
            set => SetValue(DesigningProperty, value);
        }

        public static readonly DependencyProperty DesigningProperty =
            DependencyProperty.Register("Designing", typeof(bool), typeof(BindableTextBlock), new PropertyMetadata(false));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BindableTextBlock), new PropertyMetadata("(Unbound Text)"));



        public FontWeight TextFontWeight
        {
            get { return (FontWeight)GetValue(TextFontWeightProperty); }
            set { SetValue(TextFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFontWeightProperty =
            DependencyProperty.Register("TextFontWeight", typeof(FontWeight), typeof(BindableTextBlock));



        public BindableTextType Type { get;  }

        private readonly bool disableContextMenu = false;

        public BindableTextBlock(BindableTextType type, bool disableContextMenu = false)
        {
            Type = type;
            this.disableContextMenu = disableContextMenu;
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var yadjust = ActualHeight + e.VerticalChange;
            var xadjust = ActualWidth + e.HorizontalChange;
            if ((xadjust >= 15) && (yadjust >= 15))
            {
                Height = yadjust;
                Width = xadjust;
            }
        }

        private void ToggleDesign_Click(object sender, RoutedEventArgs e) => Designing = !Designing;

        private void BindTo_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as MenuItem)?.Tag as string;
            if (!string.IsNullOrEmpty(tag))
            {
                SetBinding(TextProperty, tag);
                Tag = $"BINDTO:{tag}";
            }
        }

        private void SetCustomText_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SetCustomText();

            if (dialog.ShowDialog() == true)
            {
                BindingOperations.ClearBinding(this, TextProperty);
                Text = dialog.Result;
                Tag = "";
            }
        }

        private void Component_Loaded(object sender, RoutedEventArgs e)
        {
            if (Type == BindableTextType.BarCode)
            {
                BindTo_Menu.IsEnabled = false;
                SetCustomText_Menu.IsEnabled = false;
                DESIGN_Textblock.FontFamily = DocumentHelper.BarcodeFont;
                DESIGN_Textblock.FontWeight = FontWeights.Normal;
            }
            else
            {
                DESIGN_Textblock.FontWeight = FontWeights.Bold;
                DESIGN_Textblock.FontFamily = new FontFamily("Times New Roman");
            }

            if (disableContextMenu)
            {
                MasterGrid.ContextMenu.Visibility = Visibility.Collapsed;
                MasterGrid.ContextMenu.IsEnabled = false;
                MasterGrid.ContextMenu = null;
            }
        }
    }

    public enum BindableTextType
    {
        Normal, BarCode
    }
}
