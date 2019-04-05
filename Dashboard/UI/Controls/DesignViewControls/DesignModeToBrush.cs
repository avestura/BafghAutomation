using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace Dashboard.UI.Controls.DesignViewControls
{
    [MarkupExtensionReturnType(typeof(SolidColorBrush))]
    public class DesignModeToBrush : MarkupExtension
    {
        [ConstructorArgument("designMode")]
        public bool DesignMode { get; set; }

        public DesignModeToBrush() => DesignMode = false;

        public DesignModeToBrush(bool designMode)
        {
            DesignMode = designMode;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return DesignMode ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
        }
    }
}
