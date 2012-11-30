namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// A MidiEvent couples a (delta) time to a midi message.
    /// </summary>
    public class MidiEvent
    {
        /// <summary>
        /// A delta time value counting from the 'start'.
        /// </summary>
        public long DeltaTime { get; set; }

        /// <summary>
        /// The midi message - can be cast to a more-specific class type.
        /// </summary>
        public IMidiMessage Message { get; set; }
    }
}