using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.IO.Ports;
using Dashboard.Models;
using System.Windows.Media.Imaging;

namespace Dashboard
{
    public class Config
    {
        public event EventHandler<ConfigurationSavedEventArgs> ConfigurationSaveEnded;

        protected virtual void OnConfigurationSaveEnded(ConfigurationSavedEventArgs e)
        {
            ConfigurationSaveEnded?.Invoke(this, e);
        }

        public string COMPortName { get; set; } = "COM1";
        public int BaudRate { get; set; } = 4800;
        public Parity ParityType { get; set; }
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;

        public string AliasName { get; set; } = Environment.MachineName;
        public uint StabilityTime { get; set; } = 2000;
        public uint RepetitiveDelayTime { get; set; } = 5000;

        public string LastWeightFileAddress { get; set; } = "";
        public string PackDetailsFileAddress { get; set; } = "";

        public string PrintStdNo { get; set; } = "3132";
        public string PrintProProcedure { get; set; } = "TERMEX";
        public string PrintBackgroundImageAddress { get; set; } = "";

        public ushort EndTrimLength { get; set; } = 0;

        public double ScaleFactor { get; set; } = 1;

        public bool PrintReversed { get; set; } = false;
        public bool PrintWithRemovedBackground { get; set; }

        public DesignSaveModel DesignModel { get; set; } = new DesignSaveModel
        {
            ImageBackgroundSource = null,
            Textboxes = new List<BindableTextboxSaveModel>()
        };

        #region Config :: Constants
        private const string fileName = "App.Config.Xml";
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//HOMATEC//COMMan//";
        private static readonly string path = directory + fileName;
        #endregion

        public static void InitializeLocalFolder()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void SaveSettingsToFile()
        {
            try
            {
                XmlSerializer xsSubmit = new XmlSerializer(typeof(Config));
                var xml = "";

                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, this);
                        xml = sww.ToString(); // Your XML
                    }
                }

                File.WriteAllText(path, xml);

                OnConfigurationSaveEnded(new ConfigurationSavedEventArgs() { IsSuccessful = true, Path = path });
            } catch (Exception ex)
            {
                MessageBox.Show($"Could not write config files. Changes will not take effect in next restart.\nError Details: {ex.Message}\nInner Exception: {ex.InnerException.Message}");
                OnConfigurationSaveEnded(new ConfigurationSavedEventArgs() { IsSuccessful = false, Exception = ex, Path = path });
            }
        }

        public static void LoadSettingsFromFile()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    var stream = new StreamReader(fileStream, Encoding.UTF8);
                    App.CurrentApp.AppConfiguration = (Config)serializer.Deserialize(stream);
                }
            }
            catch
            {
                App.CurrentApp.AppConfiguration = new Config();
                App.CurrentApp.AppConfiguration.SaveSettingsToFile();
            }
        }

        public class ConfigurationSavedEventArgs
        {
            public bool IsSuccessful { get; internal set; }
            public Exception Exception { get; internal set; }
            public string Path { get; set; }
        }
    }
}
