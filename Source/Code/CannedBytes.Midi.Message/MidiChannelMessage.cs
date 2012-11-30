using System;

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
            Data = data;

            #region Method Checks

            if (!(Status >= (byte)MidiChannelCommand.NoteOff &&
                    Status <= ((byte)MidiChannelCommand.PitchWheel | 0x0F)))
            {
                throw new ArgumentException("Status of data is not MidiChannelCommand.", "data");
            }

            #endregion Method Checks

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
            get { return (MidiChannelCommand)(MidiEventData.GetStatus(Data) & (byte)0xF0); }
        }

        /// <summary>
        /// Gets the midi channel the message is sent/received on.
        /// </summary>
        public byte MidiChannel
        {
            get { return (byte)(MidiEventData.GetStatus(Data) & (byte)0x0F); }
        }
    }
}