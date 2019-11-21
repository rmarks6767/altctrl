using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace glipglop
{
    public class DeviceManager
    {
        List<Device> devices;

        public DeviceManager()
        {
            devices = new List<Device>();
            Thread t = new Thread(new ThreadStart(ReadDataAndConnections));
            t.Start();
        }

        public void ReadDataAndConnections()
        {
            while(true)
            {
                // See if there are any new connections

                // Read Data
                foreach(Device d in devices)
                {
                    ReadData(d, null);
                }
            }
        }

        public void ReadData(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is Device device)
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
    }
}
