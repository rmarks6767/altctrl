using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace glipglop
{
    public class Device : SerialPort
    {
        /// <summary>
        /// The name of the device that is connected
        /// </summary>
        public string Name { get { return name; } }
        private readonly string name;

        public Device(string name, string portName = "COM3", int baudRate = 112500, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            this.name = name;
            ReadTimeout = 100;
        }
    }
}
