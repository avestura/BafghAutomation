using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class PageReportData : INotifyPropertyChanged
    {
        private string length;
        private string weight;
        private string stdNo;
        private string proc;
        private string grade;
        private string dia;
        private string barcodeData;
        private Uri imageSource;
        private readonly double scaleFactor = App.GetApp().AppConfiguration.ScaleFactor;

        public string BarCodeData
        {
            get { return barcodeData; }
            set
            {
                barcodeData = value;
                OnPropertyChanged("BarCodeData");
            }
        }

        public string BarCodeUI
        {
            get { return $"*{barcodeData}*"; }
        }

        public string Length
        {
            get { return length; }
            set
            {
                length = value;
                OnPropertyChanged("Length");
            }
        }

        public string Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public string StdNo
        {
            get { return stdNo; }
            set
            {
                stdNo = value;
                OnPropertyChanged("StdNo");
            }
        }

        public string Proc
        {
            get { return proc; }
            set
            {
                proc = value;
                OnPropertyChanged("Proc");
            }
        }

        public string Grade
        {
            get { return grade; }
            set
            {
                grade = value;
                OnPropertyChanged("Grade");
            }
        }

        public string Dia
        {
            get { return dia; }
            set
            {
                dia = value;
                OnPropertyChanged("Dia");
            }
        }

        public Uri ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public double SceneHeight
        {
            get
            {
                return 472 * scaleFactor;
            }
        }

        public double SceneWidth
        {
            get
            {
                return 354 * scaleFactor;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
