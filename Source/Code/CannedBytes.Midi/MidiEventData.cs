namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiEventData struct allows manipulation of all the parts of a midi event used in a stream.
    /// </summary>
    /// <remarks>
    /// The <see cref="P:Data"/> property contains the raw midi event data and is the only
    /// property that is backed by a field. All other properties represent parts of the
    /// <see cref="P:Data"/> property.
    /// The <see cref="P:Length"/> property is used when the data is used in a midi stream
    /// and describes the length of a long midi stream event.
    /// The <see cref="P:Tempo"/> property is used when the data is used in a midi stream
    /// and describes the tempo for a tempo midi stream event.
    /// The <see cref="P:EventType"/> property is used when the data is used in a midi stream
    /// and describes the type of midi stream event.
    /// </remarks>
    public struct MidiEventData
    {
        /// <summary>Bit mask for the lower 8 bits.</summary>
        private const int Data8Mask = 0x000000FF;

        /// <summary>Bit mask for the lower 24 bits.</summary>
        private const int Data24Mask = 0x00FFFFFF;

        /// <summary>Number of bytes to shift for the EventType position.</summary>
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
            Data = data;
        }

        /// <summary>
        /// Gets or sets the raw midi short message data.
        /// </summary>
        public int Data { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <remarks>Used for midi events in midi streams. Only 24 bits are used.
        /// The last 8 bits are reserved for the <see cref="P:EventType"/> information.</remarks>
        public int Length
        {
            get
            {
                return GetData24(Data);
            }

            set
            {
                Data &= ~Data24Mask;
                Data |= value & Data24Mask;
            }
        }

        /// <summary>
        /// Gets or sets the tempo.
        /// </summary>
        /// <remarks>Used for midi events in midi streams. Only 24 bits are used.
        /// The upper 8 bits are reserved for the <see cref="P:EventType"/> information.</remarks>
        public int Tempo
        {
            get
            {
                return GetData24(Data);
            }

            set
            {
                Data &= ~Data24Mask;
                Data |= value & Data24Mask;
            }
        }

        /// <summary>
        /// Gets or sets the type of stream event.
        /// </summary>
        /// <remarks>Used for midi events in midi streams. Use in combination with the <see cref="P:Length"/> and
        /// <see cref="P:Tempo"/> properties.</remarks>
        public MidiEventType EventType
        {
            get
            {
                return GetEventType(Data);
            }

            set
            {
                Data &= Data24Mask;
                Data |= (byte)value << EventTypeShift;
            }
        }

        /// <summary>
        /// Explicit conversion to an <see cref="System.Int32"/>.
        /// </summary>
        /// <returns>Returns the raw midi <see cref="P:Data"/>.</returns>
        public int ToInt32()
        {
            return Data;
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
            if (obj is MidiEventData data)
            {
                return Equals(data);
            }

            if (obj is int @int)
            {
                return @int == Data;
            }

            return false;
        }

        /// <summary>
        /// Specialized typed overload.
        /// </summary>
        /// <param name="obj">An object to test.</param>
        /// <returns>Returns true if the <paramref name="obj"/> represents
        /// the same midi short message as this instance.</returns>
        public bool Equals(MidiEventData obj)
        {
            return Data.Equals(obj.Data);
        }

        /// <summary>
        /// Specialized override.
        /// </summary>
        /// <returns>Returns the hash code for this instance.</returns>
        /// <remarks>The hash code is based on the raw midi short message.</remarks>
        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

        /// <summary>
        /// Helper method to extract the lower 24 bits.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the lower 24 bits of <paramref name="data"/>.</returns>
        public static int GetData24(int data)
        {
            return data & Data24Mask;
        }

        /// <summary>
        /// Helper method to extract the event type.
        /// </summary>
        /// <param name="data">Short midi data.</param>
        /// <returns>Returns the event type of <paramref name="data"/>.</returns>
        public static MidiEventType GetEventType(int data)
        {
            return (MidiEventType)((data >> EventTypeShift) & Data8Mask);
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
            return dataLeft.Data == dataRight.Data;
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
            return dataLeft.Data != dataRight.Data;
        }
    }
}