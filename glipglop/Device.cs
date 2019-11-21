using System.IO.Ports;

namespace glipglop
{
    
    public class Device : SerialPort
    {
        // Just a local variable for taking in input
        private bool pressed;

        /// <summary>
        /// The name given to a given Device
        /// </summary>
        public string name;

        /// <summary>
        /// Checks to see if the current state of the device is pressed
        /// </summary>
        /// <returns>Returns a bool of if it is pressed or not; true if it is pressed</returns>
        public bool isPressed() => pressed;

        /// <summary>
        /// Checks to see if the current state of the device is released
        /// </summary>
        /// <returns>Returns a bool of if it is released or not; true if it is released</returns>
        public bool isReleased() => !pressed;
        
        /// <summary>
        /// Creates a new version of the device
        /// </summary>
        public Device(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, string name)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            this.name = name;
            pressed = false;
        }
    }
}