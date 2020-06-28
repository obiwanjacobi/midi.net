using System;
using System.Collections.Generic;

namespace CannedBytes.Midi.Samples.MidiMapper
{
    [Serializable]
    public class MidiNoteMap : List<MidiNoteMapItem>
    {
        internal MidiNoteMapItem Add(byte noteInNumber, byte noteOutNumber)
        {
            MidiNoteMapItem item = new MidiNoteMapItem
            {
                NoteInNumber = noteInNumber,
                NoteOutNumber = noteOutNumber
            };

            Add(item);

            return item;
        }

        internal MidiNoteMapIndex CompileIndex()
        {
            MidiNoteMapIndex index = new MidiNoteMapIndex();

            index.AddRange(this);

            return index;
        }
    }
}