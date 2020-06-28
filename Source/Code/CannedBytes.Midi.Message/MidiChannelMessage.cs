namespace CannedBytes.Midi.Message
{
    using System;

    /// <summary>
    /// Represents a (short) midi channel message.
    /// </summary>
    public class MidiChannelMessage : MidiShortMessage
    {
        /// <summary>
        /// Constructs a new instance for the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Lower/least significant (max) 3 bytes are filled.</param>
        public MidiChannelMessage(int data)
        {
            Data = MidiData.GetData24(data);

            if ((Status & 0xF0) == 0xF0)
            {
                throw new ArgumentException("Status MSB of data is not MidiChannelCommand.", nameof(data));
            }

            if (Command == MidiChannelCommand.ChannelPressure ||
                Command == MidiChannelCommand.ProgramChange)
            {
                ByteLength = 2;
            }
            else
            {
                ByteLength = 3;
            }
        }

        /// <summary>
        /// Gets the channel command in the message.
        /// </summary>
        public MidiChannelCommand Command
        {
            get { return (MidiChannelCommand)(Status & 0xF0); }
        }

        /// <summary>
        /// Gets the midi channel the message is sent/received on.
        /// </summary>
        public byte MidiChannel
        {
            get { return (byte)(Status & 0x0F); }
        }
    }
}