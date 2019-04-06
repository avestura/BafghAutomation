using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public partial class Config
    {
        public event EventHandler<ConfigurationSavedEventArgs> ConfigurationSaveEnded;

        protected virtual void OnConfigurationSaveEnded(ConfigurationSavedEventArgs e)
        {
            ConfigurationSaveEnded?.Invoke(this, e);
        }

        public class ConfigurationSavedEventArgs
        {
            public bool IsSuccessful { get; internal set; }
            public Exception Exception { get; internal set; }
            public string Path { get; set; }
        }
    }
}
