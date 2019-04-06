using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using Dashboard.Models;

namespace Dashboard
{
    public partial class Config
    {
        #region Settings :: COM
        public string COMPortName { get; set; } = "COM1";
        public int BaudRate { get; set; } = 4800;
        public Parity ParityType { get; set; }
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;
        #endregion

        #region Settings :: Machine Settings
        public string AliasName { get; set; } = Environment.MachineName;
        #endregion

        #region Settings :: Detector
        public ushort EndTrimLength { get; set; } = 0;
        public uint StabilityTime { get; set; } = 2000;
        public uint RepetitiveDelayTime { get; set; } = 5000;
        #endregion

        #region Settings :: Third-Party Applications
        public string LastWeightFileAddress { get; set; } = "";
        public string PackDetailsFileAddress { get; set; } = "";
        #endregion

        #region Settings :: Printing
        public string PrintStdNo { get; set; } = "3132";
        public string PrintProProcedure { get; set; } = "TERMEX";
        public string PrintBackgroundImageAddress { get; set; } = "";
        public bool PrintReversed { get; set; } = false;
        public bool PrintWithRemovedBackground { get; set; }

        public double ScaleFactor { get; set; } = 1;
        #endregion

        #region Data :: Saved Design
        public DesignSaveModel DesignModel { get; set; } = new DesignSaveModel
        {
            ImageBackgroundSource = null,
            Textboxes = new List<BindableTextboxSaveModel>()
        };
        #endregion
    }
}
