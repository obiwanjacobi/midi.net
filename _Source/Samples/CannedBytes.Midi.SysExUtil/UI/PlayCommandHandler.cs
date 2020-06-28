using System.Linq;
using System.Windows.Input;

namespace CannedBytes.Midi.SysExUtil.UI
{
    class PlayCommandHandler : CommandHandler
    {
        private AppData appData;
        private int _currentPortId = -1;

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

            if (_currentPortId != portId)
            {
                this.appData.SysExSender.Close();
                this.appData.SysExSender.Open(portId);
                _currentPortId = portId;
            }

            this.appData.SysExSender.SendAll(this.appData.SelectedContentItems);

            base.Execute(sender, e);
        }
    }
}