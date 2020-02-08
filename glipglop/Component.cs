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

        // The hidden bool that signifies if it is pressed or not
        private bool pressed;

        /// <summary>
        /// The delegate responsible for when a thing is pressed
        /// </summary>
        public PressedDel Pressed;

        /// <summary>
        /// The delegate responsible for when something is released
        /// </summary>
        public ReleasedDel Released;

        public Component(string name, string deviceName)
        {
            DeviceName = deviceName;
            Name = name;
            pressed = false;
            Pressed += new PressedDel(ChangeToPressed);
            Released += new ReleasedDel(ChangeToReleased);
        }

        /// <summary>
        /// Used to see if a given component is pressed
        /// </summary>
        /// <returns>Returns a bool for if it is pressed or not</returns>
        public bool isPressed() => pressed;

        /// <summary>
        /// Used to see if a given component is released
        /// </summary>
        /// <returns>Returns a bool for if it is released or not</returns>
        public bool isReleased() => !pressed;

        /// <summary>
        /// Used to change the status of the pressed local variable
        /// </summary>
        private void ChangeToPressed() => pressed = true;

        /// <summary>
        /// Used to change the status of the pressed local variable
        /// </summary>
        private void ChangeToReleased() => pressed = false;
    }
}