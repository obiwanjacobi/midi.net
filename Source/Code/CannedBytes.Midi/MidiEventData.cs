using System;
using System.Diagnostics.Contracts;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiEventData struct allows manipulation of all the parts of a short midi message.
    /// </summary>
    /// <remarks>
    /// The <see cref="P:Data"/> property contains the raw short midi data and is the only
    /// property that is backed by a field. All other properties represent parts of the
    /// <see cref="P:Data"/> property.
    /// The <see cref="P:Status"/>, <see cref="P:Param1"/> and <see cref="P:Param2"/> properties
    /// are used for normal short midi messages.
    /// The <see cref="P:Length"/> property is used when the data is used in a midi stream
    /// and describes the length of a long midi stream event.
    /// The <see cref="P:Tempo"/> property is used when the data is used in a midi stream
    /// and describes the tempo for a tempo midi stream event.
    /// The <see cref="P:EventType"/> property is used when the data is used in a midi stream
    /// and describes the type of midi stream event.
    /// The <see cref="P:RunningStatusData"/> property returns the short midi message without the
    /// <see cref="P:Status"/> for use in a running status scenario.
    /// </remarks>
    public struct MidiEventData
    {
        private const int ChannelMask = 0x0000000F;
        private const int StatusMask = 0x000000FF;
        private const int Param1Mask = 0x0000FF00;
        private const int Param2Mask = 0x00FF0000;
        private const int Data24Mask = 0x00FFFFFF;
        private const int Param1Shift = 8;
        private const int Param2Shift = 16;
        private const int EventTypeShift = 24;

        /// <summary>
        /// Maximum value allowed for any data byte.
        /// </summary>
        public const int DataValueMax = 0x7F;

        /// <summary>
        /// Constructs a new instance for the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The raw midi short message.</param>
        /// <remarks>The value is assigned to the <see cref="P:Data"/> property.</remarks>
        public MidiEventData(int data)
        {
            _data = data;
        }

        private int _data;

        /// <summary>
        /// Gets or sets the raw midi short message data.
        /// </summary>
        public int Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Gets the running status data (without the <see cref="P:Status"/> part).
        /// </summary>
        public int RunningStatusData
        {
            get { return (Data >> 8) & 0x0000FFFF; }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <remarks>Only 24 bits are used.
        /// The last 8 bits are reserved for the <see cref="P:EventType"/> information.</remarks>
        public int Length
        {
            get { return GetData24(_data); }
            set { _data = (value & Data24Mask); }
        }

        /// <summary>
        /// Gets or sets the tempo.
        /// </summary>
        /// <remarks>Only 24 bits are used.
        /// The last 8 bits are reserved for the <see cref="P:EventType"/> information.</remarks>
        public int Tempo
        {
            get { return GetData24(_data); }
            set { _data = (value & Data24Mask); }
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
                _data |= value;
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
                Contract.Requires<ArgumentOutOfRangeException>(
                    (value >= 0 && value <= DataValueMax), "The value set for Param1 was out of range (0-127).");

                _data &= ~Param1Mask;
                _data |= (value << Param1Shift);
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
                Contract.Requires<ArgumentOutOfRangeException>(
                    (value >= 0 && value <= DataValueMax), "The value set for Param2 was out of range (0-127).");

                _data &= ~Param2Mask;
                _data |= (value << Param2Shift);
            }
        }

        /// <summary>
        /// Gets or sets the type of stream event.
        /// </summary>
        /// <remarks>Use in combination with the <see cref="P:Length"/> and
        /// <see cref="P:Tempo"/> properties.</remarks>
        public MidiEventType EventType
        {
            get { return GetEventType(_data); }
            set
            {
                _data &= ~Data24Mask;
                _data |= ((byte)value << EventTypeShift);
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
        /// MidiEventData type or the value is null, the method returns false.</remarks>
        public override bool Equals(object obj)
        {
            if (!(obj is MidiEventData))
            {
                return false;
            }

            return Equals((MidiEventData)obj);
        }

        /// <summary>
        /// Specialized typed overload.
        /// </summary>
        /// <param name="obj">An object to test.</param>
        /// <returns>Returns true if the <paramref name="obj"/> represents
        /// the same midi short message as this instance.</returns>
        public bool Equals(MidiEventData obj)
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
            return (byte)((data & Param1Mask) >> Param1Shift);
        }

        /// <summary>
        /// Helper method to extract the second parameter byte.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the second parameter byte of <paramref name="data"/>.</returns>
        public static byte GetParam2(int data)
        {
            return (byte)((data & Param2Mask) >> Param2Shift);
        }

        /// <summary>
        /// Helper method to extract the event type.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the event type of <paramref name="data"/>.</returns>
        public static MidiEventType GetEventType(int data)
        {
            return (MidiEventType)((data & ~Data24Mask) >> EventTypeShift);
        }

        /// <summary>
        /// An explicit conversion from a <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns a new MidiEventData instance.</returns>
        public static MidiEventData FromInt32(int data)
        {
            return new MidiEventData(data);
        }

        /// <summary>
        /// Implicit operator to convert to a <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="data">The MidiEventData to convert.</param>
        /// <returns>Returns the <see cref="P:Data"/> for <paramref name="data"/>.</returns>
        /// <remarks>
        /// This method is called in the following code:
        /// <code>
        /// MidiEventData midiEventData = new MidiEventData(0);
        /// int shortMsg = midiEventData;
        /// </code></remarks>
        public static implicit operator int(MidiEventData data)
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
        /// MidiEventData leftData = new MidiEventData(0);
        /// MidiEventData rightData = new MidiEventData(0);
        /// if(leftData == rightData)
        /// {
        ///   // they are equal
        /// }
        /// </code></remarks>
        public static bool operator ==(MidiEventData dataLeft, MidiEventData dataRight)
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
        /// MidiEventData leftData = new MidiEventData(0);
        /// MidiEventData rightData = new MidiEventData(0xFF);
        /// if(leftData != rightData)
        /// {
        ///   // they are not equal
        /// }
        /// </code></remarks>
        public static bool operator !=(MidiEventData dataLeft, MidiEventData dataRight)
        {
            return (dataLeft._data != dataRight._data);
        }
    }
}