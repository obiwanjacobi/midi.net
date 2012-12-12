namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a short midi message (max 3 bytes).
    /// </summary>
    public abstract class MidiShortMessage : MidiMessage
    {
        /// <summary>
        /// Retrieves the short message data as a buffer.
        /// </summary>
        /// <returns>Never returns null.</returns>
        public override byte[] GetData()
        {
            byte[] data = new byte[this.ByteLength];

            if (this.ByteLength > 0)
            {
                data[0] = this.Status;
            }

            if (this.ByteLength > 1)
            {
                data[1] = this.Parameter1;
            }

            if (this.ByteLength > 2)
            {
                data[2] = this.Parameter2;
            }

            return data;
        }

        /// <summary>
        /// Gets the full data of the short midi message.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "It is the same thing.")]
        public int Data { get; set; }

        /// <summary>
        /// Gets the status part of the short message.
        /// </summary>
        public byte Status
        {
            get { return MidiData.GetStatus(this.Data); }
        }

        /// <summary>
        /// Gets the first (optional) parameter of the short message.
        /// </summary>
        public byte Parameter1
        {
            get { return MidiData.GetParameter1(this.Data); }
        }

        /// <summary>
        /// Gets the second (optional) parameter of the short message.
        /// </summary>
        public byte Parameter2
        {
            get { return MidiData.GetParameter2(this.Data); }
        }
    }
}