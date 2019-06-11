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

        public bool GP1Rise { get; set; }
        public bool GP1Fall{ get; set; }
        public bool GP2Rise { get; set; }
        public bool GP2Fall { get; set; }
        public bool GP3Rise { get; set; }
        public bool GP3Fall { get; set; }
        public bool Fade { get; set; }


        public int Counter { get; set; }
        public float CounterFloat { get; set; }

        public I2cEncoderFadeStatus FadeStatus { get; set; }
    }
}
