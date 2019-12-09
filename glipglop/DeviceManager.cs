using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace glipglop
{
    public class DeviceManager
    {
        // Used maybe for changing the state of if it should stop being read
        bool reading;
        Thread readThread;

        // Name of device and list of components on that device
        Dictionary<string, List<Component>> devices;

        public DeviceManager(Dictionary<string, List<string>> devicesToCreate)
        {
            devices = new Dictionary<string, List<Component>>();
            CreateDevices(devicesToCreate);
            reading = true;

            // Break off a thread to run the reading data connections
            readThread = new Thread(new ThreadStart(ReadDataAndConnections));
            readThread.Start();
        }

        /// <summary>
        /// Used to pause the reading of serialports
        /// </summary>
        public void PauseReading() => reading = false;

        /// <summary>
        /// Used to stop the reading of the serial ports.  Calling this will join the thread making the device manager technically useless
        /// </summary>
        public void StopReading() => readThread.Join();

        /// <summary>
        ///  Opens a new connection with one of the given components
        /// </summary>
        /// <param name="component">The component to open</param>
        public void OpenConnection(Component component) => component.Open();

        /// <summary>
        /// Closes an existing connection with one of the components
        /// </summary>
        /// <param name="component">The component to close</param>
        public void CloseConnection(Component component) => component.Close();

        /// <summary>
        /// Used on a thread to constantly read data from the connections that we have 
        /// </summary>
        public void ReadDataAndConnections()
        {
            while (reading)
            {
                foreach (List<Component> list in devices.Values)
                {
                    foreach (Component comp in list)
                    {
                        Console.WriteLine($"Reading: {comp.Name}");
                        ReadData(comp);
                    }
                }
            }
        }

        /// <summary>
        /// Read in the data through the associated Serial Port
        /// </summary>
        /// <param name="component">The component to read input from</param>
        public void ReadData(Component component)
        {
            try
            {
                // Get the data
                string data = component.ReadLine();

                // Convert to a json object
                JObject j = JObject.FromObject(data);

                // Get the data from the new JObject
                string device = j.Value<string>("device"); // The device name
                string port = j.Value<string>("port");     // The port that is in use
                string type = j.Value<string>("type");     // Pressed or released

                // TODO: We could probably assume the device is in the dict
                if (devices.ContainsKey(device))
                {
                    // Get the comp if it exists
                    Component com = devices[device].Find(comp => comp.Name == port);

                    // Check to make sure we don't have to make this Component
                    if (com == null)
                    {
                        com = new Component(port, device);
                        devices[device].Add(com);
                    }

                    // Check to see if the comp was pressed or released
                    if (type == "Pressed")
                        com.Pressed();
                    else
                        com.Released();
                }
            }
            catch(Exception exp)
            {
                Console.WriteLine($"{exp.Message}");
            }
        }

        /// <summary>
        /// Used to create the devices and the components on those devices
        /// </summary>
        /// <param name="toCreateDevices"></param>
        public void CreateDevices(Dictionary<string, List<string>> toCreateDevices)
        {
            foreach (string device in toCreateDevices.Keys)
            {
                devices.Add(device, new List<Component>());

                foreach(string component in toCreateDevices[device])
                {
                    devices[device].Add(new Component(component, device));
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
            if (devices.ContainsKey(device))
            {
                Component com = devices[device].Find(comp => comp.Name == component);

                if (com != null)
                {
                    com.Pressed += pressed;
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
                Component com = devices[device].Find(comp => comp.Name == component);

                if (com != null)
                {
                    com.Released += released;
                }
            }
        }
    }
}
