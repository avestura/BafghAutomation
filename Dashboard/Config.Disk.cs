using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Dashboard
{
    public partial class Config
    {
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
            }
            catch (Exception ex)
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

        public static void InitializeLocalFolder()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
