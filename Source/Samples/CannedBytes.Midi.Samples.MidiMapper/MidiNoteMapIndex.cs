using System.Collections.Generic;

namespace CannedBytes.Midi.Samples.MidiMapper
{
    class MidiNoteMapIndex : Dictionary<byte, List<MidiNoteMapItem>>
    {
        public void AddRange(ICollection<MidiNoteMapItem> items)
        {
            foreach (MidiNoteMapItem item in items)
            {
                List<MidiNoteMapItem> noteItems;

                if (ContainsKey(item.NoteInNumber))
                {
                    noteItems = base[item.NoteInNumber];
                }
                else
                {
                    noteItems = new List<MidiNoteMapItem>();
                    Add(item.NoteInNumber, noteItems);
                }

                noteItems.Add(item);
            }
        }

        public List<MidiNoteMapItem> Find(byte noteNumber)
        {
            if (ContainsKey(noteNumber))
            {
                return base[noteNumber];
            }

            return null;
        }
    }
}