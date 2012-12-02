namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Specifies constants defining channel message types.
    /// </summary>
    public enum MidiChannelCommands
    {
        /// <summary>
        /// Represents the note-off command type.
        /// </summary>
        NoteOff = 0x80,

        /// <summary>
        /// Represents the note-on command type.
        /// </summary>
        NoteOn = 0x90,

        /// <summary>
        /// Represents the poly pressure (after touch) command type.
        /// </summary>
        PolyPressure = 0xA0,

        /// <summary>
        /// Represents the controller command type.
        /// </summary>
        ControlChange = 0xB0,

        /// <summary>
        /// Represents the program change command type.
        /// </summary>
        ProgramChange = 0xC0,

        /// <summary>
        /// Represents the channel pressure (after touch) command
        /// type.
        /// </summary>
        ChannelPressure = 0xD0,

        /// <summary>
        /// Represents the pitch wheel command type.
        /// </summary>
        PitchWheel = 0xE0,
    }
}