using System.Windows.Input;
using CannedBytes.Midi.RecordToFile.Midi;
using Microsoft.Win32;

namespace CannedBytes.Midi.RecordToFile.UI
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

                this.appData.MidiReceiver.Start(portId);
                ((UpdatingRoutedUICommand)Command).Text = "Stop";
            }
            else
            {
                this.appData.MidiReceiver.Stop();
                ((UpdatingRoutedUICommand)Command).Text = "Start";

                SaveEventsToFile();
            }

            base.Execute(sender, e);
        }

        private void SaveEventsToFile()
        {
            if (this.appData.Events != null &&
                this.appData.Events.Count > 0)
            {
                var filePath = GetFilePath();

                if (filePath != null)
                {
                    using (var serializer = new MidiFileSerializer(filePath))
                    {
                        serializer.Serialize(this.appData.Events);
                    }
                }
            }
        }

        private string GetFilePath()
        {
            var sfd = new SaveFileDialog();

            sfd.AddExtension = true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.DereferenceLinks = true;
            sfd.Filter = "MIDI Files (*.mid)|*.mid|All Files (*.*)|*.*";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = false;
            sfd.Title = "Select the target MIDI file.";
            sfd.ValidateNames = true;
            sfd.OverwritePrompt = true;

            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }

            return null;
        }
    }
}