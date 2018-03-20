using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace COM_Ports_Communication.Models
{
    public class SerialPortCommunication
    {
        private static SerialPort port;
        public static bool StopListening=false;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PortName"></param>
        /// <param name="BoundRate"></param>
        public SerialPortCommunication(string PortName, int BoundRate)
        {
            port = new SerialPort();
            port.PortName = PortName;
            port.BaudRate = BoundRate;
        }

        #endregion

        #region Public Properties

        public bool IsOpen{ get => port.IsOpen; }

        #endregion

        #region Public Methods

        /// <summary>
        /// COM Port Opening
        /// </summary>
        public void OpenConnection()
        {
            port.Open();
        }

        /// <summary>
        /// COM Port Closing
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                port.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string ReadLine(int timeout)
        {
            try
            {
                port.ReadTimeout = timeout;
                return port.ReadLine();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void WriteLine(string data)
        {
            port.WriteLine(data);
        }

        #endregion




        #region Static Methods

        /// <summary>
        /// Get list of Available COM ports
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailablePortNames()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Bound Rates Values
        /// </summary>
        public static List<string> BoundRates = new List<string>() {
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "74880",
            "115200",
            "230400",
            "256000",
            "512000",
            "921600"
        };

        #endregion

    }
}
