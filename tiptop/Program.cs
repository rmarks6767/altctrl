/// This is the testing framework for glipglop, to make sure it works
using System;
using glipglop;

namespace tiptop
{
    class Program
    {
        static void Main()
        {
            DeviceManager manager = new DeviceManager();

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
