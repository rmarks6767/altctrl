using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace glipglop
{
    public class DeviceManager
    {

        List<SerialPort> devices;

        public DeviceManager()
        {

        }

        public void ReadData(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is Device port)
            {
            }
        }

        public Device CreateConnection(string portName)
        {
            Device device = new Device(portName, 9600, Parity.None, 8, StopBits.One);
            device.DataReceived += new SerialDataReceivedEventHandler(ReadData);
            device.Open();
            return device;
        }

        public void CloseConnection(Device device) => device.Close();

        public void Pressed(Device device)
        {

        }

        public void Released()
        {

        }
    }
}
