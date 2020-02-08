/// This is the testing framework for glipglop, to make sure it works
using System;
using System.Collections.Generic;
using glipglop;

namespace tiptop
{
    class Program
    {
        static void Main()
        {
            Dictionary<string, List<string>> devices = new Dictionary<string, List<string>>
            {
                {
                    "D0",
                    new List<string>()
                    {
                        "P0",
                        "P1",
                        "P2",
                        "P3",
                    }
                }
            };

            DeviceManager manager = new DeviceManager(devices);

            manager.AddPressed(new PressedDel(Pressed), "D0", "P0");
            manager.AddReleased(new ReleasedDel(Released), "D0", "P0");
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
