using System;
using System.Windows.Forms;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.Mapper.UI
{
    partial class NoteMapView : UserControl
    {
        public NoteMapView()
        {
            InitializeComponent();
        }

        private MidiNoteMap _noteMap;

        public MidiNoteMap MidiNoteMap
        {
            get
            {
                //if (_noteMap == null)
                //{
                //    _noteMap = new MidiNoteMap();
                //}

                return _noteMap;
            }

            set
            {
                _noteMap = value;
                FillList();
            }
        }

        public bool NewMapItemUI()
        {
            MapItemForm dlg = new MapItemForm();

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                AddMapItem(dlg.NoteInNumber, dlg.NoteOutNumber, dlg.ItemName);

                return true;
            }

            return false;
        }

        public bool EditMapItemUI()
        {
            MapItemForm dlg = new MapItemForm();

            MidiNoteMapItem item = Selected;

            if (item != null)
            {
                dlg.ItemName = item.Name;
                dlg.NoteInNumber = item.NoteInNumber;
                dlg.NoteOutNumber = item.NoteOutNumber;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    item.Name = dlg.ItemName;
                    item.NoteInNumber = (byte)dlg.NoteInNumber;
                    item.NoteOutNumber = (byte)dlg.NoteOutNumber;

                    Refresh(item);

                    return true;
                }
            }

            return false;
        }

        public bool DeleteMapItemUI(bool confirm)
        {
            if (Selected != null)
            {
                RemoveMapItem(Selected);

                return true;
            }

            return false;
        }

        private void AddMapItem(int noteIn, int noteOut, string name)
        {
            MidiNoteMapItem item = MidiNoteMap.Add((byte)noteIn, (byte)noteOut);
            item.Name = name;

            AddToList(item);
        }

        private void RemoveMapItem(MidiNoteMapItem item)
        {
            MidiNoteMap.Remove(item);

            ListViewItem vwItem = FindViewItem(item);

            if (vwItem != null)
            {
                MapItems.Items.Remove(vwItem);

                if (Selected == item)
                {
                    if (MapItems.Items.Count == 0)
                    {
                        Selected = null;
                    }
                    else
                    {
                        Selected = MidiNoteMap.Find(delegate(MidiNoteMapItem mapItem)
                            { return true; });
                    }
                }
            }
        }

        public void Refresh(MidiNoteMapItem item)
        {
            ListViewItem vwItem = FindViewItem(item);

            if (vwItem != null)
            {
                Assign(vwItem, item);
            }
        }

        private MidiNoteMapItem _selected;

        public MidiNoteMapItem Selected
        {
            get { return _selected; }
            set
            {
                if (value != null)
                {
                    ListViewItem vwItem = FindViewItem(value);

                    if (vwItem != null)
                    {
                        vwItem.Selected = true;
                    }
                }

                _selected = value;
            }
        }

        private void FillList()
        {
            MapItems.Items.Clear();

            if (MidiNoteMap != null)
            {
                foreach (MidiNoteMapItem item in MidiNoteMap)
                {
                    AddToList(item);
                }
            }
        }

        private void Assign(ListViewItem vwItem, MidiNoteMapItem item)
        {
            MidiNoteName midiNN = new MidiNoteName();

            vwItem.SubItems.Clear();
            vwItem.Text = item.Name;
            vwItem.Tag = item;

            midiNN.NoteNumber = item.NoteInNumber;
            vwItem.SubItems.Add(midiNN.FullNoteName);
            vwItem.SubItems.Add(item.NoteInNumber.ToString());

            midiNN.NoteNumber = item.NoteOutNumber;
            vwItem.SubItems.Add(midiNN.FullNoteName);
            vwItem.SubItems.Add(item.NoteOutNumber.ToString());
        }

        private void AddToList(MidiNoteMapItem item)
        {
            ListViewItem vwItem = new ListViewItem();

            Assign(vwItem, item);

            MapItems.Items.Add(vwItem);
        }

        private ListViewItem FindViewItem(MidiNoteMapItem item)
        {
            foreach (ListViewItem vwItem in this.MapItems.Items)
            {
                if (vwItem.Tag == item)
                {
                    return vwItem;
                }
            }

            return null;
        }

        private void MapItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.MapItems.SelectedItems.Count > 0)
            {
                _selected = (MidiNoteMapItem)this.MapItems.SelectedItems[0].Tag;
            }
        }

        private void MapItems_DoubleClick(object sender, EventArgs e)
        {
            if (EditMapItemUI())
            {
                Program.Document.IsDirty = true;
            }
        }

        private void MapItems_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (DeleteMapItemUI(false))
                {
                    Program.Document.IsDirty = true;
                }
            }
        }
    }
}