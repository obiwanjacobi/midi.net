using System.Collections.Generic;

namespace CannedBytes.Midi.Mapper
{
    class MidiNoteMapIndex : Dictionary<byte, List<MidiNoteMapItem>>
    {
        public void AddRange(ICollection<MidiNoteMapItem> items)
        {
            foreach (MidiNoteMapItem item in items)
            {
                List<MidiNoteMapItem> noteItems = null;

                if (base.ContainsKey(item.NoteInNumber))
                {
                    noteItems = base[item.NoteInNumber];
                }
                else
                {
                    noteItems = new List<MidiNoteMapItem>();
                    base.Add(item.NoteInNumber, noteItems);
                }

                noteItems.Add(item);
            }
        }

        public List<MidiNoteMapItem> Find(byte noteNumber)
        {
            if (base.ContainsKey(noteNumber))
            {
                return base[noteNumber];
            }

            return null;
        }
    }
}