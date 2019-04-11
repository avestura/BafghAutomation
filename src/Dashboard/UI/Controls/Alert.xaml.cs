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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dashboard.UI.Controls
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class Alert : UserControl
    {
        public Alert() => InitializeComponent();

        public AlertTypes AlertType
        {
            get => (AlertTypes)GetValue(AlertTypeProperty);
            set => SetValue(AlertTypeProperty, value);
        }

        public static readonly DependencyProperty AlertTypeProperty =
            DependencyProperty.Register("AlertType", typeof(AlertTypes), typeof(Alert), null);

        public bool Waiting
        {
            get => (bool)GetValue(WaitingProperty);
            set => SetValue(WaitingProperty, value);
        }

        public static readonly DependencyProperty WaitingProperty =
            DependencyProperty.Register("Waiting", typeof(bool), typeof(Alert), null);

        public enum AlertTypes
        {
            Success, Failed, Warning, Alternative
        }
    }
}
