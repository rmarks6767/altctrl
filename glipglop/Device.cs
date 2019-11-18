using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Threading;

namespace glipglop
{
    public class Device : SerialPort
    {
        private bool pressed;
        public bool isPressed() => pressed;
        public bool isReleased() => !pressed;

        public Device(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            pressed = false;
        }
    }
}