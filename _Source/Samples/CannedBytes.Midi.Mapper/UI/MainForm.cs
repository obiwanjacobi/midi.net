using global::System;
using global::System.Windows.Forms;

namespace CannedBytes.Midi.Mapper.UI
{
    partial class MainForm : Form
    {
        private MidiNoteMapper _mapper;

        public MainForm()
        {
            InitializeComponent();

            FillMidiPortLists();

            if (this.MidiInList.Items.Count > 0)
            {
                this.MidiInList.SelectedIndex = 0;
            }

            if (this.MidiOutList.Items.Count > 0)
            {
                if (this.MidiInList.Items.Count > 0)
                {
                    this.MidiOutList.Text = this.MidiInList.Text;
                }
                else
                {
                    this.MidiOutList.SelectedIndex = 0;
                }
            }

            this.ListenChannel.SelectedIndex = 0;
            this.SendChannel.SelectedIndex = 0;

            NewDocument();
        }

        private void FillMidiPortLists()
        {
            foreach (MidiInPortCaps inCaps in new MidiInPortCapsCollection())
            {
                this.MidiInList.Items.Add(inCaps.Name);
            }

            foreach (MidiOutPortCaps outCaps in new MidiOutPortCapsCollection())
            {
                this.MidiOutList.Items.Add(outCaps.Name);
            }
        }

        private void ShowError(Exception e)
        {
            MessageBox.Show(this, e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void NewDocument()
        {
            Program.Document = new MidiMapperDocument();

            UpdateUI();

            Text = "Midi Mapper - [unsaved]";
        }

        private void LoadDocument()
        {
            try
            {
                if (OpenFileDlg.ShowDialog(this) == DialogResult.OK)
                {
                    Program.Document = MidiMapperDocument.Load(OpenFileDlg.FileName);

                    UpdateUI();

                    Text = "Midi Mapper - [" + Program.Document.FilePath + "]";

                    Program.Document.IsDirty = false;
                }
            }
            catch (Exception e)
            {
                Program.Document = null;

                ShowError(e);
            }
        }

        private void SaveDocument(bool newLocation)
        {
            UpdateDocument();

            if (Program.Document != null)
            {
                newLocation |= String.IsNullOrEmpty(Program.Document.FilePath);
            }

            if (newLocation)
            {
                if (SaveFileDlg.ShowDialog(this) == DialogResult.OK)
                {
                    Program.Document.SaveAs(SaveFileDlg.FileName);
                }
            }
            else
            {
                Program.Document.Save();
            }

            Text = "Midi Mapper - [" + Program.Document.FilePath + "]";
        }

        private void UpdateUI()
        {
            ListenChannel.SelectedIndex = Program.Document.InChannel;
            SendChannel.SelectedIndex = Program.Document.OutChannel;
            VelocityOffsetCtrl.Value = Program.Document.VelocityOffset;
            MidiThruChk.Checked = Program.Document.MidiThru;

            // by ref
            NoteMapView.MidiNoteMap = Program.Document.MidiNoteMap;
        }

        private void UpdateDocument()
        {
            Program.Document.InChannel = ListenChannel.SelectedIndex;
            Program.Document.OutChannel = SendChannel.SelectedIndex;
            Program.Document.VelocityOffset = (byte)VelocityOffsetCtrl.Value;
            Program.Document.MidiThru = MidiThruChk.Checked;
        }

        // event handlers

        private void NewMapItem_Click(object sender, EventArgs e)
        {
            if (NoteMapView.NewMapItemUI())
            {
                Program.Document.IsDirty = true;
            }
        }

        private void EditMapItem_Click(object sender, EventArgs e)
        {
            if (NoteMapView.EditMapItemUI())
            {
                Program.Document.IsDirty = true;
            }
        }

        private void DeleteMapItem_Click(object sender, EventArgs e)
        {
            if (NoteMapView.DeleteMapItemUI(false))
            {
                Program.Document.IsDirty = true;
            }
        }

        private void StartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mapper != null)
                {
                    // stop mapper;
                    _mapper.Stop();
                    _mapper.Dispose();
                    _mapper = null;

                    StartStop.Text = "Start";
                }
                else
                {
                    if (MidiInList.SelectedIndex == -1 ||
                        MidiOutList.SelectedIndex == -1)
                    {
                        MessageBox.Show(this, Properties.Resources.Midi_NoInOrOutPortSelected, Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    // start mapper;
                    _mapper = new MidiNoteMapper();

                    if (ListenChannel.SelectedIndex > 0)
                    {
                        _mapper.InChannel = Byte.Parse(ListenChannel.Text);
                    }

                    if (SendChannel.SelectedIndex > 0)
                    {
                        _mapper.OutChannel = Byte.Parse(SendChannel.Text);
                    }

                    _mapper.VelocityOffset = (byte)VelocityOffsetCtrl.Value;
                    _mapper.MidiThru = MidiThruChk.Checked;

                    _mapper.Start(MidiInList.SelectedIndex, MidiOutList.SelectedIndex,
                        NoteMapView.MidiNoteMap.CompileIndex());

                    StartStop.Text = "Stop";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_mapper != null)
                {
                    _mapper.Stop();
                    _mapper.Dispose();
                    _mapper = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                e.Cancel = true;

                return;
            }

            DialogResult result = DialogResult.No;

            if (Program.Document != null &&
                Program.Document.IsDirty)
            {
                result = MessageBox.Show(this, "Do you want to save your map?", Text,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    SaveDocument(false);
                }
            }

            e.Cancel = (result == DialogResult.Cancel);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDocument();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            LoadDocument();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDocument(false);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveDocument(false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDocument(true);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ListenChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.Document != null)
            {
                Program.Document.IsDirty = true;
            }
        }

        private void SendChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.Document != null)
            {
                Program.Document.IsDirty = true;
            }
        }

        private void Preset_Click(object sender, EventArgs e)
        {
            ToolStripButton preset = (ToolStripButton)sender;
            Program.CurrentPresetIndex = Int32.Parse((string)preset.Tag);

            preset1.Checked = false;
            preset2.Checked = false;
            preset3.Checked = false;
            preset.Checked = true;

            if (Program.Document == null)
                NewDocument();
            else
                UpdateUI();

            if (Program.Document.FilePath == null)
                Text = "Midi Mapper - [unsaved]";
            else
                Text = "Midi Mapper - [" + Program.Document.FilePath + "]";
        }
    }
}