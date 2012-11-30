using System;

namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// A class that helps in generating a name for a note number.
    /// </summary>
    public class MidiNoteName
    {
        private static readonly string[] _noteNames = {
			"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        private const byte _noteCount = 12;

        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiNoteName()
        { }

        /// <summary>
        /// Constructs a new instance for the specified <paramref name="noteNumber"/>.
        /// </summary>
        /// <param name="noteNumber">A note number as it is used in the NoteOn and NoteOff midi messages.</param>
        public MidiNoteName(byte noteNumber)
        {
            NoteNumber = noteNumber;
        }

        /// <summary>
        /// Constructs a new instance for the specified <paramref name="noteName"/>.
        /// </summary>
        /// <param name="noteName">Must not be null or empty.</param>
        public MidiNoteName(string noteName)
        {
            ParseNoteName(noteName);
        }

        /// <summary>
        /// Parses the note name into its components.
        /// </summary>
        /// <param name="noteName"></param>
        private void ParseNoteName(string noteName)
        {
            noteName = noteName.ToUpperInvariant();

            string nn = null;
            byte index = FindNoteName(noteName, out nn);

            if (!String.IsNullOrEmpty(nn))
            {
                _noteName = nn;

                if (noteName.Length > nn.Length)
                {
                    _octave = byte.Parse(noteName.Substring(nn.Length));
                }

                _noteNumber = (byte)((((int)_octave - (int)OctaveOffset) * (int)_noteCount) + (int)index);

                CompileNoteName();
                return;
            }

            throw new ArgumentOutOfRangeException("noteName", noteName,
                "Specified argument was not recognized as a valid note name.");
        }

        /// <summary>
        /// Compiles a note name based on the specified <paramref name="noteNumber"/>.
        /// </summary>
        /// <param name="noteNumber">A note number as used in the midi NoteOn and NoteOff messages.</param>
        private void CompileNoteName(byte noteNumber)
        {
            _octave = (byte)(((int)noteNumber / (int)_noteCount) + (int)OctaveOffset);
            _noteName = _noteNames[noteNumber % 12];

            CompileNoteName();
        }

        /// <summary>
        /// Combines the note name and octave information.
        /// </summary>
        private void CompileNoteName()
        {
            _fullNoteName = NoteName + Octave.ToString();
        }

        /// <summary>
        /// Finds the note name in the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A full note name that is compared to the possible known names.</param>
        /// <param name="result">The resulting note name - without octave information.</param>
        /// <returns>Returns an index of the note name found.</returns>
        private byte FindNoteName(string value, out string result)
        {
            result = null;

            byte index = 0;
            byte resultIndex = 0;

            foreach (string nn in _noteNames)
            {
                if (value.StartsWith(nn))
                {
                    result = nn;
                    resultIndex = index;
                }

                index++;
            }

            return resultIndex;
        }

        private byte _noteNumber;

        /// <summary>
        /// The note number as it is used in the midi NoteOn and NoteOff messages.
        /// </summary>
        public byte NoteNumber
        {
            get { return _noteNumber; }
            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentOutOfRangeException("NoteNumber value", value,
                        "Specified note number is out of range. Expected range of note numbers is 0-127 (inclusive).");
                }

                _noteNumber = value;

                CompileNoteName(value);
            }
        }

        private string _fullNoteName;

        /// <summary>
        /// Gets or sets the full note name including octave information.
        /// </summary>
        public string FullNoteName
        {
            get { return _fullNoteName; }
            set { ParseNoteName(value); }
        }

        private string _noteName;

        /// <summary>
        /// Gets or sets the bare note name
        /// </summary>
        public string NoteName
        {
            get { return _noteName; }
            set
            {
                ParseNoteName(value);
                CompileNoteName();
            }
        }

        private byte _octave;

        /// <summary>
        /// Gets or sets the octave.
        /// </summary>
        public byte Octave
        {
            get { return _octave; }
            set
            {
                // TODO: validate.
                // octave (compensated with offset) should be in range 0-9
                _octave = value;

                CompileNoteName();
            }
        }

        private byte _octaveOffset;

        /// <summary>
        /// Gets or sets the octave offset.
        /// </summary>
        /// <remarks>Usually a negative number or zero. Default value is 0 (zero).
        /// An octave offset of -2 is also common.</remarks>
        public byte OctaveOffset
        {
            get { return _octaveOffset; }
            set
            {
                _octaveOffset = value;
                CompileNoteName(NoteNumber);
            }
        }
    }
}