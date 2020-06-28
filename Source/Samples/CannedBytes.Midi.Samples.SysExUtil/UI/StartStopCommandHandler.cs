using System.Windows.Input;

namespace CannedBytes.Midi.Samples.SysExUtil.UI
{
    internal class StartStopCommandHandler : CommandHandler
    {
        private readonly AppData _appData;

        public StartStopCommandHandler(AppData appData)
        {
            _appData = appData;
            Command = AppCommands.StartStop;
        }

        public bool IsStarted { get; private set; }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _appData.SelectedMidiInPort != null;
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            IsStarted = !IsStarted;

            if (IsStarted)
            {
                var portId = _appData.MidiInPorts.IndexOf(_appData.SelectedMidiInPort);

                _appData.SysExReceiver.Start(portId);
                ((UpdatingRoutedUICommand)Command).Text = "Stop";
            }
            else
            {
                _appData.SysExReceiver.Stop();
                ((UpdatingRoutedUICommand)Command).Text = "Start";
            }

            base.Execute(sender, e);
        }
    }
}
