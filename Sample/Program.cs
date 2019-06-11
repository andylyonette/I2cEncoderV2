using nanoframework.Drivers.I2cEncoderV2;
using nanoFramework.Hardware.Esp32;
using System;
using System.Threading;

namespace Sample
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("nanoFramework I2cEncoderV2 Sample!");

            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            Console.WriteLine("Bus: I2C1, Address: 0x11, I2cBusSpeed: FastMode");
            var encoder = new I2cEncoder("I2C1", 0x11, true);
            encoder.Initialise(I2cEncoderDataType.INT, I2cEncoderWrapConfig.DISABLE, I2cEncoderDirection.RIGHT, I2cEncoderPullUpConfig.ENABLE, I2cEncoderRMOD.X1, I2cEncoderType.STD, I2cEncoderEEPROM.BANK1);

            Console.WriteLine("Counter Max: 100");
            encoder.WriteMax(100);
            Console.WriteLine("Counter Min: 0");
            encoder.WriteMin(0);
            Console.WriteLine("Counter Step: 1");
            Console.WriteLine("Counter: 0");
            encoder.WriteStep(1);


            encoder.EnableEvents(23, true, true, false, true, true, true, true, true);
            encoder.StatusUpdate += OnStatusUpdate;
            Thread.Sleep(Timeout.Infinite);
        }

        static void OnStatusUpdate(object sender, I2cEncoderEventArgs e)
        {
            if (e.ButtonDoublePressed) { Console.WriteLine("Button double-pressed"); }
            if (e.ButtonPressed) { Console.WriteLine("Button pressed"); }
            if (e.ButtonReleased) { Console.WriteLine("Button released"); }
            if (e.CounterDecremented) { Console.WriteLine($"Counter decremented, {e.Counter}"); }
            if (e.CounterIncremented) { Console.WriteLine($"Counter incremented, {e.Counter}"); }
            if (e.CounterMax) { Console.WriteLine($"Counter at max, {e.Counter}"); }
            if (e.CounterMin) { Console.WriteLine($"Counter at min, {e.Counter}"); }
        }

    }
}
