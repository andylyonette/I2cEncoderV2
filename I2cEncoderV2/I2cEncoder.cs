using System;
using System.Text;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace nanoframework.Drivers.I2cEncoderV2
{
    public class I2cEncoder
    {
        private I2cDevice _i2cEncoder;
        private GpioController _gpioController;
        private GpioPin _intPin;
        private int _gconf = 0;
        private I2cEncoderDataType _dataType;
        private byte _stat;
        private byte _stat2;

        public bool EventsEnabled {get; private set;}


        #region Constructors
        public I2cEncoder(string i2cBus, byte address, bool i2cBusSpeedFast = false)
        {
            if (i2cBusSpeedFast)
            {
                _i2cEncoder = I2cDevice.FromId(i2cBus, new I2cConnectionSettings(address)
                {
                    BusSpeed = I2cBusSpeed.FastMode,
                    SharingMode = I2cSharingMode.Shared
                });

            }
            else
            {
                _i2cEncoder = I2cDevice.FromId(i2cBus, new I2cConnectionSettings(address)
                {
                    BusSpeed = I2cBusSpeed.StandardMode,
                    SharingMode = I2cSharingMode.Shared
                });
            }
            EventsEnabled = false;
        }
        #endregion Constructors


        #region Events
        internal void OnValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                _stat = (byte)Read8(I2cEncoderAddress.REG_ESTATUS);
                _stat2 = 0;

                var args = new I2cEncoderEventArgs();

                if ((_stat & (byte)I2cEncoderStatus.PUSHP) != 0)
                {
                    args.ButtonPressed = true;
                }
                if ((_stat & (byte)I2cEncoderStatus.PUSHR) != 0)
                {
                    args.ButtonReleased = true;
                }
                if ((_stat & (byte)I2cEncoderStatus.PUSHD) != 0)
                {
                    args.ButtonDoublePressed = true;
                }
                if ((_stat & (byte)I2cEncoderStatus.RDEC) != 0)
                {
                    args.CounterDecremented = true;
                    if (_dataType == I2cEncoderDataType.INT)
                    {
                        args.Counter = ReadCounter();
                    }
                    else
                    {
                        args.CounterFloat = ReadCounterFloat();
                    }
                }
                if ((_stat & (byte)I2cEncoderStatus.RINC) != 0)
                {
                    args.CounterIncremented = true;
                    if (_dataType == I2cEncoderDataType.INT)
                    {
                        args.Counter = ReadCounter();
                    }
                    else
                    {
                        args.CounterFloat = ReadCounterFloat();
                    }
                }
                if ((_stat & (byte)I2cEncoderStatus.RMAX) != 0)
                {
                    args.CounterMax = true;
                    if (_dataType == I2cEncoderDataType.INT)
                    {
                        args.Counter = ReadCounter();
                    }
                    else
                    {
                        args.CounterFloat = ReadCounterFloat();
                    }
                }
                if ((_stat & (byte)I2cEncoderStatus.RMIN) != 0)
                {
                    args.CounterMin = true;
                    if (_dataType == I2cEncoderDataType.INT)
                    {
                        args.Counter = ReadCounter();
                    }
                    else
                    {
                        args.CounterFloat = ReadCounterFloat();
                    }
                }
                if ((_stat & (byte)I2cEncoderStatus.INT_2) != 0)
                {
                    // Imprement INT
                }
                OnStatusUpdate(args);
            }
        }


        public event StatusUpdateEventHandler StatusUpdate;

        protected virtual void OnStatusUpdate(I2cEncoderEventArgs e)
        {
            StatusUpdateEventHandler handler = StatusUpdate;
            if (handler != null) { handler(this, e); }
        }

        public delegate void StatusUpdateEventHandler(object sender, I2cEncoderEventArgs e);
        #endregion Events



        #region Public Methods
        public void Initialise(I2cEncoderDataType dataType, I2cEncoderWrapConfig wrapConfig, I2cEncoderDirection direction, I2cEncoderPullUpConfig pullUpConfig, I2cEncoderRMOD rmodConfig, I2cEncoderType encoderType, I2cEncoderEEPROM eepromBank)
        {
            Reset();
            Thread.Sleep(5);

            var _dataType2 = (byte)dataType;
            var _wrapConfig = (byte)wrapConfig;
            var _direction = (byte)direction;
            var _pullUpConfig = (byte)pullUpConfig;
            var _rmod = (byte)rmodConfig;
            var _encoderType = (byte)encoderType;
            var _eEPromConfig = (byte)eepromBank;
            int value = _dataType2 | _wrapConfig | _direction | _pullUpConfig | _rmod | _encoderType | _eEPromConfig;
            var result = Write8(I2cEncoderAddress.REG_GCONF, value);
            _gconf = value;
            _dataType = dataType;
        }

        public void EnableEvents(int interruptPinNumber, bool buttonPress, bool buttonRelease, bool buttonDoublePress, bool counterDecrement, bool counterIncrement, bool counterMax, bool counterMin, bool int2)
        {
            byte value = 0x00;

            if (buttonPress) { value |= (byte)I2cEncoderStatus.PUSHP; }
            if (buttonRelease) { value |= (byte)I2cEncoderStatus.PUSHR; }
            if (buttonDoublePress) { value |= (byte)I2cEncoderStatus.PUSHD; }
            if (counterDecrement) { value |= (byte)I2cEncoderStatus.RDEC; }
            if (counterIncrement) { value |= (byte)I2cEncoderStatus.RINC; }
            if (counterMax) { value |= (byte)I2cEncoderStatus.RMAX; }
            if (counterMin) { value |= (byte)I2cEncoderStatus.RMIN; }
            if (int2) { value |= (byte)I2cEncoderStatus.INT_2; }

            var result = Write8(I2cEncoderAddress.REG_INTCONF, value);

            if (!EventsEnabled)
            {
                _gpioController = GpioController.GetDefault();
                _intPin = _gpioController.OpenPin(interruptPinNumber);
                _intPin.SetDriveMode(GpioPinDriveMode.Input);
                _intPin.ValueChanged += OnValueChanged;

                EventsEnabled = true;
            }
        }
        public void Reset()
        {
            Write8(I2cEncoderAddress.REG_GCONF, 0x80);
        }


        #region Public Read Methods
        public int ReadGP1Conf() { return Read8(I2cEncoderAddress.REG_GP1CONF); }
        public int ReadGP2Conf() { return Read8(I2cEncoderAddress.REG_GP2CONF); }
        public int ReadGP3Conf() { return Read8(I2cEncoderAddress.REG_GP3CONF); }
        public int ReadInterruptConfig() { return Read8(I2cEncoderAddress.REG_INTCONF); }
        public int ReadLEDR() { return Read8(I2cEncoderAddress.REG_RLED); }
        public int ReadLEDG() { return Read8(I2cEncoderAddress.REG_GLED); }
        public int ReadLEDB() { return Read8(I2cEncoderAddress.REG_BLED); }
        public int ReadCounter() { return Read32(I2cEncoderAddress.REG_CVALB4); }
        public float ReadCounterFloat() { return ReadFloat(I2cEncoderAddress.REG_CVALB4); }
        public int ReadMax() { return Read32(I2cEncoderAddress.REG_CMAXB4); }
        public int ReadMin() { return Read32(I2cEncoderAddress.REG_CMINB4); }
        public float ReadMaxFloat() { return ReadFloat(I2cEncoderAddress.REG_CMAXB4); }
        public float ReadMinFloat() { return ReadFloat(I2cEncoderAddress.REG_CMINB4); }
        public int ReadStep() { return Read32(I2cEncoderAddress.REG_ISTEPB4); }
        public float ReadStepFloat() { return ReadFloat(I2cEncoderAddress.REG_ISTEPB4); }
        public int ReadGP1() { return Read8(I2cEncoderAddress.REG_GP1REG); }
        public int ReadGP2() { return Read8(I2cEncoderAddress.REG_GP2REG); }
        public int ReadGP3() { return Read8(I2cEncoderAddress.REG_GP3REG); }
        public int ReadAntiBouncingPeriod() { return Read8(I2cEncoderAddress.REG_ANTBOUNC); }
        public int ReadDoublePushPeriod() { return Read8(I2cEncoderAddress.REG_DPPERIOD); }
        public int ReadFadeRGB() { return Read8(I2cEncoderAddress.REG_FADERGB); }
        public int ReadFadeGP() { return Read8(I2cEncoderAddress.REG_FADEGP); }
        public I2cEncoderFadeStatus ReadFadeStatusRaw() { return (I2cEncoderFadeStatus)Read8(I2cEncoderAddress.REG_FSTATUS); }
        #endregion Public Read Methods

        #region Public Write Methods
        public bool WriteGP1Conf(int value) { return Write8(I2cEncoderAddress.REG_GP1CONF, value); }
        public bool WriteGP2Conf(int value) { return Write8(I2cEncoderAddress.REG_GP2CONF, value); }
        public bool WriteGP3Conf(int value) { return Write8(I2cEncoderAddress.REG_GP3CONF, value); }
        public bool WriteInterruptConfig(int value) { return Write8(I2cEncoderAddress.REG_INTCONF, value); }
        public bool WriteLEDR(int value) { return Write8(I2cEncoderAddress.REG_RLED, value); }
        public bool WriteLEDG(int value) { return Write8(I2cEncoderAddress.REG_GLED, value); }
        public bool WriteLEDB(int value) { return Write8(I2cEncoderAddress.REG_BLED, value); }
        public bool WriteCounter(int value) { return Write32(I2cEncoderAddress.REG_CVALB4, value); }
        public bool WriteCounterFloat(int value) { return WriteFloat(I2cEncoderAddress.REG_CVALB4, value); }
        public bool WriteMax(int value) { return Write32(I2cEncoderAddress.REG_CMAXB4, value); }
        public bool WriteMin(int value) { return Write32(I2cEncoderAddress.REG_CMINB4, value); }
        public bool WriteMaxFloat(int value) { return WriteFloat(I2cEncoderAddress.REG_CMAXB4, value); }
        public bool WriteMinFloat(int value) { return WriteFloat(I2cEncoderAddress.REG_CMINB4, value); }
        public bool WriteStep(int value) { return Write32(I2cEncoderAddress.REG_ISTEPB4, value); }
        public bool WriteStepFloat(int value) { return WriteFloat(I2cEncoderAddress.REG_ISTEPB4, value); }
        public bool WriteGP1(int value) { return Write8(I2cEncoderAddress.REG_GP1REG, value); }
        public bool WriteGP2(int value) { return Write8(I2cEncoderAddress.REG_GP2REG, value); }
        public bool WriteGP3(int value) { return Write8(I2cEncoderAddress.REG_GP3REG, value); }
        public bool WriteAntiBouncingPeriod(int value) { return Write8(I2cEncoderAddress.REG_ANTBOUNC, value); }
        public bool WriteDoublePushPeriod(int value) { return Write8(I2cEncoderAddress.REG_DPPERIOD, value); }
        public bool WriteFadeRGB(int value) { return Write8(I2cEncoderAddress.REG_FADERGB, value); }
        public bool WriteFadeGP(int value) { return Write8(I2cEncoderAddress.REG_FADEGP, value); }
        #endregion Public Write Methods
        #endregion Public Methods

        #region Private Methods
        #region Private Read Methods
        private byte[] ReadAll()
        {
            var bAddress = new byte[] { 0x00 };
            var buffer = new byte[34];

            var result = _i2cEncoder.WriteReadPartial(bAddress, buffer);

            return buffer;
        }


        public int Read8(I2cEncoderAddress address)
        {
            var bAddress = new byte[1];
            bAddress[0] = (byte)address;

            var buffer = new byte[1];

            var result = _i2cEncoder.WriteReadPartial(bAddress, buffer);
            return (int)buffer[0];
        }

        public int Read16(I2cEncoderAddress address)
        {
            var bAddress = new byte[1];
            bAddress[0] = (byte)address;

            var buffer = new byte[2];

            var result = _i2cEncoder.WriteReadPartial(bAddress, buffer);

            var bufferInt = 0;
            if (buffer[0] != 0 || buffer[1] != 0)
            {
                var bufferReverse = new Byte[2];
                bufferReverse[0] = buffer[1];
                bufferReverse[1] = buffer[0];
                bufferInt = BitConverter.ToInt16(bufferReverse, 0);
            }

            return bufferInt;
        }

        public int Read32(I2cEncoderAddress address)
        {
            var bAddress = new byte[1];
            bAddress[0] = (byte)address;

            var buffer = new byte[4];

            var result = _i2cEncoder.WriteReadPartial(bAddress, buffer);

            var bufferInt = 0;
            if (buffer[0] != 0 || buffer[1] != 0 || buffer[2] != 0 || buffer[3] != 0)
            {
                var bufferReverse = new Byte[4];
                bufferReverse[0] = buffer[3];
                bufferReverse[1] = buffer[2];
                bufferReverse[2] = buffer[1];
                bufferReverse[3] = buffer[0];
                bufferInt = BitConverter.ToInt32(bufferReverse, 0);
            }

            return bufferInt;
        }

        public float ReadFloat(I2cEncoderAddress address)
        {
            var bAddress = new byte[1];
            bAddress[0] = (byte)address;

            var buffer = new byte[4];

            var result = _i2cEncoder.WriteReadPartial(bAddress, buffer);

            float bufferFloat = 0;
            if (buffer[0] != 0 || buffer[1] != 0 || buffer[2] != 0 || buffer[3] != 0)
            {
                var bufferReverse = new Byte[4];
                bufferReverse[0] = buffer[3];
                bufferReverse[1] = buffer[2];
                bufferReverse[2] = buffer[1];
                bufferReverse[3] = buffer[0];
                bufferFloat = (float)BitConverter.ToSingle(bufferReverse, 0);
            }

            return bufferFloat;
        }
        #endregion Private Read Methods

        #region Private Write Methods
        public bool Write8(I2cEncoderAddress address, int value)
        {
            var buffer = new byte[2];
            buffer[0] = (byte)address;
            buffer[1] = (byte)value;

            var result = _i2cEncoder.WritePartial(buffer);

            if (result.Status == I2cTransferStatus.FullTransfer) { return true; } else { return false; }
        }

        private bool Write24(I2cEncoderAddress address, int value)
        {
            var buffer = new byte[4];
            buffer[0] = (byte)address;
            if (value != 0)
            {
                var bufferReverse = BitConverter.GetBytes(value);
                buffer[1] = bufferReverse[2];
                buffer[2] = bufferReverse[1];
                buffer[3] = bufferReverse[0];
            }

            var result = _i2cEncoder.WritePartial(buffer);

            if (result.Status == I2cTransferStatus.FullTransfer) { return true; } else { return false; }
        }

        private bool Write32(I2cEncoderAddress address, int value)
        {
            var buffer = new byte[5];
            buffer[0] = (byte)address;
            if (value != 0)
            {
                var bufferReverse = BitConverter.GetBytes(value);
                buffer[1] = bufferReverse[3];
                buffer[2] = bufferReverse[2];
                buffer[3] = bufferReverse[1];
                buffer[4] = bufferReverse[0];
            }

            var result = _i2cEncoder.WritePartial(buffer);

            if (result.Status == I2cTransferStatus.FullTransfer) { return true; } else { return false; }
        }

        private bool WriteFloat(I2cEncoderAddress address, float value)
        {
            var buffer = new byte[5];
            buffer[0] = (byte)address;
            if (value != (float)0.0)
            {
                var bufferReverse = BitConverter.GetBytes(value);
                buffer[1] = bufferReverse[3];
                buffer[2] = bufferReverse[2];
                buffer[3] = bufferReverse[1];
                buffer[4] = bufferReverse[0];
            }

            var result = _i2cEncoder.WritePartial(buffer);

            if (result.Status == I2cTransferStatus.FullTransfer) { return true; } else { return false; }
        }
        #endregion Private Write Methods
        #endregion Private Methods
    }
}
