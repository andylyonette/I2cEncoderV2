using System;
using System.Text;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public enum I2cEncoderFadeStatus : byte
    {
        NULL = 0x00,
        FADE_R = 0x01,
        FADE_G = 0x02,
        FADE_B = 0x04,
        FADES_GP1 = 0x08,
        FADES_GP2 = 0x10,
        FADES_GP3 = 0x20
    }
}
