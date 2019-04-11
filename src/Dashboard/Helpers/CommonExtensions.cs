using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Helpers
{
    public static class CommonExtensions
    {
        public static void Try(this Action action)
        {
            try { action(); } catch { }
        }
    }
}
