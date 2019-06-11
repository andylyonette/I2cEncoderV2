using System;
using System.Text;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public enum I2cEncoderPullUpConfig : byte
    {
        ENABLE = 0x00,
        DISABLE = 0x08
    }
}
