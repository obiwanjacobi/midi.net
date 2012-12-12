namespace CannedBytes.Midi.Message
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// A class that helps in generating a name for a note number.
    /// </summary>
    public class MidiNoteName
    {
        /// <summary>
        /// Contains all the names of all the notes (in one octave).
        /// </summary>
        private static readonly string[] NoteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        /// <summary>12 notes in one octave.</summary>
        private const byte NoteCount = 12;

        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiNoteName()
        {
        }

        /// <summary>
        /// Constructs a new instance for the specified <paramref name="noteNumber"/>.
        /// </summary>
        /// <param name="noteNumber">A note number as it is used in the NoteOn and NoteOff midi messages.</param>
        public MidiNoteName(byte noteNumber)
        {
            this.NoteNumber = noteNumber;
        }

        /// <summary>
        /// Constructs a new instance for the specified <paramref name="noteName"/>.
        /// </summary>
        /// <param name="noteName">Must not be null or empty.</param>
        public MidiNoteName(string noteName)
        {
            Contract.Requires(noteName != null);
            Throw.IfArgumentNullOrEmpty(noteName, "noteName");

            this.ParseNoteName(noteName);
        }

        /// <summary>
        /// Parses the note name into its components.
        /// </summary>
        /// <param name="newNoteName">Must not be null.</param>
        private void ParseNoteName(string newNoteName)
        {
            Contract.Requires(newNoteName != null);
            Throw.IfArgumentNull(newNoteName, "noteName");

            this.noteName = newNoteName.ToUpperInvariant();

            string nn = null;
            byte index = FindNoteName(this.noteName, out nn);

            if (!String.IsNullOrEmpty(nn))
            {
                this.noteName = nn;

                if (this.noteName.Length > nn.Length)
                {
                    this.octave = byte.Parse(this.noteName.Substring(nn.Length), CultureInfo.InvariantCulture);
                }

                this.noteNumber = (byte)((((int)this.octave - (int)this.OctaveOffset) * (int)NoteCount) + (int)index);

                this.CompileNoteName();
                return;
            }

            throw new ArgumentOutOfRangeException(
                      "newNoteName",
                      newNoteName,
                      "Specified argument was not recognized as a valid note name.");
        }

        /// <summary>
        /// Compiles a note name based on the specified <paramref name="newNoteNumber"/>.
        /// </summary>
        /// <param name="newNoteNumber">A note number as used in the midi NoteOn and NoteOff messages.</param>
        private void CompileNoteName(byte newNoteNumber)
        {
            this.octave = (byte)(((int)newNoteNumber / (int)NoteCount) + (int)this.OctaveOffset);
            this.noteName = NoteNames[newNoteNumber % 12];

            this.CompileNoteName();
        }

        /// <summary>
        /// Combines the note name and octave information.
        /// </summary>
        private void CompileNoteName()
        {
            this.fullNoteName = this.NoteName + this.Octave.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Finds the note name in the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A full note name that is compared to the possible known names.</param>
        /// <param name="result">The resulting note name - without octave information.</param>
        /// <returns>Returns an index of the note name found.</returns>
        private static byte FindNoteName(string value, out string result)
        {
            Contract.Requires(value != null);
            Throw.IfArgumentNull(value, "value");

            result = null;

            byte index = 0;
            byte resultIndex = 0;

            foreach (string nn in NoteNames)
            {
                if (nn != null && value.StartsWith(nn, StringComparison.OrdinalIgnoreCase))
                {
                    result = nn;
                    resultIndex = index;
                }

                index++;
            }

            return resultIndex;
        }

        /// <summary>
        /// Backing field for the <see cref="NoteNumber"/> property.
        /// </summary>
        private byte noteNumber;

        /// <summary>
        /// The note number as it is used in the midi NoteOn and NoteOff messages.
        /// </summary>
        public byte NoteNumber
        {
            get
            {
                return this.noteNumber;
            }

            set
            {
                Throw.IfArgumentOutOfRange(value, (byte)0, (byte)127, "NoteNumber");

                this.noteNumber = value;

                this.CompileNoteName(value);
            }
        }

        /// <summary>
        /// Backing field for the <see cref="FullNoteName"/> property.
        /// </summary>
        private string fullNoteName;

        /// <summary>
        /// Gets or sets the full note name including octave information.
        /// </summary>
        public string FullNoteName
        {
            get { return this.fullNoteName; }
            set { this.ParseNoteName(value); }
        }

        /// <summary>
        /// Backing field for the <see cref="NoteName"/> property.
        /// </summary>
        private string noteName;

        /// <summary>
        /// Gets or sets the bare note name.
        /// </summary>
        public string NoteName
        {
            get
            {
                return this.noteName;
            }

            set
            {
                this.ParseNoteName(value);
                this.CompileNoteName();
            }
        }

        /// <summary>
        /// Backing field for the <see cref="Octave"/> property.
        /// </summary>
        private byte octave;

        /// <summary>
        /// Gets or sets the octave.
        /// </summary>
        public byte Octave
        {
            get
            {
                return this.octave;
            }

            set
            {
                // TODO: validate.
                // octave (compensated with offset) should be in range 0-9
                this.octave = value;

                this.CompileNoteName();
            }
        }

        /// <summary>
        /// Backing field for the <see cref="OctaveOffset"/> property.
        /// </summary>
        private byte octaveOffset;

        /// <summary>
        /// Gets or sets the octave offset.
        /// </summary>
        /// <remarks>Usually a negative number or zero. Default value is 0 (zero).
        /// An octave offset of -2 is also common.</remarks>
        public byte OctaveOffset
        {
            get
            {
                return this.octaveOffset;
            }

            set
            {
                this.octaveOffset = value;
                this.CompileNoteName(this.NoteNumber);
            }
        }
    }
}