namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a short midi message (max 3 bytes).
    /// </summary>
    public abstract class MidiShortMessage : MidiMessage
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
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

        private int _data;

        public int Data
        {
            get { return _data; }
            protected set { _data = value; }
        }

        public byte Status
        {
            get { return MidiEventData.GetStatus(Data); }
        }

        public byte Param1
        {
            get { return MidiEventData.GetParam1(Data); }
        }

        public byte Param2
        {
            get { return MidiEventData.GetParam2(Data); }
        }
    }
}