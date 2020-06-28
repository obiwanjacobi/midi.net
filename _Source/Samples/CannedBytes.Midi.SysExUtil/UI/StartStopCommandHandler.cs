using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CannedBytes.Midi.SysExUtil.UI
{
    internal class StartStopCommandHandler : CommandHandler
    {
        private AppData appData;
        public StartStopCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.StartStop;
        }

        public bool IsStarted { get; private set; }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = appData.SelectedMidiInPort != null;
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            IsStarted = !IsStarted;

            if (IsStarted)
            {
                var portId = this.appData.MidiInPorts.IndexOf(this.appData.SelectedMidiInPort);

                this.appData.SysExReceiver.Start(portId);
                ((UpdatingRoutedUICommand)Command).Text = "Stop";
            }
            else
            {
                this.appData.SysExReceiver.Stop();
                ((UpdatingRoutedUICommand)Command).Text = "Start";
            }

            base.Execute(sender, e);
        }
    }
}
