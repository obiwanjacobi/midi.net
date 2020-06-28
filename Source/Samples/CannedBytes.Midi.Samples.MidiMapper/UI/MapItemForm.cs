using System;
using System.Windows.Forms;

namespace CannedBytes.Midi.Samples.MidiMapper.UI
{
    partial class MapItemForm : Form
    {
        public MapItemForm()
        {
            InitializeComponent();
        }

        public string ItemName
        {
            get { return MapItemName.Text; }
            set { MapItemName.Text = value; }
        }

        public int NoteInNumber
        {
            get { return NoteInNumberCtrl.NoteNumber; }
            set { NoteInNumberCtrl.NoteNumber = value; }
        }

        public string NoteInName
        {
            get { return NoteInNumberCtrl.NoteName; }
        }

        public int NoteOutNumber
        {
            get { return NoteOutNumberCtrl.NoteNumber; }
            set { NoteOutNumberCtrl.NoteNumber = value; }
        }

        public string NoteOutName
        {
            get { return NoteOutNumberCtrl.NoteName; }
        }

        private void HexChk_CheckedChanged(object sender, EventArgs e)
        {
            NoteInNumberCtrl.Hexadecimal = HexChk.Checked;
            NoteOutNumberCtrl.Hexadecimal = HexChk.Checked;
        }
    }
}