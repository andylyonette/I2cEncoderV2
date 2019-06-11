using System;
using System.Text;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public enum I2cEncoderAddress : byte
    {
        REG_GCONF = 0x00,
        REG_GP1CONF = 0x01,
        REG_GP2CONF = 0x02,
        REG_GP3CONF = 0x03,
        REG_INTCONF = 0x04,
        REG_ESTATUS = 0x05,
        REG_I2STATUS = 0x06,
        REG_FSTATUS = 0x07,
        REG_CVALB4 = 0x08,
        REG_CVALB3 = 0x09,
        REG_CVALB2 = 0x0A,
        REG_CVALB1 = 0x0B,
        REG_CMAXB4 = 0x0C,
        REG_CMAXB3 = 0x0D,
        REG_CMAXB2 = 0x0E,
        REG_CMAXB1 = 0x0F,
        REG_CMINB4 = 0x10,
        REG_CMINB3 = 0x11,
        REG_CMINB2 = 0x12,
        REG_CMINB1 = 0x13,
        REG_ISTEPB4 = 0x14,
        REG_ISTEPB3 = 0x15,
        REG_ISTEPB2 = 0x16,
        REG_ISTEPB1 = 0x17,
        REG_RLED = 0x18,
        REG_GLED = 0x19,
        REG_BLED = 0x1A,
        REG_GP1REG = 0x1B,
        REG_GP2REG = 0x1C,
        REG_GP3REG = 0x1D,
        REG_ANTBOUNC = 0x1E,
        REG_DPPERIOD = 0x1F,
        REG_FADERGB = 0x20,
        REG_FADEGP = 0x21,
        REG_EEPROMS = 0x80
    }
}
