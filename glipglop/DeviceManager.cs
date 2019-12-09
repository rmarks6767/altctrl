using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace glipglop
{
    public class DeviceManager
    {
        bool reading;

        // Name of device and device
        Dictionary<string, List<Component>> devices;

        public DeviceManager(Dictionary<string, List<string>> devicesToCreate)
        {
            devices = new Dictionary<string, List<Component>>();
            CreateDevices(devicesToCreate);
            reading = true;

            // Break off a thread to run the reading data connections
            Thread readThread = new Thread(new ThreadStart(ReadDataAndConnections));
            readThread.Start();
        }

        public void ReadDataAndConnections()
        {
            while (reading)
            {
                foreach (List<Component> list in devices.Values)
                {
                    foreach (Component comp in list)
                    {
                        Console.WriteLine($"Reading: {comp.Name}");
                        ReadData(comp, null);
                    }
                }
            }
        }

        public void ReadData(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is Component component)
            {
                try
                {
                    // Get the data
                    string data = component.ReadLine();

                    // Convert to a json object
                    JObject j = JObject.FromObject(data);

                    // Get the data from the new JObject
                    string device = j.Value<string>("device");  // The device name
                    string port = j.Value<string>("port");     // The port that is in use
                    string type = j.Value<string>("type");          // Pressed or released

                    // TODO: We could probably assume the device is in the dict
                    if (devices.ContainsKey(device))
                    {
                        Component com = devices[device].Find(comp => comp.Name == port);

                        // Check to make sure we don't have to make this Component
                        if (com == null)
                        {
                            com = new Component(port, device, "COM3", 112500, Parity.None, 8, StopBits.One);
                            devices[device].Add(com);
                        }

                        // Check to see if the comp was pressed or released
                        if (type == "Pressed")
                        {
                            com.Pressed();
                        }
                        else
                        {
                            com.Released();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("An Error Occured while reading input!");
                }
            }
        }

        public void CreateDevices(Dictionary<string, List<string>> toCreateDevices)
        {
            foreach (string device in toCreateDevices.Keys)
            {
                devices.Add(device, new List<Component>());

                foreach(string component in toCreateDevices[device])
                {
                    devices[device].Add(new Component(component, device, "COM3", 112500, Parity.None, 8, StopBits.One));
                }
            }
        }

        public void OpenConnection(Component component) => component.Open();
        public void CloseConnection(Component component) => component.Close();
    }
}
