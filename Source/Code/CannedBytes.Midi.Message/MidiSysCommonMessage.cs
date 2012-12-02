using System;

namespace CannedBytes.Midi.Message
{
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

            if (Enum.IsDefined(typeof(MidiSysCommonTypes), Data))
            {
                CommonType = (MidiSysCommonTypes)Enum.ToObject(typeof(MidiSysCommonTypes), Data);
            }
            else
            {
                CommonType = MidiSysCommonTypes.Invalid;
            }
        }

        /// <summary>
        /// The type of system common message.
        /// </summary>
        public MidiSysCommonTypes CommonType { get; private set; }
    }
}