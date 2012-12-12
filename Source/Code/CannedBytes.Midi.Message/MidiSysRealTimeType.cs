namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// The types of real-time midi messages.
    /// </summary>
    public enum MidiSysRealTimeType
    {
        /// <summary>
        /// Not a system real-time message.
        /// </summary>
        Invalid = 0x00,

        /// <summary>
        /// Some master device that controls sequence playback sends this timing
        /// message to keep a slave device in sync with the master. A MIDI Clock
        /// message is sent at regular intervals (based upon the master's Tempo)
        /// in order to accomplish this.
        /// </summary>
        MidiClock = 0xF8,

        /// <summary>
        /// Some master device that controls sequence playback sends this timing
        /// message to keep a slave device in sync with the master. A MIDI Tick
        /// message is sent at regular intervals of one message every 10 milliseconds.
        /// </summary>
        MidiTick = 0xF9,

        /// <summary>
        /// Some master device that controls sequence playback sends this message to
        /// make a slave device start playback of some song/sequence from the beginning.
        /// </summary>
        MidiStart = 0xFA,

        /// <summary>
        /// Some master device that controls sequence playback sends this message
        /// to make a slave device resume playback from its current "Song Position".
        /// The current Song Position is the point when the song/sequence was
        /// previously stopped, or previously cued with a Song Position Pointer message.
        /// </summary>
        MidiContinue = 0xFB,

        /// <summary>
        /// Some master device that controls sequence playback sends this
        /// message to make a slave device stop playback of a song/sequence.
        /// </summary>
        MidiStop = 0xFC,

        /// <summary>
        /// A device sends out an Active Sense message (at least once) every
        /// 300 milliseconds if there has been no other activity on the MIDI buss,
        /// to let other devices know that there is still a good MIDI connection between the devices.
        /// </summary>
        ActiveSense = 0xFE,

        /// <summary>
        /// The device receiving this should reset itself to a default state,
        /// usually the same state as when the device was turned on. Often,
        /// this means to turn off all playing notes, turn the local keyboard on,
        /// clear running status, set Song Position to 0, stop any timed playback
        /// (of a sequence), and perform any other standard setup unique to the device.
        /// Also, a device may choose to kick itself into Omni On, Poly mode if it
        /// has no facilities for allowing the musician to store a default mode.
        /// </summary>
        Reset = 0xFF
    }
}