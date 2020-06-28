using CannedBytes.Midi.Message;
using System;
using System.Windows.Forms;

namespace CannedBytes.Midi.Samples.MidiMapper.UI
{
    partial class NoteNumberControl : UserControl
    {
        public NoteNumberControl()
        {
            InitializeComponent();
        }

        public bool Hexadecimal
        {
            get { return NoteNo.Hexadecimal; }
            set { NoteNo.Hexadecimal = value; }
        }

        private MidiNoteName _noteName = new MidiNoteName();

        public int NoteNumber
        {
            get { return _noteName.NoteNumber; }
            set
            {
                _noteName.NoteNumber = (byte)value;
                UpdateUI();
            }
        }

        public string NoteName
        {
            get { return _noteName.FullNoteName; }
            set
            {
                _noteName.FullNoteName = value;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            this.NoteNo.Value = _noteName.NoteNumber;
            this.NoteNameCmb.Text = _noteName.NoteName;
            this.NoteNameOctave.Value = _noteName.Octave;
        }

        private void NoteNameCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoteNameCmb.SelectedIndex >= 0)
            {
                _noteName.NoteName = NoteNameCmb.Text;

                this.NoteNo.Value = _noteName.NoteNumber;
                this.NoteNameOctave.Value = _noteName.Octave;
            }
        }

        private void NoteNameOctave_ValueChanged(object sender, EventArgs e)
        {
            _noteName.Octave = (byte)NoteNameOctave.Value;

            this.NoteNo.Value = _noteName.NoteNumber;
            this.NoteNameCmb.Text = _noteName.NoteName;
        }

        private void NoteNo_ValueChanged(object sender, EventArgs e)
        {
            _noteName.NoteNumber = (byte)NoteNo.Value;

            this.NoteNameCmb.Text = _noteName.NoteName;
            this.NoteNameOctave.Value = _noteName.Octave;
        }
    }
}