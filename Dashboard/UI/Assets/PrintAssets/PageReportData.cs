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
        private readonly double scaleFactor = App.CurrentApp.AppConfiguration.ScaleFactor;

        public string BarCodeData
        {
            get => barcodeData;
            set
            {
                barcodeData = value;
                OnPropertyChanged("BarCodeData");
            }
        }

        public string BarCodeUI => $"*{barcodeData}*";

        public string Length
        {
            get => length;
            set
            {
                length = value;
                OnPropertyChanged("Length");
            }
        }

        public string Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public string StdNo
        {
            get => stdNo;
            set
            {
                stdNo = value;
                OnPropertyChanged("StdNo");
            }
        }

        public string Proc
        {
            get => proc;
            set
            {
                proc = value;
                OnPropertyChanged("Proc");
            }
        }

        public string Grade
        {
            get => grade;
            set
            {
                grade = value;
                OnPropertyChanged("Grade");
            }
        }

        public string Dia
        {
            get => dia;
            set
            {
                dia = value;
                OnPropertyChanged("Dia");
            }
        }

        public Uri ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public double SceneHeight => 472 * scaleFactor;

        public double SceneWidth => 354 * scaleFactor;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
