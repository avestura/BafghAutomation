using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Net.Http;

// IMPORTANT ============================
// TODO: Change those threads with Async/Await pattern (Legacy Code)
// IMPORTANT ============================
namespace Dashboard.IO
{
    /**
     * <summary>Port manager that uses a <see cref="SerialPort"></see> as GlobalPort and four predefined threads.</summary>
     **/
    public static class PortManager
    {
        /// <summary>
        /// Global Serial Port of the Application
        /// </summary>
        public static SerialPort GlobalPort { get; set; } = new SerialPort();

        /// <summary>
        /// Thread that used to reads data from serial port
        /// </summary>
        public static Thread ReaderThread { get; set; }

        /// <summary>
        /// Thread that used for parsing Readed data from <see cref="ReaderThread"/> to an integer value
        /// </summary>
        public static Thread ParserThread { get; set; }

        /// <summary>
        /// Thread that is used for sending elements to network
        /// </summary>
        public static Thread ElementsSender { get; set; }

        /// <summary>
        /// Thread that is used for creating weight elements
        /// </summary>
        public static Thread ItemCreatorThread { get; set; }
    }
}
