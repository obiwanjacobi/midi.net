using System;
using System.Xml.Serialization;

namespace CannedBytes.Midi.Mapper
{
    [Serializable]
    public class MidiNoteMapItem
    {
        private string _name;

        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private byte _noteIn;

        [XmlAttribute("note-in")]
        public byte NoteInNumber
        {
            get { return _noteIn; }
            set { _noteIn = value; }
        }

        private byte _noteOut;

        [XmlAttribute("note-out")]
        public byte NoteOutNumber
        {
            get { return _noteOut; }
            set { _noteOut = value; }
        }
    }
}