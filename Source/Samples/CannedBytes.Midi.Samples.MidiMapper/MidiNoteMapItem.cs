using System;
using System.Xml.Serialization;

namespace CannedBytes.Midi.Samples.MidiMapper
{
    [Serializable]
    public class MidiNoteMapItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("note-in")]
        public byte NoteInNumber { get; set; }

        [XmlAttribute("note-out")]
        public byte NoteOutNumber { get; set; }
    }
}