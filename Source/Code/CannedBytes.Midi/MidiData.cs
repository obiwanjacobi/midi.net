using System;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiData struct allows manipulation of all the parts of a short midi message.
    /// </summary>
    /// <remarks>
    /// The <see cref="P:Data"/> property contains the raw short midi data and is the only
    /// property that is backed by a field. All other properties represent parts of the
    /// <see cref="P:Data"/> property.
    /// The <see cref="P:Status"/>, <see cref="P:Param1"/> and <see cref="P:Param2"/> properties
    /// are used for normal short midi messages.
    /// The <see cref="P:RunningStatusData"/> property returns the short midi message without the
    /// <see cref="P:Status"/> for use in a running status scenario.
    /// </remarks>
    public struct MidiData
    {
        private const int ChannelMask = 0x0000000F;
        private const int StatusMask = Data8Mask;
        private const int Param1Mask = 0x0000FF00;
        private const int Param2Mask = 0x00FF0000;
        private const int Data24Mask = 0x00FFFFFF;
        private const int Data16Mask = 0x0000FFFF;
        private const int Data8Mask = 0x000000FF;
        private const int Param1Shift = 8;
        private const int Param2Shift = 16;
        private const int RunningStatusDataShift = 8;
        private const int RunningStatusDataMask = 0x0000FFFF;

        /// <summary>
        /// Maximum value allowed for any data byte.
        /// </summary>
        public const int DataValueMax = 0x7F;

        /// <summary>
        /// Constructs a new instance for the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The raw midi short message.</param>
        /// <remarks>The value is assigned to the <see cref="P:Data"/> property.</remarks>
        public MidiData(int data)
        {
            _data = GetData24(data);
        }

        private int _data;

        /// <summary>
        /// Gets or sets the raw midi short message data.
        /// </summary>
        public int Data
        {
            get { return _data; }
            set { _data = GetData24(value); }
        }

        /// <summary>
        /// Gets the running status data (without the <see cref="P:Status"/> part).
        /// </summary>
        public int RunningStatusData
        {
            get { return (Data >> RunningStatusDataShift) & RunningStatusDataMask; }
        }

        /// <summary>
        /// Gets or sets the status part of the short midi message.
        /// </summary>
        /// <remarks>Setting the <see cref="P:Status"/> field will also set the <see cref="P:Channel"/>.</remarks>
        public byte Status
        {
            get { return GetStatus(_data); }
            set
            {
                _data &= ~StatusMask;
                _data |= (value & StatusMask);
            }
        }

        /// <summary>
        /// Gets or sets the channel part of the <see cref="P:Status"/> field.
        /// </summary>
        /// <remarks>Setting the <see cref="P:Status"/> field will also set the <see cref="P:Channel"/>.</remarks>
        public byte Channel
        {
            get { return (byte)(Status & ChannelMask); }
            set
            {
                _data &= ~ChannelMask;
                _data |= (value & ChannelMask);
            }
        }

        /// <summary>
        /// Gets a value indicating if <see cref="Param1"/> should have a value.
        /// </summary>
        public bool HasParam1
        {
            get
            {
                byte status = Status;

                switch (status)
                {
                    case 0xFF:
                    case 0xFE:
                    case 0xFC:
                    case 0xFB:
                    case 0xFA:
                    case 0xF9:
                    case 0xF8:
                    case 0xF6:
                    case 0xF0:
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets the param 1 part of the short midi message.
        /// </summary>
        /// <remarks>Note: Not all midi short messages use (all) parameters.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified <paramref name="value"/>
        /// is greater than <see cref="F:DataValueMax"/>.</exception>
        public byte Param1
        {
            get { return GetParam1(_data); }
            set
            {
                //Contract.Requires<ArgumentOutOfRangeException>(
                //    (value >= 0 && value <= DataValueMax), "The value set for Param1 was out of range (0-127).");

                _data &= ~Param1Mask;
                _data |= ((value << Param1Shift) & Param1Mask);
            }
        }

        /// <summary>
        /// Gets a value indicating if <see cref="Param2"/> should have a value.
        /// </summary>
        public bool HasParam2
        {
            get
            {
                byte status = Status;

                return HasParam1 && ((((status & 0xF0) != 0xC0) && ((status & 0xF0) != 0xD0)) || (status == 0xF2));
            }
        }

        /// <summary>
        /// Gets or sets the param 2 part of the short midi message.
        /// </summary>
        /// <remarks>Note: Not all midi short messages use (all) parameters.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified <paramref name="value"/>
        /// is greater than <see cref="F:DataValueMax"/>.</exception>
        public byte Param2
        {
            get { return GetParam2(_data); }
            set
            {
                //Contract.Requires<ArgumentOutOfRangeException>(
                //    (value >= 0 && value <= DataValueMax), "The value set for Param2 was out of range (0-127).");

                _data &= ~Param2Mask;
                _data |= ((value << Param2Shift) & Param2Mask);
            }
        }

        /// <summary>
        /// Explicit conversion to an <see cref="System.Int32"/>.
        /// </summary>
        /// <returns>Returns the raw midi <see cref="P:Data"/>.</returns>
        public int ToInt32()
        {
            return _data;
        }

        /// <summary>
        /// Optimized override.
        /// </summary>
        /// <param name="obj">An object to test.</param>
        /// <returns>Returns true if the <paramref name="obj"/> represents
        /// the same midi short message as this instance.</returns>
        /// <remarks>If the specified <paramref name="obj"/> is not of the
        /// MidiData type or the value is null, the method returns false.</remarks>
        public override bool Equals(object obj)
        {
            if (obj is MidiData)
            {
                return Equals((MidiData)obj);
            }

            if (obj is Int32)
            {
                return ((int)obj == _data);
            }

            return false;
        }

        /// <summary>
        /// Specialized typed overload.
        /// </summary>
        /// <param name="obj">An object to test.</param>
        /// <returns>Returns true if the <paramref name="obj"/> represents
        /// the same midi short message as this instance.</returns>
        public bool Equals(MidiData obj)
        {
            return _data.Equals(obj._data);
        }

        /// <summary>
        /// Specialized override.
        /// </summary>
        /// <returns>Returns the hash code for this instance.</returns>
        /// <remarks>The hash code is based on the raw midi short message.</remarks>
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }

        /// <summary>
        /// Helper method to extract the lower 24 bits.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the lower 24 bits of <paramref name="data"/>.</returns>
        public static int GetData24(int data)
        {
            return (data & Data24Mask);
        }

        /// <summary>
        /// Helper method to extract the lower 16 bits.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the lower 16 bits of <paramref name="data"/>.</returns>
        public static int GetData16(int data)
        {
            return (data & Data16Mask);
        }

        /// <summary>
        /// Helper method to extract the lower 8 bits.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the lower 8 bits of <paramref name="data"/>.</returns>
        public static int GetData8(int data)
        {
            return (data & Data8Mask);
        }

        /// <summary>
        /// Helper method to extract the status byte.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the status byte of <paramref name="data"/>.</returns>
        public static byte GetStatus(int data)
        {
            return (byte)(data & StatusMask);
        }

        /// <summary>
        /// Helper method to extract the first parameter byte.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the first parameter byte of <paramref name="data"/>.</returns>
        public static byte GetParam1(int data)
        {
            return (byte)((data >> Param1Shift) & Data8Mask);
        }

        /// <summary>
        /// Helper method to extract the second parameter byte.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the second parameter byte of <paramref name="data"/>.</returns>
        public static byte GetParam2(int data)
        {
            return (byte)((data >> Param2Shift) & Data8Mask);
        }

        /// <summary>
        /// An explicit conversion from a <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns a new MidiData instance.</returns>
        public static MidiData FromInt32(int data)
        {
            return new MidiData(data);
        }

        /// <summary>
        /// Implicit operator to convert to a <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="data">The MidiData to convert.</param>
        /// <returns>Returns the <see cref="P:Data"/> for <paramref name="data"/>.</returns>
        /// <remarks>
        /// This method is called in the following code:
        /// <code>
        /// MidiData MidiData = new MidiData(0);
        /// int shortMsg = MidiData;
        /// </code></remarks>
        public static implicit operator int(MidiData data)
        {
            return data.Data;
        }

        /// <summary>
        /// Optimized equality operator.
        /// </summary>
        /// <param name="dataLeft">The first (left) instance.</param>
        /// <param name="dataRight">The second (right) instance.</param>
        /// <returns>Returns true if both instances represent the same midi short message.</returns>
        /// <remarks>This method is called in the following code:
        /// <code>
        /// MidiData leftData = new MidiData(0);
        /// MidiData rightData = new MidiData(0);
        /// if(leftData == rightData)
        /// {
        ///   // they are equal
        /// }
        /// </code></remarks>
        public static bool operator ==(MidiData dataLeft, MidiData dataRight)
        {
            return (dataLeft._data == dataRight._data);
        }

        /// <summary>
        /// Optimized inequality operator.
        /// </summary>
        /// <param name="dataLeft">The first (left) instance.</param>
        /// <param name="dataRight">The second (right) instance.</param>
        /// <returns>Returns false if both instances represent the same midi short message.</returns>
        /// <remarks>This method is called in the following code:
        /// <code>
        /// MidiData leftData = new MidiData(0);
        /// MidiData rightData = new MidiData(0xFF);
        /// if(leftData != rightData)
        /// {
        ///   // they are not equal
        /// }
        /// </code></remarks>
        public static bool operator !=(MidiData dataLeft, MidiData dataRight)
        {
            return (dataLeft._data != dataRight._data);
        }
    }
}