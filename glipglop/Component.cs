namespace glipglop
{
    public class Component
    {
        /// <summary>
        /// The name of the device that the component belongs to
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The name of the component on the device
        /// </summary>
        public string Name { get; set; }

        // The bool that signifies if it is pressed or not
        public bool Pressed { get; set; }

        public Component(string name, string deviceName)
        {
            DeviceName = deviceName;
            Name = name;
            Pressed = false;
        }

        /// <summary>
        /// Used to see if a given component is pressed
        /// </summary>
        /// <returns>Returns a bool for if it is pressed or not</returns>
        public bool isPressed() => Pressed;

        /// <summary>
        /// Used to see if a given component is released
        /// </summary>
        /// <returns>Returns a bool for if it is released or not</returns>
        public bool isReleased() => !Pressed;
    }
}