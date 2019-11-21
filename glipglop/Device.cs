using System.IO.Ports;

namespace glipglop
{
    /// <summary>
    /// Events for when a press and a release happen
    /// </summary>
    public delegate void PressedDel();
    public delegate void ReleasedDel();
    public class Device : SerialPort
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

        // Just a local variable for taking in input
        private bool pressed;
        
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
        public Device(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            pressed = false;
        }
    }
}