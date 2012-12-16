using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CannedBytes.Midi.SysExUtil.UI
{
    class PlayCommandHandler : CommandHandler
    {
        private AppData appData;

        public PlayCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.Play;
        }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = appData.SelectedMidiOutPort != null && this.appData.SelectedContentItems != null && this.appData.SelectedContentItems.Any();
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var portId = this.appData.MidiOutPorts.IndexOf(this.appData.SelectedMidiOutPort);
            this.appData.SysExSender.Open(portId);

            try
            {
                this.appData.SysExSender.SendAll(this.appData.SelectedContentItems);
            }
            finally
            {
                this.appData.SysExSender.Close();
            }

            base.Execute(sender, e);
        }
    }
}
