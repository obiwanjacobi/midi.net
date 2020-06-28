using System;

namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a midi real-time (short) message.
    /// </summary>
    public class MidiSysRealtimeMessage : MidiShortMessage
    {
        /// <summary>
        /// Constructs a new instance on the specified message <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Only the least significant byte is set.</param>
        public MidiSysRealtimeMessage(int data)
        {
            Data = MidiData.GetData8(data);
            ByteLength = 1;

            if (Enum.IsDefined(typeof(MidiSysRealtimeType), Data))
            {
                RealtimeType = (MidiSysRealtimeType)Enum.ToObject(typeof(MidiSysRealtimeType), Data);
            }
            else
            {
                RealtimeType = MidiSysRealtimeType.Invalid;
            }
        }

        /// <summary>
        /// The type of real-time midi message.
        /// </summary>
        public MidiSysRealtimeType RealtimeType { get; set; }
    }
}