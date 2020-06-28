using System.Linq;
using System.Windows.Input;

namespace CannedBytes.Midi.Samples.SysExUtil.UI
{
    class PlayCommandHandler : CommandHandler
    {
        private readonly AppData _appData;
        private int _currentPortId = -1;

        public PlayCommandHandler(AppData appData)
        {
            _appData = appData;
            Command = AppCommands.Play;
        }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _appData.SelectedMidiOutPort != null &&
                _appData.SelectedContentItems != null &&
                _appData.SelectedContentItems.Any();
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var portId = _appData.MidiOutPorts.IndexOf(_appData.SelectedMidiOutPort);

            if (_currentPortId != portId)
            {
                _appData.SysExSender.Close();
                _appData.SysExSender.Open(portId);
                _currentPortId = portId;
            }

            _appData.SysExSender.SendAll(_appData.SelectedContentItems);

            base.Execute(sender, e);
        }
    }
}