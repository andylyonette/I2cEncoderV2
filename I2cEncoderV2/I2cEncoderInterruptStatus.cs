using System;
using System.Text;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public enum I2cEncoderInterruptStatus : byte
    {
        NULL = 0x00,
        GP1_POS = 0x01,
        GP1_NEG = 0x02,
        GP2_POS = 0x04,
        GP2_NEG = 0x08,
        GP3_POS = 0x10,
        GP3_NEG = 0x20,
        FADE_INT = 0x40
    }
}
