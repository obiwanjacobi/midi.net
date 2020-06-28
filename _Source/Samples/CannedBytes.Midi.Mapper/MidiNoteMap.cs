using System;
using System.Collections.Generic;

namespace CannedBytes.Midi.Mapper
{
    [Serializable]
    public class MidiNoteMap : List<MidiNoteMapItem>
    {
        internal MidiNoteMapItem Add(byte noteInNumber, byte noteOutNumber)
        {
            MidiNoteMapItem item = new MidiNoteMapItem();
            item.NoteInNumber = noteInNumber;
            item.NoteOutNumber = noteOutNumber;

            base.Add(item);

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