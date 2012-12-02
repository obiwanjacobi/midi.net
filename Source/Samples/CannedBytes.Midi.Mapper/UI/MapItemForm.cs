using System;
using System.Windows.Forms;

namespace CannedBytes.Midi.Mapper.UI
{
    partial class MapItemForm : Form
    {
        public MapItemForm()
        {
            InitializeComponent();
        }

        public string ItemName
        {
            get { return this.MapItemName.Text; }
            set { this.MapItemName.Text = value; }
        }

        public int NoteInNumber
        {
            get { return this.NoteInNumberCtrl.NoteNumber; }
            set { this.NoteInNumberCtrl.NoteNumber = value; }
        }

        public string NoteInName
        {
            get { return this.NoteInNumberCtrl.NoteName; }
        }

        public int NoteOutNumber
        {
            get { return this.NoteOutNumberCtrl.NoteNumber; }
            set { this.NoteOutNumberCtrl.NoteNumber = value; }
        }

        public string NoteOutName
        {
            get { return this.NoteOutNumberCtrl.NoteName; }
        }

        private void HexChk_CheckedChanged(object sender, EventArgs e)
        {
            NoteInNumberCtrl.Hexadecimal = HexChk.Checked;
            NoteOutNumberCtrl.Hexadecimal = HexChk.Checked;
        }
    }
}