using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace CannedBytes.Midi.Samples.SysExUtil.UI
{
    class FileSaveCommandHandler : CommandHandler
    {
        private readonly AppData _appData;

        public FileSaveCommandHandler(AppData appData)
        {
            _appData = appData;
            Command = AppCommands.FileSave;
        }

        protected override void CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _appData.SelectedContentItems != null && _appData.SelectedContentItems.Any();
        }

        protected override void Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = "SysEx Files (*.syx)|*.syx|All Files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = false,
                Title = "Select the target SysEx file.",
                ValidateNames = true,
                OverwritePrompt = true
            };

            if (sfd.ShowDialog() == true)
            {
                using (var fileStream = File.OpenWrite(sfd.FileName))
                {
                    var serializer = new SysExSerializer();

                    serializer.Serialize(fileStream, _appData.SelectedContentItems);
                }
            }

            base.Execute(sender, e);
        }
    }
}
