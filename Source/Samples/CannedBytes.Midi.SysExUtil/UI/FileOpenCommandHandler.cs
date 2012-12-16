using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using CannedBytes.Midi.SysExUtil.Persistence;
using System.IO;

namespace CannedBytes.Midi.SysExUtil.UI
{
    class FileOpenCommandHandler : CommandHandler
    {
        private AppData appData;

        public FileOpenCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.FileOpen;
        }

        protected override void Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();

            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DereferenceLinks = true;
            ofd.Filter = "SysEx Files (*.syx)|*.syx|All Files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.Multiselect = false;
            ofd.RestoreDirectory = false;
            ofd.Title = "Select the SysEx file to open.";
            ofd.ValidateNames = true;

            if (ofd.ShowDialog() == true)
            {
                using (var fileStream = File.OpenRead(ofd.FileName))
                {
                    var serializer = new SysExSerializer();
                    var buffers = serializer.Deserialize(fileStream);

                    this.appData.SysExBuffers.Clear();

                    foreach (var buffer in buffers)
                    {
                        this.appData.SysExBuffers.Add(buffer);
                    }
                    
                }
            }


            base.Execute(sender, e);
        }
    }
}
