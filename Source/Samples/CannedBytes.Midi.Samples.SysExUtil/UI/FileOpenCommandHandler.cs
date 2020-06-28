using Microsoft.Win32;
using System.IO;

namespace CannedBytes.Midi.Samples.SysExUtil.UI
{
    class FileOpenCommandHandler : CommandHandler
    {
        private readonly AppData _appData;

        public FileOpenCommandHandler(AppData appData)
        {
            _appData = appData;
            Command = AppCommands.FileOpen;
        }

        protected override void Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = "SysEx Files (*.syx)|*.syx|All Files (*.*)|*.*",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = false,
                Title = "Select the SysEx file to open.",
                ValidateNames = true
            };

            if (ofd.ShowDialog() == true)
            {
                using (var fileStream = File.OpenRead(ofd.FileName))
                {
                    var serializer = new SysExSerializer();
                    var buffers = serializer.Deserialize(fileStream);

                    _appData.SysExBuffers.Clear();

                    foreach (var buffer in buffers)
                    {
                        _appData.SysExBuffers.Add(buffer);
                    }

                }
            }

            base.Execute(sender, e);
        }
    }
}
