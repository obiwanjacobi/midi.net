﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using CannedBytes.Midi.SysExUtil.Midi;

namespace CannedBytes.Midi.SysExUtil
{
    /// <summary>
    /// Root data object for UI binding.
    /// </summary>
    internal class AppData : DisposableBase, INotifyPropertyChanged
    {
        public AppData(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            MidiInPorts = new MidiInPortCapsCollection();
            MidiOutPorts = new MidiOutPortCapsCollection();

            SysExBuffers = new ObservableCollection<MidiSysExBuffer>();
            SysExReceiver = new MidiSysExReceiver(this);
            SysExSender = new MidiSysExSender();
        }

        public Dispatcher Dispatcher { get; private set; }

        public MidiInPortCapsCollection MidiInPorts { get; private set; }

        public MidiOutPortCapsCollection MidiOutPorts { get; private set; }

        public MidiInPortCaps SelectedMidiInPort { get; set; }

        public MidiOutPortCaps SelectedMidiOutPort { get; set; }

        public ObservableCollection<MidiSysExBuffer> SysExBuffers { get; private set; }

        public IEnumerable<MidiSysExBuffer> SelectedContentItems { get; set; }

        public MidiSysExSender SysExSender { get; private set; }
        public MidiSysExReceiver SysExReceiver { get; private set; }

        private long recvErrCnt;
        public long ReceiveErrorCount
        {
            get { return this.recvErrCnt; }
            set
            {
                if (this.recvErrCnt != value)
                {
                    this.recvErrCnt = value;
                    OnPropertyChanged("ReceiveErrorCount");
                }
            }
        }

        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            this.SysExReceiver.Dispose();
            this.SysExSender.Dispose();
        }
    }
}