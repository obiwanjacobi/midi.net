using CannedBytes.Midi.IO;
using CannedBytes.Midi.Samples.RecordToFile.Midi;
using System.Collections.Generic;
using System.Windows.Threading;

namespace CannedBytes.Midi.Samples.RecordToFile
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