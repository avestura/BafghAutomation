using Dashboard.UI.Controls.DesignViewControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Dashboard.Models
{
    public class BindableTextboxSaveModel
    {
        public double CanvasTop { get; set; }
        public double CanvasLeft { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public bool IsBound { get; set; }
        public string BindingTag { get; set; }
        public string Text { get; set; }
        public double RotationDegree { get; set; }
        public FontWeight FontWeight { get; set; }
        public BindableTextType Type { get; set; }
    }

    public class DesignSaveModel
    {
        public string ImageBackgroundSource { get; set; }

        public List<BindableTextboxSaveModel> Textboxes { get; set; }
    }
}
