using System;
using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Message
{
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
            Contract.Ensures((byte)Command != 0xF0);
            Contract.Ensures(ByteLength > 0);

            Data = MidiData.GetData24(data);

            if ((Status & 0xF0) == 0xF0)
            {
                throw new ArgumentException("Status MSB of data is not MidiChannelCommand.", "data");
            }

            if (Command == MidiChannelCommands.ChannelPressure ||
                Command == MidiChannelCommands.ProgramChange)
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
        public MidiChannelCommands Command
        {
            get { return (MidiChannelCommands)(Status & 0xF0); }
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