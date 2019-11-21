using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace glipglop
{
    /// <summary>
    /// Events for when a press and a release happen
    /// </summary>
    public delegate void PressedDel();
    public delegate void ReleasedDel();
    public class DeviceManager
    {
        /// <summary>
        ///  Events for when a button is pressed, add something to this like so:
        ///  {Device}.Pressed += new Pressed({Function to do when pressed});
        /// </summary>
        public PressedDel Pressed;

        /// <summary>
        ///  Events for when a button is pressed, add something to this like so:
        ///  {Device}.Released += new Released({Function to do when released});
        /// </summary>
        public ReleasedDel Released;

        List<Device> devices;

        public DeviceManager()
        {
            devices = new List<Device>();
            //Thread t = new Thread(new ThreadStart(ReadDataAndConnections));
            //t.Start();
        }

        public void ReadDataAndConnections()
        {
            foreach (Device d in devices)
            {
                ReadData(d, null);
            }
        }

        public void ReadData(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is Device device)
            {
                try
                {
                    Console.WriteLine(device.ReadLine());
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void CreateConnection(string portName)
        {
            Device device = new Device(portName, 112500, Parity.None, 8, StopBits.One, "Button");
            device.DataReceived += new SerialDataReceivedEventHandler(ReadData);
            device.Open();
            devices.Add(device);
        }

        public void CloseConnection(Device device) => device.Close();
    }
}
