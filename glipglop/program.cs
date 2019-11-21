using System;
using System.Collections.Generic;
using System.Text;

namespace glipglop
{
    class Program
    {
        static void Main(string[] args)
        {
            DeviceManager manager = new DeviceManager();

            manager.Pressed += Pressed;
            manager.Released += Released;
        }

        static void Pressed()
        {
            Console.WriteLine("Pressed");
        }

        static void Released()
        {
            Console.WriteLine("Released");
        }
    }
}
