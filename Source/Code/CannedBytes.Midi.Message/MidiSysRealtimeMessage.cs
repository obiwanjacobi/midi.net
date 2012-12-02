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

            if (Enum.IsDefined(typeof(MidiSysRealtimeTypes), Data))
            {
                RealtimeType = (MidiSysRealtimeTypes)Enum.ToObject(typeof(MidiSysRealtimeTypes), Data);
            }
            else
            {
                RealtimeType = MidiSysRealtimeTypes.Invalid;
            }
        }

        public MidiSysRealtimeTypes RealtimeType { get; set; }
    }
}