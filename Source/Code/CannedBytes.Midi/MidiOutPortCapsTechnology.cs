namespace CannedBytes.Midi
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the different types of technology used by a MIDI out device.
    /// </summary>
    public enum MidiOutPortCapsTechnology
    {
        /// <summary>Not set.</summary>
        None = 0,

        /// <summary>The device is a MIDI port.</summary>
        MidiPort = 1,

        /// <summary>The device is a MIDI synth.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Synth", Justification = "Synth is just fine.")]
        Synth = 2,

        /// <summary>The device is a square wave synth.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Synth", Justification = "Synth is just fine.")]
        SquareWaveSynth = 3,

        /// <summary>The device is an FM synth.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Synth", Justification = "Synth is just fine.")]
        FMSynth = 4,

        /// <summary>The device is a MIDI mapper.</summary>
        MidiMapper = 5,

        /// <summary>The device is a Wavetable synth.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Synth", Justification = "Synth is just fine.")]
        WavetableSynth = 6,

        /// <summary>The device is a software synth.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Synth", Justification = "Synth is just fine.")]
        SoftwareSynth = 7
    }
}