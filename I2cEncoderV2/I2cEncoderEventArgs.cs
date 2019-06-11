using nanoFramework.Runtime.Events;
using System;
using System.Text;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public class I2cEncoderEventArgs : EventArgs
    {
        public bool ButtonDoublePressed { get; set; }
        public bool ButtonPressed { get; set; }
        public bool ButtonReleased { get; set; }
        public bool CounterDecremented { get; set; }
        public bool CounterIncremented { get; set; }
        public bool CounterMax { get; set; }
        public bool CounterMin { get; set; }

        public int Counter { get; set; }
        public float CounterFloat { get; set; }
    }
}
