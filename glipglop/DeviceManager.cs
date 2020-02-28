using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace glipglop
{
    public class DeviceManager
    {
        /// <summary>
        /// The different components that are being read from
        /// </summary>
        Dictionary<Device, List<Component>> components;
        Dictionary<string, Device> devices;

        /// <summary>
        /// The bool to tell the program to read or not read connections
        /// </summary>
        bool reading;

        public DeviceManager(Dictionary<string, List<string>> devices)
        {
            this.devices = new Dictionary<string, Device>();
            components = CreateDevices(devices);
            reading = true;
            Thread t = new Thread(new ThreadStart(ReadDataAndConnections));
            t.Start();
        }

        public void ReadDataAndConnections() { while (reading) { } }

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

                    // Get the data from the new JObject

                    // TODO: We could probably assume the device is in the dict
                    if (devices.ContainsKey(data.Device))
                    {
                        // Get the comp if it exists
                        Component com = components[devices[data.Device]].Find(comp => comp.Name == data.Port);

                        // Check to make sure we don't have to make this Component
                        if (com == null)
                        {
                            com = new Component(data.Port, data.Device);
                            components[devices[data.Device]].Add(com);
                        }

                        // Check to see if the comp was pressed or released
                        if (data.Type == "Pressed")
                            com.Pressed();
                        else
                            com.Released();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Used to create the devices and the components on those devices
        /// </summary>
        /// <param name="toCreateDevices"></param>
        public Dictionary<Device, List<Component>> CreateDevices(Dictionary<string, List<string>> devs)
        {
            Dictionary<Device, List<Component>> comps = new Dictionary<Device, List<Component>>();
            foreach (string device in devs.Keys)
            {
                string[] ports = SerialPort.GetPortNames();
                Device dev;
                Boolean check = true;
                while (check)
                {
                    foreach (string p in ports)
                    {
                        try
                        {
                            dev = new Device(device, p);
                            dev.DataReceived += new SerialDataReceivedEventHandler(ReadData);
                            dev.Open();
                            devices.Add(device, dev);
                            comps.Add(dev, new List<Component>());
                            foreach (string component in devs[device])
                            {
                                Component comp = new Component(component, device);
                                comps[dev].Add(comp);
                            }
                            Console.WriteLine("Port: " + p);
                            check = false;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e);
                        }
                        if (check == false)
                        {
                            break;
                        }
                    }
                }
            }

            return comps;
        }

        /// <summary>
        /// Used to add delegates for pressed to the given device
        /// </summary>
        /// <param name="pressed">The delegate that will be added to the pressed</param>
        /// <param name="device">The device to add it to</param>
        /// <param name="component">The component to add it to</param>
        public void AddPressed(PressedDel pressed, string device, string component)
        {
            if (devices.ContainsKey(device))
            {
                if (devices.ContainsKey(device))
                {
                    Component comp = components[devices[device]].Find(com => com.Name == component);
                    comp.Pressed += pressed;
                }
            }
        }

        /// <summary>
        /// Used to add delegates for released to the given device
        /// </summary>
        /// <param name="released">The delegate that will be added to the released</param>
        /// <param name="device">The device to add it to</param>
        /// <param name="component">The component to add it to</param>
        public void AddReleased(ReleasedDel released, string device, string component)
        {
            if (devices.ContainsKey(device))
            {
                if (devices.ContainsKey(device))
                {
                    Component comp = components[devices[device]].Find(com => com.Name == component);
                    comp.Released += released;
                }
            }
        }
    }
}
