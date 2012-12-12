namespace CannedBytes.Midi.IO
{
    using CannedBytes.Midi.Message;

    /// <summary>
    /// A MidiFileEvent couples a (delta/absolute) time to a midi message.
    /// </summary>
    public class MidiFileEvent
    {
        /// <summary>
        /// A delta time value counting from the previous event.
        /// </summary>
        public long DeltaTime { get; set; }

        /// <summary>
        /// A absolute time value counting from the 'start'.
        /// </summary>
        public long AbsoluteTime { get; set; }

        /// <summary>
        /// The midi message - can be cast to a more-specific class type.
        /// </summary>
        public IMidiMessage Message { get; set; }
    }
}