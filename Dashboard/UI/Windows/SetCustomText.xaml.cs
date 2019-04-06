using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Dashboard.UI.Windows
{
    /// <summary>
    /// Interaction logic for SetCustomText.xaml
    /// </summary>
    public partial class SetCustomText : Window
    {
        public string Result { get; private set; }

        public SetCustomText() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = input.Text;
            DialogResult = true;
            Close();
        }
    }
}
