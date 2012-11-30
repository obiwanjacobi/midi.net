namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// An enumeration of the different types of meta messages.
    /// </summary>
    /// <remarks>
    /// <seealso cref="http://home.roadrunner.com/~jgglatt/tech/midifile.htm"/>
    /// </remarks>
    public enum MidiMetaTypes
    {
        /// <summary>
        /// Meta event was not initialized or recognized.
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Optional event identifying the sequence by number.
        /// </summary>
        /// <remarks>
        /// FF 00 02 ss ss
        /// or...
        /// FF 00 00
        /// This optional event must occur at the beginning of a MTrk (ie, before any non-zero
        /// delta-times and before any midi events). It specifies the sequence number.
        /// The two data bytes ss ss, are that number which corresponds to the MIDI Cue message.
        /// In a format 2 MIDI file, this number identifies each "pattern" (ie, Mtrk) so that
        /// a "song" sequence can use the MIDI Cue message to refer to patterns.
        /// If the ss ss numbers are omitted (ie, the second form shown above), then the MTrk's
        /// location in the file is used. (ie, The first MTrk chunk is sequence number 0.
        /// The second MTrk is sequence number 1. Etc).
        /// In format 0 or 1, which contain only one "pattern" (even though format 1 contains
        /// several MTrks), this event is placed in only the first MTrk. So, a group of
        /// format 0 or 1 files with different sequence numbers can comprise a "song collection".
        /// There can be only one of these events per MTrk chunk in a Format 2.
        /// There can be only one of these events in a Format 0 or 1, and it must be in the first MTrk.
        /// </remarks>
        SequenceNumber = 0x00,
        /// <summary>
        /// General purpose text.
        /// </summary>
        /// <remarks>
        /// FF 01 len text
        /// Any amount of text (amount of bytes = len) for any purpose. It's best to put this
        /// event at the beginning of an MTrk. Although this text could be used for any purpose,
        /// there are other text-based Meta-Events for such things as orchestration, lyrics,
        /// track name, etc. This event is primarily used to add "comments" to a MIDI file which
        /// a program would be expected to ignore when loading that file.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        Text = 0x01,
        /// <summary>
        /// Copyright statement.
        /// </summary>
        /// <remarks>
        /// FF 02 len text
        /// A copyright message. It's best to put this event at the beginning of an MTrk.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        Copyright = 0x02,
        /// <summary>
        /// The name of the track.
        /// </summary>
        /// <remarks>
        /// FF 03 len text
        /// The name of the sequence or track. It's best to put this event at the beginning of an MTrk.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        TrackName = 0x03,
        /// <summary>
        /// Name of the instrument being used to play the track.
        /// </summary>
        /// <remarks>
        /// FF 04 len text
        /// The name of the instrument (ie, MIDI module) being used to play the track. This may be
        /// different than the Sequence/Track Name. For example, maybe the name of your sequence
        /// (ie, Mtrk) is "Butterfly", but since the track is played upon a Roland S-770, you may
        /// also include an Instrument Name of "Roland S-770".
        /// It's best to put one (or more) of this event at the beginning of an MTrk to provide
        /// the user with identification of what instrument(s) is playing the track. Usually,
        /// the instruments (ie, patches, tones, banks, etc) are setup on the audio devices via
        /// MIDI Program Change and MIDI Bank Select Controller events within the MTrk. So, this
        /// event exists merely to provide the user with visual feedback of what instruments are used
        /// for a track.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        Instrument = 0x04,
        /// <summary>
        /// A song lyric which occurs on a given beat.
        /// </summary>
        /// <remarks>
        /// FF 05 len text
        /// A song lyric which occurs on a given beat. A single Lyric MetaEvent should contain only one syllable.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        Lyric = 0x05,
        /// <summary>
        /// The text for a marker which occurs on a given beat.
        /// </summary>
        /// <remarks>
        /// FF 06 len text
        /// The text for a marker which occurs on a given beat. Marker events might be used to denote
        /// a loop start and loop end (ie, where the sequence loops back to a previous event).
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        Marker = 0x06,
        /// <summary>
        /// The text for a cue point which occurs on a given beat.
        /// </summary>
        /// <remarks>
        /// FF 07 len text
        /// The text for a cue point which occurs on a given beat. A Cue Point might be used to denote
        /// where a WAVE (ie, sampled sound) file starts playing, for example, where the text would be
        /// the WAVE's filename.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        CuePoint = 0x07,
        /// <summary>
        /// The name of the program (or patch) used to play the MTrk.
        /// </summary>
        /// <remarks>
        /// FF 08 len text
        /// The name of the program (ie, patch) used to play the MTrk. This may be different than the
        /// Sequence/Track Name. For example, maybe the name of your sequence (ie, Mtrk) is "Butterfly",
        /// but since the track is played upon an electric piano patch, you may also include a Program
        /// Name of "ELECTRIC PIANO".
        /// Usually, the instruments (ie, patches, tones, banks, etc) are setup on the audio devices via
        /// MIDI Program Change and MIDI Bank Select Controller events within the MTrk. So, this event
        /// exists merely to provide the user with visual feedback of what particular patch is used for
        /// a track. But it can also give a hint to intelligent software if patch remapping needs to be
        /// done. For example, if the MIDI file was created on a non-General MIDI instrument, then the
        /// MIDI Program Change event will likely contain the wrong value when played on a General MIDI
        /// instrument. Intelligent software can use the Program Name event to look up the correct value
        /// for the MIDI Program Change event.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        PatchName = 0x08,
        /// <summary>
        /// The name of the MIDI device (port) where the track is routed.
        /// </summary>
        /// <remarks>
        /// FF 09 len text
        /// The name of the MIDI device (port) where the track is routed. This replaces the "MIDI Port"
        /// Meta-Event which some sequencers formally used to route MIDI tracks to various MIDI ports
        /// (in order to support more than 16 MIDI channels).
        /// For example, assume that you have a MIDI interface that has 4 MIDI output ports. They are
        /// listed as "MIDI Out 1", "MIDI Out 2", "MIDI Out 3", and "MIDI Out 4". If you wished a
        /// particular MTrk to use "MIDI Out 1" then you would put a Port Name Meta-event at the beginning
        /// of the MTrk, with "MIDI Out 1" as the text.
        /// All MIDI events that occur in the MTrk, after a given Port Name event, will be routed to that port.
        /// In a format 0 MIDI file, it would be permissible to have numerous Port Name events intermixed
        /// with MIDI events, so that the one MTrk could address numerous ports. But that would likely
        /// make the MIDI file much larger than it need be. The Port Name event is useful primarily in
        /// format 1 MIDI files, where each MTrk gets routed to one particular port.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        DeviceName = 0x09,
        /// <summary>
        /// A definitive marking of the end of a track.
        /// </summary>
        /// <remarks>
        /// FF 2F 00
        /// This event is not optional. It must be the last event in every MTrk. It's used as a
        /// definitive marking of the end of an MTrk. Only 1 per MTrk.
        /// </remarks>
        EndOfTrack = 0x2F,
        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// FF 51 03 tt tt tt
        /// Indicates a tempo change. The 3 data bytes of tt tt tt are the tempo in microseconds per
        /// quarter note. In other words, the microsecond tempo value tells you how long each one of
        /// your sequencer's "quarter notes" should be. For example, if you have the 3 bytes of
        /// 07 A1 20, then each quarter note should be 0x07A120 (or 500,000) microseconds long.
        /// So, the MIDI file format expresses tempo as "the amount of time (ie, microseconds) per
        /// quarter note".
        /// NOTE: If there are no tempo events in a MIDI file, then the tempo is assumed to be 120 BPM
        /// In a format 0 file, the tempo changes are scattered throughout the one MTrk. In format 1,
        /// the very first MTrk should consist of only the tempo (and time signature) events so that it
        /// could be read by some device capable of generating a "tempo map". It is best not to place
        /// MIDI events in this MTrk. In format 2, each MTrk should begin with at least one initial
        /// tempo (and time signature) event.
        /// </remarks>
        Tempo = 0x51,
        /// <summary>
        /// Designates the SMPTE start time of the track.
        /// </summary>
        /// <remarks>
        /// FF 54 05 hr mn se fr ff
        /// Designates the SMPTE start time (hours, minutes, seconds, frames, subframes) of the MTrk.
        /// It should be at the start of the MTrk. The hour should not be encoded with the SMPTE format
        /// as it is in MIDI Time Code. In a format 1 file, the SMPTE OFFSET must be stored with the
        /// tempo map (ie, the first MTrk), and has no meaning in any other MTrk. The ff field contains
        /// fractional frames in 100ths of a frame, even in SMPTE based MTrks which specify a different
        /// frame subdivision for delta-times (ie, different from the subframe setting in the MThd).
        /// </remarks>
        SmpteOffset = 0x54,
        /// <summary>
        /// The Time signature of the song.
        /// </summary>
        /// <remarks>
        /// FF 58 04 nn dd cc bb
        /// Time signature is expressed as 4 numbers. nn and dd represent the "numerator" and "denominator"
        /// of the signature as notated on sheet music. The denominator is a negative power of 2:
        /// 2 = quarter note, 3 = eighth, etc.
        /// The cc expresses the number of MIDI clocks in a metronome click.
        /// The bb parameter expresses the number of notated 32nd notes in a MIDI quarter note (24 MIDI
        /// clocks). This event allows a program to relate what MIDI thinks of as a quarter, to something
        /// entirely different.
        /// For example, 6/8 time with a metronome click every 3 eighth notes and 24 clocks per quarter
        /// note would be the following event:
        /// FF 58 04 06 03 18 08
        /// NOTE: If there are no time signature events in a MIDI file, then the time signature is assumed
        /// to be 4/4.
        /// In a format 0 file, the time signatures changes are scattered throughout the one MTrk. In
        /// format 1, the very first MTrk should consist of only the time signature (and tempo) events so
        /// that it could be read by some device capable of generating a "tempo map". It is best not to
        /// place MIDI events in this MTrk. In format 2, each MTrk should begin with at least one initial
        /// time signature (and tempo) event.
        /// </remarks>
        TimeSignature = 0x58,
        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// FF 59 02 sf mi
        /// sf = -7 for 7 flats, -1 for 1 flat, etc, 0 for key of c, 1 for 1 sharp, etc.
        /// mi = 0 for major, 1 for minor
        /// </remarks>
        KeySignature = 0x59,
        /// <summary>
        /// This can be used by a program to store proprietary data.
        /// </summary>
        /// <remarks>
        /// FF 7F len data
        /// This can be used by a program to store proprietary data. The first byte(s) should be a
        /// unique ID of some sort so that a program can identity whether the event belongs to it,
        /// or to some other program. A 4 character (ie, ascii) ID is recommended for such.
        /// Note that len could be a series of bytes since it is expressed as a variable length quantity.
        /// </remarks>
        Custom = 0x7F
    }
}