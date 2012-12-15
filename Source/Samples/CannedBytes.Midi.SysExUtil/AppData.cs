using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CannedBytes.Midi.SysExUtil.Midi;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace CannedBytes.Midi.SysExUtil
{
    /// <summary>
    /// Root data object for UI binding.
    /// </summary>
    internal class AppData
    {
        public AppData(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            MidiInPorts = new MidiInPortCapsCollection();
            MidiOutPorts = new MidiOutPortCapsCollection();

            SysExBuffers = new ObservableCollection<MidiSysExBuffer>();
            SysExReceiver = new MidiSysExReceiver(this);
        }

        public Dispatcher Dispatcher { get; private set; }

        public MidiInPortCapsCollection MidiInPorts { get; private set; }

        public MidiOutPortCapsCollection MidiOutPorts { get; private set; }

        public MidiInPortCaps SelectedMidiInPort { get; set; }

        public MidiOutPortCaps SelectedMidiOutPort { get; set; }

        public ObservableCollection<MidiSysExBuffer> SysExBuffers { get; private set; }



        public MidiSysExReceiver SysExReceiver { get; private set; }
    }
}
