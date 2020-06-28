using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using CannedBytes.Midi.SysExUtil.Persistence;
using System.IO;

namespace CannedBytes.Midi.SysExUtil.UI
{
    class FileSaveCommandHandler : CommandHandler
    {
        private AppData appData;

        public FileSaveCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.FileSave;
        }

        protected override void CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.appData.SelectedContentItems != null && this.appData.SelectedContentItems.Any();
        }

        protected override void Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();

            sfd.AddExtension = true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.DereferenceLinks = true;
            sfd.Filter = "SysEx Files (*.syx)|*.syx|All Files (*.*)|*.*";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = false;
            sfd.Title = "Select the target SysEx file.";
            sfd.ValidateNames = true;
            sfd.OverwritePrompt = true;

            if (sfd.ShowDialog() == true)
            {
                using (var fileStream = File.OpenWrite(sfd.FileName))
                {
                    var serializer = new SysExSerializer();

                    serializer.Serialize(fileStream, this.appData.SelectedContentItems);
                }
            }

            base.Execute(sender, e);
        }
    }
}
