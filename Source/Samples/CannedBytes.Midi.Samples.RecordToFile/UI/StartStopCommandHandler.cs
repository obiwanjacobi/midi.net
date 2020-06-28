using CannedBytes.Midi.Samples.RecordToFile.Midi;
using Microsoft.Win32;
using System.Windows.Input;

namespace CannedBytes.Midi.Samples.RecordToFile.UI
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

                _appData.MidiReceiver.Start(portId);
                ((UpdatingRoutedUICommand)Command).Text = "Stop";
            }
            else
            {
                _appData.MidiReceiver.Stop();
                ((UpdatingRoutedUICommand)Command).Text = "Start";

                SaveEventsToFile();
            }

            base.Execute(sender, e);
        }

        private void SaveEventsToFile()
        {
            if (_appData.Events != null &&
                _appData.Events.Count > 0)
            {
                var filePath = GetFilePath();

                if (filePath != null)
                {
                    MidiFileSerializer.Serialize(_appData.Events, filePath);
                }
            }
        }

        private string GetFilePath()
        {
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = "MIDI Files (*.mid)|*.mid|All Files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = false,
                Title = "Select the target MIDI file.",
                ValidateNames = true,
                OverwritePrompt = true
            };

            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }

            return null;
        }
    }
}