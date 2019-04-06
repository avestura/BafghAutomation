using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public partial class Config
    {
        private const string fileName = "App.Config.Xml";

        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//HOMATEC//COMMan//";

        private static readonly string path = directory + fileName;
    }
}
