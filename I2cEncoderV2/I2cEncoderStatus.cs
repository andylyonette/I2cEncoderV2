using System;
using System.Text;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public enum I2cEncoderStatus : byte
    {
        NULL = 0x00,
        PUSHR = 0x01,
        PUSHP = 0x02,
        PUSHD = 0x04,
        RINC = 0x08,
        RDEC = 0x10,
        RMAX = 0x20,
        RMIN = 0x40,
        INT_2 = 0x80
    }
}
