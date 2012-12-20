using System.Collections.Generic;
using System.Windows.Threading;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.RecordToFile.Midi;

namespace CannedBytes.Midi.RecordToFile
{
    class AppData
    {
        public AppData(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            MidiInPorts = new MidiInPortCapsCollection();
            MidiReceiver = new MidiReceiver(this);
            Events = new List<MidiFileEvent>();
        }

        public Dispatcher Dispatcher { get; private set; }

        public MidiInPortCapsCollection MidiInPorts { get; private set; }

        public MidiInPortCaps SelectedMidiInPort { get; set; }

        public MidiReceiver MidiReceiver { get; private set; }

        public List<MidiFileEvent> Events { get; private set; }
    }
}