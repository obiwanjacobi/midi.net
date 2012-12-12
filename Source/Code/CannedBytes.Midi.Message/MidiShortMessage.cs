namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a short midi message (max 3 bytes).
    /// </summary>
    public abstract class MidiShortMessage : MidiMessage
    {
        /// <summary>
        ///Retrieves the short message data as a buffer.
        /// </summary>
        /// <returns>Never returns null.</returns>
        public override byte[] GetData()
        {
            byte[] data = new byte[ByteLength];

            if (ByteLength > 0)
            {
                data[0] = Status;
            }

            if (ByteLength > 1)
            {
                data[1] = Param1;
            }

            if (ByteLength > 2)
            {
                data[2] = Param2;
            }

            return data;
        }

        /// <summary>
        /// Gets the full data of the short midi message.
        /// </summary>
        public int Data { get; set; }

        /// <summary>
        /// Gets the status part of the short message.
        /// </summary>
        public byte Status
        {
            get { return MidiData.GetStatus(Data); }
        }

        /// <summary>
        /// Gets the first (optional) parameter of the short message.
        /// </summary>
        public byte Param1
        {
            get { return MidiData.GetParameter1(Data); }
        }

        /// <summary>
        /// Gets the second (optional) parameter of the short message.
        /// </summary>
        public byte Param2
        {
            get { return MidiData.GetParameter2(Data); }
        }
    }
}