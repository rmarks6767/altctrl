using System.IO.Ports;

namespace glipglop
{
    public class Device : SerialPort
    {
        /// <summary>
        /// The name of the device that is connected
        /// </summary>
        public string Name { get; set; }

        public Device(string name, string portName = "COM3", int baudRate = 112500, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            Name = name;
            ReadTimeout = 100;
        }
    }
}
