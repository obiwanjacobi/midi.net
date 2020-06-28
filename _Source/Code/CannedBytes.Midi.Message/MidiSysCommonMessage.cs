namespace CannedBytes.Midi.Message
{
    using System;

    /// <summary>
    /// Represents Common System (short) midi message.
    /// </summary>
    public class MidiSysCommonMessage : MidiShortMessage
    {
        /// <summary>
        /// Constructs a new instance on the specified message <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Only the least significant byte is set.</param>
        public MidiSysCommonMessage(int data)
        {
            Data = MidiData.GetData8(data);
            ByteLength = 1;

            if (Enum.IsDefined(typeof(MidiSysCommonType), Data))
            {
                this.CommonType = (MidiSysCommonType)Enum.ToObject(typeof(MidiSysCommonType), Data);
            }
            else
            {
                this.CommonType = MidiSysCommonType.Invalid;
            }
        }

        /// <summary>
        /// The type of system common message.
        /// </summary>
        public MidiSysCommonType CommonType { get; private set; }
    }
}