using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Dashboard.UI.Controls
{
    public class CustomPrintDocumentViewer : DocumentViewer
    {
        public bool DocumentReversed { get; set; }

        public bool DocumentBackgroundRemoved { get; set; }

        protected override void OnPrintCommand()
        {
            var conf = App.CurrentApp.AppConfiguration;
            // TODO: Remove HardCoded UI Query
            if( (conf.PrintWithRemovedBackground || conf.PrintReversed)
                && Document is FixedDocument f
                && f.Pages.First() is PageContent page1
                && page1.Child is FixedPage fx
                && fx.Children[0] is Image bg)
            {
                if (conf.PrintReversed && !DocumentReversed)
                {
                    fx.LayoutTransform = new RotateTransform(180);
                    DocumentReversed = true;
                }
                if (conf.PrintWithRemovedBackground && !DocumentBackgroundRemoved)
                {
                    bg.Visibility = Visibility.Hidden;
                    DocumentBackgroundRemoved = true;
                }
                base.OnPrintCommand();
            }
            else
            {
                base.OnPrintCommand();
            }

        }
    }
}
