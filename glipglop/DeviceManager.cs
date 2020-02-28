using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace glipglop
{
    public class DeviceManager
    {
        Thread readingThread;

        /// <summary>
        /// The different components that are being read from
        /// </summary>
        Dictionary<Device, List<Component>> components;
        Dictionary<string, Device> devices;
        Dictionary<string, PressedDel> pressedDels;
        Dictionary<string, ReleasedDel> releasedDels;
        List<string> openPorts;

        /// <summary>
        /// The bool to tell the program to read or not read connections
        /// </summary>
        public bool Reading { get; set; }

        public DeviceManager()
        {
            devices = new Dictionary<string, Device>();
            components = new Dictionary<Device, List<Component>>();
            pressedDels = new Dictionary<string, PressedDel>();
            releasedDels = new Dictionary<string, ReleasedDel>();
            openPorts = new List<string>();
            Reading = true;
            readingThread = new Thread(new ThreadStart(ReadConnections));
            readingThread.Start();
        }



        /// <summary>
        /// Used to (re)start the thread to read devices
        /// </summary>
        public void StartReading()
        {
            Reading = true;
            readingThread = new Thread(new ThreadStart(ReadConnections));
            readingThread.Start();
        }

        /// <summary>
        /// Used to pause the reading of the deviceManager
        /// </summary>
        public void StopReading(bool keepDevices)
        {
            Reading = false;
            if (!keepDevices)
            {
                foreach (Device dev in devices.Values)
                    dev.Close();

                devices = new Dictionary<string, Device>();
                components = new Dictionary<Device, List<Component>>();
            }
            readingThread.Join();
        }

        /// <summary>
        /// Used to read all of the incoming serial data
        /// </summary>
        /// <param name="sender">The device that is attached</param>
        /// <param name="e">Not used tbh but it's gotta be there</param>
        public void ReadData(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is Device dev)
            {
                try
                {
                    string serialInput = dev.ReadLine();
                    Console.WriteLine(serialInput);

                    // Convert to a json object
                    Data data = JObject.Parse(serialInput).ToObject<Data>();

                    // make sure we have created the given device and now rename it if we don't already have it
                    if (!devices.ContainsKey(data.Device) && devices.ContainsKey(dev.PortName))
                    {
                        devices.Remove(dev.PortName);
                        devices.Add(data.Device, dev);
                        dev.Name = data.Device;
                    }

                    // Get the comp if it exists
                    Component com = components[devices[data.Device]].Find(comp => comp.Name == data.Port);

                    // Check to make sure we don't have to make this Component
                    if (com == null)
                    {
                        com = new Component(data.Port, data.Device);
                        components[devices[data.Device]].Add(com);
                    }

                    // Check to see if the comp was pressed or released and call the associated delegates
                    if (data.Type == "Pressed")
                    {
                        pressedDels[$"{data.Device},{data.Port}"].Invoke();
                        com.Pressed = true;
                    }
                    else
                    {
                        releasedDels[$"{data.Device},{data.Port}"].Invoke();
                        com.Pressed = false;
                    }
                }
                catch (Exception ex)
                {
                    // This most likely means that this port is no longer plugged in
                    // so we shall remove it from everything
                    devices.Remove(dev.Name);
                    components.Remove(dev);
                    openPorts.Remove(dev.PortName);
                    dev.Close();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void ReadConnections()
        {
            while (Reading)
            {
                List<string> ports = SerialPort.GetPortNames().ToList();

                foreach (string port in openPorts)
                    ports.Remove(port);

                foreach (string port in ports)
                    Console.WriteLine(port);

                foreach (string p in ports)
                {
                    try
                    {
                        Device device = new Device("unknown", p);
                        device.DataReceived += new SerialDataReceivedEventHandler(ReadData);
                        device.Open();
                        devices.Add(p, device);
                        components.Add(device, new List<Component>());
                        openPorts.Add(p);
                    }
                    catch (Exception e) { continue; }
                }
            }
        }

        /// <summary>
        /// Used to add delegates for pressed to the given device
        /// </summary>
        /// <param name="pressed">The delegate that will be added to the pressed</param>
        /// <param name="device">The device to add it to</param>
        /// <param name="component">The component to add it to</param>
        public void AddPressed(PressedDel pressed, string device, string component)
        {
            string key = string.Format($"{device},{component}");
            if (pressedDels.ContainsKey(key))
                pressedDels[key] += pressed;
            else
                pressedDels.Add(key, pressed);
        }

        /// <summary>
        /// Used to add delegates for released to the given device
        /// </summary>
        /// <param name="released">The delegate that will be added to the released</param>
        /// <param name="device">The device to add it to</param>
        /// <param name="component">The component to add it to</param>
        public void AddReleased(ReleasedDel released, string device, string component)
        {
            string key = string.Format($"{device},{component}");
            if (releasedDels.ContainsKey(key))
                releasedDels[key] += released;
            else 
                releasedDels.Add(key, released);
        }
    }
}
